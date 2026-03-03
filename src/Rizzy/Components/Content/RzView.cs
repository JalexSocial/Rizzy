using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Rizzy.Configuration;
using System.Collections.Concurrent;
using System.Reflection;

namespace Rizzy;

/// <summary>
/// Specifies how an <see cref="RzView"/> should render its content.
/// </summary>
internal enum RzViewMode
{
    /// <summary>
    /// Renders the component as a full page. This applies the configured root component
    /// and resolves layouts automatically from the target component's <see cref="LayoutAttribute"/>
    /// or the configured default layout.
    /// </summary>
    Page,

    /// <summary>
    /// Renders the component as a partial view. This bypasses the configured root component
    /// and skips automatic layout resolution, but still renders through <see cref="EmptyLayout"/>
    /// to preserve partial-view composition behavior.
    /// </summary>
    Partial
}

/// <summary>
/// Renders a specified Razor component dynamically while handling optional page composition,
/// MVC model state cascading, and HTMX out-of-band swap content.
/// </summary>
internal sealed class RzView : ComponentBase
{
    /// <summary>
    /// Caches layout types discovered from <see cref="LayoutAttribute"/> to avoid repeated reflection.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, Type?> LayoutAttributeCache = new();

    /// <summary>
    /// The layout resolved for the current render cycle.
    /// </summary>
    private Type? _resolvedLayout;

    /// <summary>
    /// Provides Rizzy framework configuration, including the default layout and root component.
    /// </summary>
    [Inject]
    public IOptions<RizzyConfig> RizzyConfig { get; set; } = default!;

    /// <summary>
    /// Gets or sets the type of the Razor component to render.
    /// </summary>
    [Parameter, EditorRequired]
    public required Type ComponentType { get; set; } = default!;

    /// <summary>
    /// Gets or sets the parameters to pass to the dynamically rendered component.
    /// </summary>
    [Parameter, EditorRequired]
    public required IDictionary<string, object?> ComponentParameters { get; set; } = default!;

    /// <summary>
    /// Gets or sets the optional MVC model state dictionary to cascade through the component tree.
    /// </summary>
    [Parameter]
    public ModelStateDictionary? ModelState { get; set; }

    /// <summary>
    /// Gets or sets how the view should be rendered. Defaults to <see cref="RzViewMode.Page"/>.
    /// When set to <see cref="RzViewMode.Partial"/>, the root component is bypassed and
    /// <see cref="EmptyLayout"/> is used instead of automatic layout resolution.
    /// </summary>
    [Parameter]
    public RzViewMode Mode { get; set; } = RzViewMode.Page;

    /// <summary>
    /// Creates a <see cref="CascadingValue{TValue}"/> around the supplied child fragment.
    /// </summary>
    /// <typeparam name="TValue">The type of the cascaded value.</typeparam>
    /// <param name="builder">The render tree builder.</param>
    /// <param name="value">The value to cascade.</param>
    /// <param name="fragment">The child content that receives the cascaded value.</param>
    private static void CreateCascadingValue<TValue>(RenderTreeBuilder builder, TValue value, RenderFragment fragment)
    {
        builder.OpenComponent<CascadingValue<TValue>>(0);
        builder.AddComponentParameter(1, nameof(CascadingValue<TValue>.Value), value);
        builder.AddComponentParameter(2, nameof(CascadingValue<TValue>.ChildContent), fragment);
        builder.CloseComponent();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        _resolvedLayout = Mode == RzViewMode.Partial
            ? typeof(EmptyLayout)
            : ResolveAutoLayout();
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        CreateCascadingValue(builder, ModelState, bodyBuilder =>
        {
            RenderFragment content = contentBuilder =>
            {
                RenderBody(contentBuilder);

                contentBuilder.OpenComponent<HtmxSwapContent>(100);
                contentBuilder.CloseComponent();
            };

            if (Mode == RzViewMode.Page)
            {
                // Inject htmx configuration and antiforgery token scripts into the head, then render the root component which will wrap the entire page content.
                bodyBuilder.OpenComponent<Microsoft.AspNetCore.Components.Web.HeadContent>(1);
                bodyBuilder.AddAttribute(2, "ChildContent", (RenderFragment)(headBuilder => {
                    headBuilder.OpenComponent<HtmxConfigHeadOutlet>(3);
                    headBuilder.CloseComponent();
                }));
                bodyBuilder.CloseComponent();

                // Render the root component, which will wrap the content. This allows for full-page composition and ensures that HTMX scripts are included in the correct order.
                bodyBuilder.OpenComponent(5, RizzyConfig?.Value.RootComponent ?? typeof(EmptyRootComponent));
                bodyBuilder.AddAttribute(6, "ChildContent", content);
                bodyBuilder.CloseComponent();
            }
            else
            {
                content(bodyBuilder);
            }
        });
    }

    /// <summary>
    /// Renders the view body, wrapping the target component in a <see cref="LayoutView"/>
    /// when a layout has been resolved.
    /// </summary>
    /// <param name="builder">The render tree builder.</param>
    private void RenderBody(RenderTreeBuilder builder)
    {
        if (_resolvedLayout is not null)
        {
            builder.OpenComponent<LayoutView>(10);
            builder.AddComponentParameter(11, nameof(LayoutView.Layout), RuntimeHelpers.TypeCheck<Type>(_resolvedLayout));
            builder.AddAttribute(12, nameof(LayoutView.ChildContent), (RenderFragment)RenderDynamicComponent);
            builder.CloseComponent();
            return;
        }

        RenderDynamicComponent(builder);
    }

    /// <summary>
    /// Renders the target component using <see cref="DynamicComponent"/>.
    /// </summary>
    /// <param name="builder">The render tree builder.</param>
    private void RenderDynamicComponent(RenderTreeBuilder builder)
    {
        builder.OpenComponent<DynamicComponent>(20);
        builder.AddComponentParameter(21, nameof(DynamicComponent.Type), RuntimeHelpers.TypeCheck<Type>(ComponentType));
        builder.AddComponentParameter(22, nameof(DynamicComponent.Parameters), RuntimeHelpers.TypeCheck<IDictionary<string, object?>>(ComponentParameters));
        builder.CloseComponent();
    }

    /// <summary>
    /// Resolves the layout for the current component by checking for a <see cref="LayoutAttribute"/>
    /// and falling back to the configured default layout when no attribute is present.
    /// </summary>
    /// <returns>
    /// The resolved layout type, or <see langword="null"/> when no layout applies.
    /// </returns>
    private Type? ResolveAutoLayout()
    {
        if (!LayoutAttributeCache.TryGetValue(ComponentType, out var layout))
        {
            layout = ComponentType.GetCustomAttribute<LayoutAttribute>()?.LayoutType;
            LayoutAttributeCache.TryAdd(ComponentType, layout);
        }

        return layout ?? RizzyConfig.Value.DefaultLayout;
    }
}