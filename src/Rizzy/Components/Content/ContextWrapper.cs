using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rizzy.Components.Content;

internal static class ContextWrapper
{
	public static void CreateCascadingValue<TValue>(RenderTreeBuilder builder, TValue value, RenderFragment fragment)
	{
		builder.OpenComponent<CascadingValue<TValue>>(0);
		builder.AddComponentParameter(1, "Value", value);
		builder.AddComponentParameter(2, "ChildContent", fragment);
		builder.CloseComponent();
	}
}
