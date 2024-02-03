using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rizzy.Components.Layout;

public class HtmxApp<T> : ComponentBase where T : LayoutComponentBase
{
	[CascadingParameter]
	public HttpContext? Context { get; set; }

	[Parameter]
	public RenderFragment ChildContent { get; set; } = default!;

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenComponent<HtmxLayout<T>>(0);
		builder.AddAttribute(1, "Body", (RenderFragment)(builder2 =>
		{
			builder2.AddContent(2, ChildContent);
		}));
		builder.CloseComponent();
	}
}
