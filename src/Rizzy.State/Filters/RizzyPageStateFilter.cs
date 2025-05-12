using Microsoft.AspNetCore.Components;         // For IComponent
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;                // For ViewResult, ContentResult, StatusCodeResult
using Microsoft.AspNetCore.Mvc.Abstractions;   // For ActionDescriptor
using Microsoft.AspNetCore.Mvc.Controllers;    // For ControllerActionDescriptor
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Rizzy.Components.Content;              // For RzPage, RzPartial (if RazorComponentResult exposes these)
using Rizzy.Htmx;                            // For HtmxRequestHeaderNames, HtmxResponseExtensions, SwapStyle
using Rizzy.State.Serialization;
using Rizzy.State.StateHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // For ValidationContext, Validator
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;                // For JsonPatch.Net with System.Text.Json
using System.Threading.Tasks;
using JsonPatch.Net;                         // For JsonPatch.Net

namespace Rizzy.State.Filters;

/// <summary>
/// An MVC Action Filter that manages the lifecycle of Rizzy's server-authoritative state tokens.
/// On GET requests for actions returning Rizzy views, it protects and embeds the initial state.
/// On htmx POST requests, it unprotects the incoming state, applies JSON patches,
/// performs validation and optimistic concurrency checks, and prepares the updated state
/// for the response.
/// </summary>
public class RizzyPageStateFilter : IAsyncResultFilter
{
    private readonly IRizzyStateProtector _stateProtector;
    private readonly IRizzyViewModelSerializer _viewModelSerializer;
    private readonly ILogger<RizzyPageStateFilter> _logger;
    // In a real application, inject your DbContext or a dedicated versioning service.
    // private readonly YourApplicationDbContext _dbContext;
    private readonly PlaceholderPersistenceService _persistenceService = new PlaceholderPersistenceService(); // Placeholder

