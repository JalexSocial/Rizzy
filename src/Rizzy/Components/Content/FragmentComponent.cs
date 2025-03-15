using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Rizzy;

/// <summary>
/// Component that encapsulates one or more <see cref="RenderFragment"/> instances for use as partial or child components.
/// </summary>
public class FragmentComponent : ComponentBase
{
    /// <summary>
    /// A single <see cref="RenderFragment"/> to be rendered.
    /// </summary>
    [Parameter]
    public RenderFragment? Fragment { get; set; }

    /// <summary>
    /// A collection of <see cref="RenderFragment"/> items to be rendered in sequence.
    /// </summary>
    [Parameter]
    public IEnumerable<RenderFragment?>? Fragments { get; set; }

    /// <summary>
    /// Builds the render tree by rendering <see cref="Fragment"/> and each item in <see cref="Fragments"/>, if present.
    /// </summary>
    /// <param name="builder">A utility class used to build the component's render tree.</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Fragment != null)
        {
            builder.AddContent(0, Fragment);
        }

        if (Fragments != null)
        {
            foreach (var fragment in Fragments)
            {
                if (fragment != null)
                {
                    builder.AddContent(1, fragment);
                }
            }
        }
    }
}

