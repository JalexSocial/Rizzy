using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Rizzy.Components;

/// <summary>
/// Component that encapsulates a RenderFragment for use as a partial or child component.
/// </summary>
public class FragmentComponent : ComponentBase
{
	[Parameter]
	public RenderFragment? Fragment { get; set; }

    [Parameter]
	public IEnumerable<RenderFragment?>? Fragments { get; set; }

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

