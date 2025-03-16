using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Rizzy;

public static class HttpContextExtensions
{
    /// <summary>
    /// Retrieves the nonce values for the current HTTP request. New nonce values are generated and cached if not already present.
    /// The recommended way to generate nonce values is to utilize an IRizzyNonceProvider service implementation directly that can
    /// be injected into a Controller, Component, View, etc.
    /// </summary>
    /// <returns>An instance of <see cref="string"/> containing nonce value.</returns>
    public static string GetNonce(this HttpContext context)
    {
	    var provider = context.RequestServices.GetRequiredService<IRizzyNonceProvider>();

	    return provider.GetNonce();
    }


	/// <summary>
	/// Initializes the form mapping for the current HTTP request by extracting form values
	/// and files from <see cref="HttpContext.Request.Form"/> and invoking the internal
	/// <c>HttpContextFormDataProvider.SetFormData</c> method via reflection.
	/// </summary>
	/// <remarks>
	/// This method uses reflection to access internal types from the Microsoft.AspNetCore.Components.Endpoints assembly.
	/// It retrieves the form field named "_handler" (which serves as the mapping scope) from the form data.
	/// Typically, form data is available during POST requests; however, other HTTP methods (such as PUT or PATCH)
	/// might also include form data if the Content-Type is set to "application/x-www-form-urlencoded" or "multipart/form-data".
	/// This method is intended to be called early in the controller action (e.g. in OnActionExecuting) to ensure that
	/// any components using the [SupplyParameterFromForm] attribute receive the expected values.
	/// </remarks>
	internal static void InitializeBlazorFormData(this HttpContext context)
	{
	    // Only proceed if the request method is POST.
	    // Note: Although POST is the most common, you may consider including PUT or PATCH if you expect form data in those requests.
	    if (!context.Request.HasFormContentType)
	        return;

	    // Retrieve form values and files from the current HTTP request.
	    var form = context.Request.Form;
	    var formFiles = context.Request.Form.Files;

	    // Get the handler value from the form; this field is used as the mapping scope.
	    string? handlerValue = form["_handler"].FirstOrDefault();
	    if (handlerValue is null)
	        return;

	    // Use reflection to load the internal types from the "Microsoft.AspNetCore.Components.Endpoints" assembly.
	    var types = Assembly.Load("Microsoft.AspNetCore.Components.Endpoints").GetTypes();
	    var providerType = types.First(t => t.Name == "HttpContextFormDataProvider");
	    var formDictType = types.First(t => t.Name == "FormCollectionReadOnlyDictionary");

	    // Retrieve the HttpContextFormDataProvider instance from the DI container.
	    object formDataProvider = context.RequestServices.GetRequiredService(providerType)
	                              ?? throw new Exception("HttpContextFormDataProvider is not registered.");

	    // Create an instance of FormCollectionReadOnlyDictionary using the current form data.
	    object formDictInstance = Activator.CreateInstance(formDictType, form)
	                              ?? throw new Exception("Unable to create an instance of FormCollectionReadOnlyDictionary.");

	    // Retrieve the SetFormData method and invoke it with the handler, form dictionary, and form files.
	    MethodInfo setFormDataMethod = providerType.GetMethod("SetFormData", BindingFlags.Public | BindingFlags.Instance)
	                                   ?? throw new Exception("SetFormData method not found.");

	    setFormDataMethod.Invoke(formDataProvider, [ handlerValue, formDictInstance, formFiles ]);
	}
}