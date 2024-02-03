using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rizzy.Components.Layout;

internal class EmptyRootComponent : ComponentBase
{
	[Parameter]
	public RenderFragment ChildContent { get; set; } = default!;

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, "html");
		builder.OpenElement(1, "body");
		builder.AddContent(2, ChildContent);
		builder.CloseElement();
		builder.CloseElement();
	}
}