    private static readonly JsonSerializerOptions _jsonPatchNodeOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };


    /// <summary>
    /// Initializes a new instance of the <see cref="RizzyPageStateFilter"/> class.
    /// </summary>
    /// <param name="stateProtector">Service for protecting and unprotecting state tokens.</param>
    /// <param name="viewModelSerializer">Service for serializing/deserializing view model state.</param>
    /// <param name="logger">Logger for diagnostics.</param>
    /// <exception cref="ArgumentNullException">Thrown if any required service is null.</exception>
    public RizzyPageStateFilter(
        IRizzyStateProtector stateProtector,
        IRizzyViewModelSerializer viewModelSerializer,
        ILogger<RizzyPageStateFilter> logger
        /*, YourApplicationDbContext dbContext */)
    {
        _stateProtector = stateProtector ?? throw new ArgumentNullException(nameof(stateProtector));
        _viewModelSerializer = viewModelSerializer ?? throw new ArgumentNullException(nameof(viewModelSerializer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        // _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc />
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var httpContext = context.HttpContext;
        var request = httpContext.Request;

        // Handle initial state protection for GET requests returning Rizzy views
        if (request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            await HandleGetRequestAsync(context, request);
        }
        // Handle state update for htmx POST requests
        else if (request.Headers[RizzyStateConstants.HtmxRequestHeaders.RZRequest] == "1" &&
                 request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                 request.HasFormContentType)
        {
            // Attempt to process the htmx POST; if it returns a result (e.g., an error),
            // then we short-circuit and don't call `await next()`.
            var didShortCircuit = await HandleHtmxPostRequestAsync(context, request, httpContext);
            if (didShortCircuit)
            {
                return;
            }
        }

        // Continue to the action execution or next filter
        var executedContext = await next();

        // After the action has executed, if it was a successful htmx POST,
        // ensure the new state token is prepared for the response header.
        if (request.Headers[RizzyStateConstants.HtmxRequestHeaders.RZRequest] == "1" &&
            request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
            IsSuccessfulActionExecution(executedContext))
        {
            await PrepareNewStateTokenForResponseAsync(httpContext, executedContext);
        }
    }

    private async Task HandleGetRequestAsync(ResultExecutingContext context, HttpRequest request)
    {
        if (context.Result is ViewResult vr && vr.Model != null && vr.Model.GetType().IsClass)
        {
            // For initial load, version can be a new timestamp or fetched if editing existing data.
            // This placeholder uses a simple timestamp; real apps might fetch from DB via vr.Model.Id.
            ulong initialVersion = (ulong)DateTime.UtcNow.ToBinary(); // Or await _persistenceService.GetVersionAsync(vr.Model);
            
            try
            {
                string token = _stateProtector.Protect(vr.Model, initialVersion);
                context.HttpContext.Items[RizzyStateConstants.HttpContextItems.StateForView] = token;
                _logger.LogTrace("RizzyState: Protected initial state token (Version: {Version}) for GET request to {Path}", initialVersion, request.Path);
            }
            catch (StateProtectionException ex)
            {
                _logger.LogError(ex, "RizzyState: Failed to protect initial state for GET request to {Path}", request.Path);
                // Optionally, set an error result or allow to proceed without state token.
            }
        }
        else if (context.Result is ViewResult vrWithError && vrWithError.Model != null && !vrWithError.Model.GetType().IsClass)
        {
            _logger.LogWarning("RizzyState: ViewResult.Model for GET request to {Path} is not a class type and cannot be state-protected.", request.Path);
        }
    }

    /// <summary>
    /// Handles the logic for an htmx POST request, including state unprotection, patching, validation, and concurrency checks.
    /// </summary>
    /// <returns>True if the request processing was short-circuited due to an error, false otherwise.</returns>
    private async Task<bool> HandleHtmxPostRequestAsync(ResultExecutingContext context, HttpRequest request, HttpContext httpContext)
    {
        _logger.LogTrace("RizzyState: Processing htmx POST request with state for {Path}", request.Path);

        string clientTokenJson = request.Form[RizzyStateConstants.HtmxRequestHeaders.RZState].ToString();
        string patchJson = request.Form[RizzyStateConstants.HtmxRequestHeaders.RZPatch].ToString();

        if (string.IsNullOrEmpty(clientTokenJson))
        {
            _logger.LogWarning("RizzyState: Missing '{StateParamName}' token in htmx POST request to {Path}", RizzyStateConstants.HtmxRequestHeaders.RZState, request.Path);
            context.Result = await CreateErrorResultAsync(httpContext, "State token is missing.", StatusCodes.Status400BadRequest, "#rz-errors");
            return true; // Short-circuit
        }

        Type? viewModelType = TryGetViewModelType(context.ActionDescriptor);
        if (viewModelType == null)
        {
            _logger.LogError("RizzyState: Could not determine ViewModel type for htmx POST request to {Path}", request.Path);
            context.Result = await CreateErrorResultAsync(httpContext, "Server error: Cannot determine view model type.", StatusCodes.Status500InternalServerError, "#rz-errors");
            return true; // Short-circuit
        }

        // Unprotect state (generic method invocation via reflection)
        var tryUnprotectMethod = typeof(IRizzyStateProtector).GetMethod(nameof(IRizzyStateProtector.TryUnprotect))!.MakeGenericMethod(viewModelType);
        object?[] unprotectArgs = [clientTokenJson, null, (ulong)0];
        bool unprotectSuccess = (bool)tryUnprotectMethod.Invoke(_stateProtector, unprotectArgs)!;
        object? clientModel = unprotectArgs[1];
        ulong clientVersion = (ulong)unprotectArgs[2];

        if (!unprotectSuccess || clientModel == null)
        {
            _logger.LogWarning("RizzyState: Failed to unprotect state token for htmx POST request to {Path}. Token might be invalid or expired.", request.Path);
            context.Result = await CreateErrorResultAsync(httpContext, "Invalid or expired state. Please try again.", StatusCodes.Status400BadRequest, "#rz-errors");
            return true; // Short-circuit
        }

        // Apply JSON Patch
        if (!string.IsNullOrEmpty(patchJson) && patchJson != "[]")
        {
            ApplyJsonPatchToModel(patchJson, clientModel, viewModelType, context.ModelState);
        }

        // Re-validate model after patching
        if (context.ModelState.IsValid) // Only if patch didn't introduce errors caught by JsonPatch.Net structure
        {
            ValidateModelWithAnnotations(clientModel, httpContext.RequestServices, context.ModelState);
        }

        if (!context.ModelState.IsValid)
        {
            _logger.LogInformation("RizzyState: Model state is invalid after patch/validation for {Path}. Errors: {Errors}",
                request.Path, string.Join("; ", context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            context.Result = await CreateValidationSummaryResultAsync(httpContext, context.ModelState, "#rz-validation-summary"); // Standard validation summary target
            // Allow action to run to render view with errors, but ensure state passed to action is the *patched* one
            httpContext.Items[RizzyStateConstants.HttpContextItems.CurrentViewModelInstance] = clientModel;
            httpContext.Items[RizzyStateConstants.HttpContextItems.CurrentVersion] = clientVersion; // Keep original version for the response if validation fails
            // Do NOT short-circuit here; let the action render the view with errors. The `PrepareNewStateTokenForResponseAsync` will handle not issuing a *new* token.
            return false; 
        }

        // Optimistic Concurrency Check
        ulong currentDbVersion = await _persistenceService.GetVersionAsync(clientModel);
        if (clientVersion != currentDbVersion)
        {
            _logger.LogWarning("RizzyState: Concurrency conflict for {Path}. Client version: {ClientVersion}, DB version: {DbVersion}", request.Path, clientVersion, currentDbVersion);
            context.Result = await CreateErrorResultAsync(httpContext, "Your data is out of date. The content has been updated by someone else. Please refresh and try again.", StatusCodes.Status409Conflict, "#rz-errors");
            httpContext.Response.Htmx(r => r.PushUrl("false")); // Prevent history pollution
            return true; // Short-circuit
        }

        // All checks passed, store model for action and subsequent token generation
        httpContext.Items[RizzyStateConstants.HttpContextItems.CurrentViewModelInstance] = clientModel;
        httpContext.Items[RizzyStateConstants.HttpContextItems.CurrentVersion] = clientVersion;
        _logger.LogTrace("RizzyState: Successfully processed incoming state and patch for {Path}. Version: {Version}", request.Path, clientVersion);

        return false; // Do not short-circuit, proceed to action
    }
    
    private void ApplyJsonPatchToModel(string patchJson, object clientModel, Type viewModelType, ModelStateDictionary modelState)
    {
        try
        {
            JsonNode? targetNode = JsonSerializer.SerializeToNode(clientModel, viewModelType, _jsonPatchNodeOptions);
            if (targetNode == null)
            {
                _logger.LogError("RizzyState: Failed to serialize clientModel to JsonNode for patching. ViewModelType: {ViewModelType}", viewModelType.FullName);
                modelState.AddModelError("", "Internal server error: Could not prepare model for updates.");
                return;
            }

            var patch = JsonPatch.Net.JsonPatch.Parse(patchJson);
            PatchResult? patchResult = patch.Apply(targetNode);

            if (!patchResult.IsSuccess)
            {
                _logger.LogWarning("RizzyState: JSON Patch application failed: {Error}. Patch: {PatchJson}", patchResult.Error, patchJson.Length > 200 ? patchJson.Substring(0,200) : patchJson);
                modelState.AddModelError("", $"Update application failed: {patchResult.Error}");
            }
            else
            {
                // Deserialize back to the view model type
                var updatedModel = targetNode.Deserialize(viewModelType, _jsonPatchNodeOptions);
                if (updatedModel == null)
                {
                    _logger.LogError("RizzyState: Failed to deserialize model after successful patch. ViewModelType: {ViewModelType}", viewModelType.FullName);
                    modelState.AddModelError("", "Internal server error: Could not apply updates to model.");
                }
                else
                {
                    // This assumes clientModel is a reference type and its properties are updated.
                    // For more complex scenarios or immutable models, this might need adjustment
                    // (e.g., replacing clientModel instance, which can be tricky with DI scopes).
                    // For now, we assume JsonPatch.Net modified the JsonNode, and deserializing it
                    // back into the *original* clientModel instance via reflection or by replacing it.
                    // Simplest for now: if Deserialize creates a new instance, we need to replace clientModel.
                    // However, common practice is to apply to an existing instance.
                    // Let's assume clientModel properties were updated by reference through targetNode, if possible,
                    // or that it needs to be replaced if Deserialize returns a new object.
                    // For safety, we'll assume Deserialize creates a new instance and we update our reference:
                    ShallowCopy(updatedModel, clientModel, viewModelType); // Copy patched values back to original instance
                }
            }
        }
        catch (JsonException jsonEx)
        {
            _logger.LogWarning(jsonEx, "RizzyState: Malformed JSON Patch document: {PatchJson}", patchJson.Length > 200 ? patchJson.Substring(0,200) : patchJson);
            modelState.AddModelError("", "Malformed update request.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RizzyState: Unexpected error applying JSON Patch: {PatchJson}", patchJson.Length > 200 ? patchJson.Substring(0,200) : patchJson);
            modelState.AddModelError("", "Error applying updates.");
        }
    }

    private static void ShallowCopy(object source, object destination, Type type)
    {
        // Basic shallow copy for POCOs. Not for complex scenarios.
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite))
        {
            prop.SetValue(destination, prop.GetValue(source));
        }
    }


    private void ValidateModelWithAnnotations(object model, IServiceProvider serviceProvider, ModelStateDictionary modelState)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, serviceProvider, items: null);
        bool isValidByAnnotations = Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);

        if (!isValidByAnnotations)
        {
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames.DefaultIfEmpty(string.Empty))
                {
                    modelState.AddModelError(memberName, validationResult.ErrorMessage ?? "Validation error.");
                }
            }
        }
    }

    private async Task PrepareNewStateTokenForResponseAsync(HttpContext httpContext, ResultExecutedContext executedContext)
    {
        // If ModelState was marked invalid by the filter or action, do not generate a new success token.
        // Instead, re-issue the old token so the client doesn't lose its current (uncommitted) patch context.
        if (executedContext.Result is ObjectResult { Value: ValidationProblemDetails } || 
            (executedContext.Result is ContentResult cr && cr.StatusCode == StatusCodes.Status422UnprocessableEntity) ||
            !context.ModelState.IsValid) // Check context.ModelState as well, as action might have added errors
        {
            if (httpContext.Request.Form.TryGetValue(RizzyStateConstants.HtmxRequestHeaders.RZState, out var oldClientTokenValues))
            {
                string oldClientToken = oldClientTokenValues.ToString();
                if (!string.IsNullOrEmpty(oldClientToken))
                {
                    httpContext.Items[RizzyStateConstants.HttpContextItems.NewStateForHeader] = oldClientToken;
                    _logger.LogTrace("RizzyState: Validation failed or action returned validation problem; re-issuing old state token for {Path}.", httpContext.Request.Path);
                    return;
                }
            }
            _logger.LogWarning("RizzyState: Validation failed for {Path}, but no old client token found to re-issue. RZ-State header will be absent.", httpContext.Request.Path);
            return;
        }

        object? finalViewModel = null;
        ulong finalVersion = 0;

        // Priority 1: Model explicitly set by the action for token update via HttpContext.Items
        if (httpContext.Items.TryGetValue(RizzyStateConstants.HttpContextItems.UpdatedViewModelForToken, out var updatedModelFromAction) && 
            updatedModelFromAction != null && updatedModelFromAction.GetType().IsClass)
        {
            finalViewModel = updatedModelFromAction;
            // Assuming if it's here, it has been saved and its version is current or should be new.
            await _persistenceService.SaveChangesAsync(finalViewModel); // Ensure save if not already done
            finalVersion = _persistenceService.GetUpdatedVersion(finalViewModel);
            _logger.LogTrace("RizzyState: Using ViewModel from HttpContext.Items (UpdatedViewModelForToken) for new token. Version: {Version}", finalVersion);
        }
        // Priority 2: Model from HttpContext.Items (CurrentViewModelInstance), potentially modified by action
        else if (httpContext.Items.TryGetValue(RizzyStateConstants.HttpContextItems.CurrentViewModelInstance, out var currentModelFromFilter) && 
                 currentModelFromFilter != null && currentModelFromFilter.GetType().IsClass)
        {
            finalViewModel = currentModelFromFilter;
            // Assume action might have modified it directly and might have saved it.
            // If it wasn't explicitly put in UpdatedViewModelForToken, we ensure save here.
            await _persistenceService.SaveChangesAsync(finalViewModel);
            finalVersion = _persistenceService.GetUpdatedVersion(finalViewModel);
            _logger.LogTrace("RizzyState: Using ViewModel from HttpContext.Items (CurrentViewModelInstance) after action for new token. Version: {Version}", finalVersion);
        }
        // Priority 3: Model from the IResult if it's a Rizzy RazorComponentResult
        else if (executedContext.Result is RazorComponentResult rizzyResult)
        {
            finalViewModel = ExtractViewModelFromRazorComponentResult(rizzyResult);
            if (finalViewModel != null)
            {
                await _persistenceService.SaveChangesAsync(finalViewModel);
                finalVersion = _persistenceService.GetUpdatedVersion(finalViewModel);
                _logger.LogTrace("RizzyState: Using ViewModel from RazorComponentResult for new token. Version: {Version}", finalVersion);
            }
        }
        
        if (finalViewModel != null && finalViewModel.GetType().IsClass)
        {
            try
            {
                string newToken = _stateProtector.Protect(finalViewModel, finalVersion);
                httpContext.Items[RizzyStateConstants.HttpContextItems.NewStateForHeader] = newToken;
                _logger.LogTrace("RizzyState: Successfully protected new state token (Version: {Version}) for htmx POST request to {Path}", finalVersion, httpContext.Request.Path);
            }
            catch (StateProtectionException ex)
            {
                _logger.LogError(ex, "RizzyState: Failed to protect new state token for htmx POST request to {Path}", httpContext.Request.Path);
                // Avoid sending a malformed or no token if protection fails. Client will keep old one.
            }
        }
        else if (httpContext.Request.Headers.ContainsKey(HtmxRequestHeaderNames.HtmxRequest)) // Only if htmx request
        {
             _logger.LogWarning("RizzyState: No final view model found to protect for {Path} after action. RZ-State header will not be updated with a new token.", httpContext.Request.Path);
             // Re-issue old token to prevent client from losing its patch context on non-state-changing POSTs
            if (httpContext.Request.Form.TryGetValue(RizzyStateConstants.HtmxRequestHeaders.RZState, out var oldClientTokenValues))
            {
                httpContext.Items[RizzyStateConstants.HttpContextItems.NewStateForHeader] = oldClientTokenValues.ToString();
                 _logger.LogTrace("RizzyState: Re-issuing old client state token as no new view model was specified for update.");
            }
        }
    }

    private static object? ExtractViewModelFromRazorComponentResult(RazorComponentResult rizzyResult)
    {
        if (rizzyResult.Parameters.TryGetValue("ComponentParameters", out var componentParamsObj))
        {
            if (componentParamsObj is IDictionary<string, object?> componentParamsDict)
            {
                if (componentParamsDict.TryGetValue("ViewModel", out var vmFromParams) && vmFromParams != null && vmFromParams.GetType().IsClass) return vmFromParams;
                if (componentParamsDict.TryGetValue("Model", out vmFromParams) && vmFromParams != null && vmFromParams.GetType().IsClass) return vmFromParams;
            }
            if (componentParamsObj != null && componentParamsObj.GetType().IsClass)
            {
                return componentParamsObj; // ComponentParameters itself is the model
            }
        }
        return null;
    }

    private static bool IsSuccessfulActionExecution(ResultExecutedContext executedContext) =>
        (executedContext.Result is not StatusCodeResult scr || (scr.StatusCode >= 200 && scr.StatusCode < 300)) &&
        !executedContext.Canceled && executedContext.Exception == null && executedContext.ExceptionHandled == false;
        
    private object? GetModelId(object model)
    {
        // Placeholder: Implement robust logic to get a unique ID from your model,
        // e.g., a property named "Id", "Key" or via an interface like IIdentifiable.
        var idProperty = model.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) ??
                         model.GetType().GetProperty("Key", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return idProperty?.GetValue(model);
    }

    /// <summary>
    /// Tries to determine the ViewModel type based on the action descriptor.
    /// This is a placeholder and needs robust implementation based on application conventions
    /// or an explicit registration mechanism for components and their view models.
    /// </summary>
    private Type? TryGetViewModelType(ActionDescriptor actionDescriptor)
    {
        if (actionDescriptor is ControllerActionDescriptor controllerActionDesc)
        {
            // Convention 1: Look for an action parameter that looks like a ViewModel
            // This is a common pattern for POST/PUT actions that bind to a model.
            var modelParameter = controllerActionDesc.Parameters
                .FirstOrDefault(p => 
                    p.ParameterType.IsClass && // Ensure it's a class
                    !typeof(IFormCollection).IsAssignableFrom(p.ParameterType) &&
                    !typeof(IFormFile).IsAssignableFrom(p.ParameterType) &&
                    !p.ParameterType.IsPrimitive && p.ParameterType != typeof(string) &&
                    (p.BindingInfo?.BindingSource == BindingSource.Form || 
                     p.BindingInfo?.BindingSource == BindingSource.Body ||
                     p.Name.EndsWith("ViewModel", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.EndsWith("Model", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.Equals("input", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.Equals("model", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.Equals("viewModel", StringComparison.OrdinalIgnoreCase)
                     // Add more heuristics if needed
                    )
                );

            if (modelParameter != null)
            {
                _logger.LogTrace("RizzyState: Deduced ViewModel type '{ViewModelType}' from action parameter '{ParameterName}' for {ControllerAction}", 
                    modelParameter.ParameterType.FullName, modelParameter.Name, controllerActionDesc.DisplayName);
                return modelParameter.ParameterType;
            }

            // Convention 2: If the controller is generic, like RzControllerWithViews<TModel>
            var controllerType = controllerActionDesc.ControllerTypeInfo;
            if (controllerType.IsGenericType && controllerType.GetGenericTypeDefinition() == typeof(RzControllerWithViews<>))
            {
                 var genericArgType = controllerType.GetGenericArguments()[0];
                 _logger.LogTrace("RizzyState: Deduced ViewModel type '{ViewModelType}' from generic RzControllerWithViews for controller {ControllerName}", 
                    genericArgType.FullName, controllerActionDesc.ControllerName);
                return genericArgType;
            }
            
            _logger.LogWarning("RizzyState: Could not deduce ViewModel type for controller {ControllerName}, action {ActionName}. " +
                               "Consider using an explicit model parameter, a generic controller base type like RzControllerWithViews<TModel>, " +
                               "or implement a ViewModel type registration system.",
                controllerActionDesc.ControllerName, controllerActionDesc.ActionName);
        }
        // TODO: Add logic for Razor Pages (PageActionDescriptor) or Minimal APIs if needed.
        return null;
    }

    private async Task<ContentResult> CreateErrorResultAsync(HttpContext httpContext, string errorMessage, int statusCode, string hxTarget, string? logMessageOverride = null)
    {
        _logger.LogWarning(logMessageOverride ?? "RizzyState: Returning error to client: {ErrorMessage}, StatusCode: {StatusCode}, Target: {HxTarget}",
            errorMessage, statusCode, hxTarget);

        // In a real app, use IRizzyService or HtmlRenderer to render a Blazor error component.
        var htmlContent = $"<div class='text-danger border border-danger rounded p-3 my-2' id='{hxTarget.TrimStart('#')}'>{System.Net.WebUtility.HtmlEncode(errorMessage)}</div>";
        
        httpContext.Response.Htmx(r => r.Retarget(hxTarget).Reswap(SwapStyle.innerHTML));
        
        return new ContentResult { Content = htmlContent, ContentType = "text/html", StatusCode = statusCode };
    }

    private async Task<ContentResult> CreateValidationSummaryResultAsync(HttpContext httpContext, ModelStateDictionary modelState, string hxTarget)
    {
        _logger.LogInformation("RizzyState: Rendering validation summary. Target: {HxTarget}", hxTarget);
        
        // In a real app, use IRizzyService or HtmlRenderer to render your standard _ValidationSummary Blazor component.
        var sb = new StringBuilder();
        sb.AppendLine($"<div id='{hxTarget.TrimStart('#')}'>"); // Ensure the target itself is part of the swap for proper replacement
        sb.AppendLine("<ul class=\"validation-summary-errors text-danger ps-3\" data-valmsg-summary=\"true\">"); // Added padding start for ul
        foreach(var error in modelState.Where(kvp => kvp.Value != null).SelectMany(kvp => kvp.Value!.Errors))
        {
            if (!string.IsNullOrWhiteSpace(error.ErrorMessage)) // Only include errors with messages
            {
                 sb.AppendLine($"<li class=\"mb-1\">{System.Net.WebUtility.HtmlEncode(error.ErrorMessage)}</li>");
            }
        }
        sb.AppendLine("</ul>");
        sb.AppendLine("</div>");
        var htmlContent = sb.ToString();

        httpContext.Response.Htmx(r => r.Retarget(hxTarget).Reswap(SwapStyle.innerHTML));

        return new ContentResult { Content = htmlContent, ContentType = "text/html", StatusCode = StatusCodes.Status422UnprocessableEntity };
    }

    // Placeholder for a service that interacts with the database for versioning.
    // Replace with your actual DbContext or repository pattern.
    private sealed class PlaceholderPersistenceService
    {
        public Task<ulong> GetVersionAsync(object viewModel)
        {
            // Simulate fetching a version. In a real app, this would query your database
            // based on an ID from the viewModel.
            // Example: If viewModel has public ulong RowVersion { get; set; }
            var rvProp = viewModel.GetType().GetProperty("RowVersion", BindingFlags.Public | BindingFlags.Instance);
            if (rvProp != null && rvProp.GetValue(viewModel) is ulong rv) return Task.FromResult(rv);
            
            return Task.FromResult((ulong)DateTime.UtcNow.ToBinary() - 10000); // Simulate an older version
        }

        public Task SaveChangesAsync(object viewModel)
        {
            // Simulate saving changes and the database updating a RowVersion.
            // Example: If viewModel has public ulong RowVersion { get; set; }
            var rvProp = viewModel.GetType().GetProperty("RowVersion", BindingFlags.Public | BindingFlags.Instance);
            if (rvProp != null && rvProp.CanWrite) {
                rvProp.SetValue(viewModel, (ulong)DateTime.UtcNow.ToBinary());
            }
            return Task.CompletedTask;
        }

        public ulong GetUpdatedVersion(object viewModel)
        {
             // Simulate getting the new version after a save.
            // Example: If viewModel has public ulong RowVersion { get; set; }
            var rvProp = viewModel.GetType().GetProperty("RowVersion", BindingFlags.Public | BindingFlags.Instance);
            if (rvProp != null && rvProp.GetValue(viewModel) is ulong rv) return rv;

            return (ulong)DateTime.UtcNow.ToBinary();
        }
    }
}