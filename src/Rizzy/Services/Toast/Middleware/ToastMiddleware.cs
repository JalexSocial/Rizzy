using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection; // Required for GetRequiredService
using Microsoft.Extensions.Logging;
using Rizzy.Htmx;
using System; // Required for ArgumentNullException
using System.Linq; // Required for Any()
using System.Threading.Tasks; // Required for Task

namespace Rizzy;

/// <summary>
/// Middleware responsible for intercepting HTTP responses for HTMX requests.
/// It reads pending toast notifications from the registered <see cref="IToastService"/>,
/// adds them to the 'HX-Trigger' header for client-side display via a specified event (e.g., "rz:toast-broadcast"),
/// and then clears the notifications from the service for the current request.
/// </summary>
/// <remarks>
/// This middleware should typically be registered as a singleton in the application's pipeline,
/// usually after routing and authentication but before the endpoint execution, to ensure it can
/// capture responses correctly. It relies on a scoped <see cref="IToastService"/> being registered.
/// </remarks>
public class ToastMiddleware
{
    private readonly ILogger<ToastMiddleware> _logger;
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToastMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    /// <param name="logger">The logger instance for recording diagnostic information.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> or <paramref name="logger"/> is null.</exception>
    public ToastMiddleware(RequestDelegate next, ILogger<ToastMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes the HTTP request, registers a callback to potentially add toast notifications
    /// to the response headers for HTMX requests, and passes control to the next middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <remarks>
    /// This method resolves the scoped <see cref="IToastService"/> and passes it to the OnStarting callback
    /// via the state parameter to avoid scope conflicts inherent in middleware field injection.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Pass to next if not an htmx request
        if (!context.Request.IsHtmx())
        {
            await _next(context);
            return;            
        }
        
        // Resolve the scoped IToastService within the request scope.
        // Do NOT store this in a field of the middleware (singleton) instance.
        var toastService = context.RequestServices.GetRequiredService<IToastService>();

        // Register the callback to execute just before response headers are sent.
        // Pass the HttpContext and the resolved scoped toastService as state.
        context.Response.OnStarting(AddToastTriggerCallback, (context, toastService));

        // Continue processing the request pipeline.
        await _next(context);
    }

    /// <summary>
    /// Callback executed via <see cref="HttpResponse.OnStarting"/>. Checks if the request is an HTMX request
    /// and if there are any pending toast notifications. If both are true, it adds an 'HX-Trigger' header
    /// containing the serialized notifications and clears them from the service.
    /// </summary>
    /// <param name="state">The state object passed from InvokeAsync, containing the HttpContext and IToastService.</param>
    /// <returns>A <see cref="Task"/> representing the completion of the callback.</returns>
    private Task AddToastTriggerCallback(object state)
    {
        // Safely unpack the state tuple
        if (state is not (HttpContext httpContext, IToastService toastService))
        {
            _logger.LogWarning("Invalid state object received in {CallbackName}. Expected (HttpContext, IToastService).", nameof(AddToastTriggerCallback));
            return Task.CompletedTask; 
        }

        try
        {
            // Avoid processing if response has already started (shouldn't happen with OnStarting, but defensive)
            if (httpContext.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started in {CallbackName}. Cannot add HX-Trigger.", nameof(AddToastTriggerCallback));
                return Task.CompletedTask;
            }

            _logger.LogTrace("Processing HTMX request for potential toasts.");

            var messages = toastService.ReadAllNotifications();

            if (messages.Any())
            {
                _logger.LogInformation("Found {Count} toast notifications to trigger.", messages.Count);

                var response = new HtmxResponse(httpContext);

                // Trigger a client-side event with the toast messages as detail
                // The client-side JS (e.g., in rizzyui.js) should listen for "rz:toast-broadcast"
                response.Trigger(Constants.ToastBroadcastEventName, messages);

                // Clear the notifications from the service for this request scope
                toastService.RemoveAll();
                _logger.LogTrace("Cleared toast notifications from service.");
            }
        }
        catch (Exception ex)
        {
            // Log any errors during the callback, but don't prevent the response from sending
            _logger.LogError(ex, "Error occurred in {CallbackName} while processing toasts.", nameof(AddToastTriggerCallback));
        }

        return Task.CompletedTask; // Or Task.FromResult(0) pre .NET 6
    }
}