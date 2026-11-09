using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Rizzy;

/// <summary>
/// Supplies posted form data to Blazor's SSR form mapping (<c>[SupplyParameterFromForm]</c>) for
/// components rendered through <see cref="Microsoft.AspNetCore.Http.HttpResults.RazorComponentResult"/>
/// (RIZZY-020, RIZZY-032).
/// </summary>
/// <remarks>
/// <para>
/// Razor component endpoints (<c>MapRazorComponents</c>) read the form and hand it to Blazor
/// themselves, but <c>RazorComponentResult</c> does not, and ASP.NET Core 10 offers no public API
/// for it: <c>HttpContextFormDataProvider</c> and its <c>SetFormData</c> method are internal. Rizzy
/// therefore calls that method through reflection.
/// </para>
/// <para>
/// The reflection is resolved once per process and fails with a descriptive
/// <see cref="InvalidOperationException"/> if a future framework version changes these internals.
/// The form is read with <see cref="HttpRequest.ReadFormAsync(CancellationToken)"/>, never
/// synchronously, and only for requests with a form content type and a <c>_handler</c> field.
/// </para>
/// </remarks>
internal static class BlazorFormData
{
    private const string AssemblyName = "Microsoft.AspNetCore.Components.Endpoints";
    private const string ProviderTypeName = "Microsoft.AspNetCore.Components.Endpoints.HttpContextFormDataProvider";
    private const string DictionaryTypeName = "Microsoft.AspNetCore.Components.Endpoints.EndpointHtmlRenderer+FormCollectionReadOnlyDictionary";
    private const string HandlerField = "_handler";
    private static readonly object InitializedKey = new();

    private static readonly Lazy<Accessor> s_accessor = new(Accessor.Resolve, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>Resolves the framework internals, throwing if they are not available. Used by tests.</summary>
    internal static void EnsureSupported() => _ = s_accessor.Value;

    /// <summary>
    /// Reads the posted form (if any) and makes it available to Blazor form mapping. Runs at most
    /// once per request.
    /// </summary>
    public static async Task InitializeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Request.HasFormContentType || context.Items.ContainsKey(InitializedKey))
            return;

        context.Items[InitializedKey] = true;

        var form = await context.Request.ReadFormAsync(context.RequestAborted).ConfigureAwait(false);
        var handler = form[HandlerField].FirstOrDefault();
        if (string.IsNullOrEmpty(handler))
            return;

        s_accessor.Value.SetFormData(context, handler, form);
    }

    private sealed class Accessor
    {
        private readonly Type _providerType;
        private readonly ConstructorInfo _dictionaryConstructor;
        private readonly MethodInfo _setFormData;

        private Accessor(Type providerType, ConstructorInfo dictionaryConstructor, MethodInfo setFormData)
        {
            _providerType = providerType;
            _dictionaryConstructor = dictionaryConstructor;
            _setFormData = setFormData;
        }

        public static Accessor Resolve()
        {
            try
            {
                var assembly = Assembly.Load(AssemblyName);
                var providerType = assembly.GetType(ProviderTypeName, throwOnError: true)!;
                var dictionaryType = assembly.GetType(DictionaryTypeName, throwOnError: true)!;

                var constructor = dictionaryType.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, [typeof(IFormCollection)])
                    ?? throw new MissingMethodException(DictionaryTypeName, ".ctor(IFormCollection)");

                var setFormData = providerType.GetMethod(
                    "SetFormData", BindingFlags.Public | BindingFlags.Instance,
                    [typeof(string), typeof(IReadOnlyDictionary<string, Microsoft.Extensions.Primitives.StringValues>), typeof(IFormFileCollection)])
                    ?? throw new MissingMethodException(ProviderTypeName, "SetFormData(string, IReadOnlyDictionary<string, StringValues>, IFormFileCollection)");

                if (!typeof(IReadOnlyDictionary<string, Microsoft.Extensions.Primitives.StringValues>).IsAssignableFrom(dictionaryType))
                    throw new InvalidCastException($"{DictionaryTypeName} no longer implements IReadOnlyDictionary<string, StringValues>.");

                return new Accessor(providerType, constructor, setFormData);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                throw new InvalidOperationException(
                    "Rizzy could not hook into Blazor form mapping for this version of ASP.NET Core " +
                    $"({typeof(HttpContext).Assembly.GetName().Version}). [SupplyParameterFromForm] cannot be populated " +
                    "for components rendered by Rizzy views. Please report this at https://github.com/JalexSocial/Rizzy/issues.",
                    ex);
            }
        }

        public void SetFormData(HttpContext context, string handler, IFormCollection form)
        {
            var provider = context.RequestServices.GetService(_providerType)
                ?? throw new InvalidOperationException(
                    "Blazor form mapping services are not registered. Call builder.Services.AddRazorComponents() " +
                    "before AddRizzy().");

            var entries = _dictionaryConstructor.Invoke([form]);
            _setFormData.Invoke(provider, [handler, entries, form.Files]);
        }
    }
}
