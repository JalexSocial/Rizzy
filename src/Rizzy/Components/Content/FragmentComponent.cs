using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Rizzy.Components;


public class FragmentComponent : ComponentBase
{
	[Parameter]
	public RenderFragment? Fragment { get; set; }

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		if (Fragment != null)
		{
			builder.AddContent(0, Fragment);
		}
	}
}

