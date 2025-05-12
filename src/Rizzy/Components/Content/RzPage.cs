using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Rizzy.Configuration;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Rizzy.State;

namespace Rizzy;

/// <summary>
/// Page component container
/// </summary>
[RizzyParameterize] 
public partial class RzPage : ComponentBase
{
    /// <summary>
    /// Caches layout attributes discovered on component types for faster lookups.
    /// </summary>
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type?> LayoutAttributeCache = new();

    /// <summary>
    /// The layout to be used if one is present. If not set, uses <see cref="RizzyConfig.DefaultLayout"/>.
    /// </summary>
    private Type? _layout = null;

    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }
    
    /// <summary>
    /// Provides the application's Rizzy configuration, including default layout and root component settings.
    /// </summary>
    [Inject]
    public IOptions<RizzyConfig> RizzyConfig { get; set; } = default!;

    /// <summary>
    /// Creates a cascading value around a specified render fragment.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to cascade.</typeparam>
    /// <param name="builder">The render tree builder to use.</param>
    /// <param name="value">The value to cascade.</param>
    /// <param name="fragment">The fragment to render inside the cascading value.</param>
    public static void CreateCascadingValue<TValue>(RenderTreeBuilder builder, TValue value, RenderFragment fragment)
    {
        builder.OpenComponent<CascadingValue<TValue>>(0);
        builder.AddComponentParameter(1, "Value", value);
        builder.AddComponentParameter(2, "ChildContent", fragment);
        builder.CloseComponent();
    }

    /// <summary>
    /// Builds the render tree for this page. Applies the root component and optional layout to the nested content.
    /// </summary>
    /// <param name="builder">The render tree builder used to construct the output.</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (HttpContext?.Items.TryGetValue(RizzyStateConstants.HttpContextItems.StateForView, out var tokenObject) == true &&
            tokenObject is string stateToken && !string.IsNullOrEmpty(stateToken))
        {
            // Render the script tag for initial state hydration
            builder.OpenElement(0, "script");
            builder.AddAttribute(1, "id", RizzyStateConstants.RzStateScriptTagId);
            builder.AddAttribute(2, "type", "application/json");
            // Nonce: application/json script tags are generally not considered executable by CSP script-src
            // so they don't strictly need a nonce for that purpose.
            // If you still want to add it for other reasons or stricter policies:
            // var nonce = HttpContext.GetNonce(); // Assuming GetNonce() extension method
            // if (!string.IsNullOrEmpty(nonce)) builder.AddAttribute(3, "nonce", nonce);
            builder.AddContent(3, (MarkupString)stateToken); // Render token as raw content
            builder.CloseElement(); // script
        }
        
        CreateCascadingValue(builder, ModelState, builderPage =>
        {
            builderPage.OpenComponent(4, RizzyConfig.Value.RootComponent ?? typeof(EmptyRootComponent));
            builderPage.AddAttribute(5, "ChildContent", (RenderFragment)(builder3 =>
            {
                if (_layout != null)
                {
                    builder3.OpenComponent<LayoutView>(6);
                    builder3.AddComponentParameter(7, "Layout", RuntimeHelpers.TypeCheck<System.Type>(_layout));
                    builder3.AddAttribute(8, "ChildContent", (RenderFragment)((builder4) =>
                    {
                        builder4.OpenComponent<DynamicComponent>(9);
                        builder4.AddComponentParameter(10, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
                        builder4.AddComponentParameter(11, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
                        builder4.CloseComponent();
                    }));
                    builder3.CloseComponent();
                }
                else
                {
                    builder3.OpenComponent<DynamicComponent>(12);
                    builder3.AddComponentParameter(13, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
                    builder3.AddComponentParameter(14, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
                    builder3.CloseComponent();
                }
            }));
            builderPage.OpenComponent<HtmxSwapContent>(15);
            builderPage.CloseComponent();
            builderPage.CloseComponent();
        });
    }

    /// <summary>
    /// The type of the Razor component to dynamically render.
    /// </summary>
    [Parameter, EditorRequired]
    public required Type ComponentType { get; set; } = default!;

    /// <summary>
    /// A dictionary of parameter names and values for the <see cref="ComponentType"/>.
    /// </summary>
    [Parameter, EditorRequired]
    public required Dictionary<string, object?> ComponentParameters { get; set; } = default!;

    /// <summary>
    /// Optional ModelState provided by MVC
    /// </summary>
    [Parameter]
    public ModelStateDictionary? ModelState { get; set; }

    /// <summary>
    /// Obtains layout information from the component's <see cref="LayoutAttribute"/> if present,
    /// falling back to the <see cref="RizzyConfig.DefaultLayout" /> if not.
    /// </summary>
    protected override void OnParametersSet()
    {
        if (!LayoutAttributeCache.TryGetValue(ComponentType, out _layout))
        {
            _layout = ComponentType.GetCustomAttribute<LayoutAttribute>()?.LayoutType;
            LayoutAttributeCache.TryAdd(ComponentType, _layout);
        }

        if (_layout == null)
        {
            var config = RizzyConfig;

            if (config?.Value.DefaultLayout != null)
                _layout = config?.Value.DefaultLayout;
        }
    }
}

