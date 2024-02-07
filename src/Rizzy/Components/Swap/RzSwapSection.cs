using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rizzy.Components.Swap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;
using Rizzy.Components.Swap.Services;

public class RzSwapSection : ComponentBase
{
    [Inject] public IHtmxSwapService HtmxSwapService { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);

        builder.OpenComponent<SectionOutlet>(0);
        builder.AddAttribute(1, "SectionName", "htmx-swap");
        builder.CloseComponent();

        builder.OpenComponent<SectionContent>(2);
        builder.AddAttribute(3, "SectionName", "htmx-swap");

        builder.AddAttribute(4, "ChildContent", HtmxSwapService.RenderToFragment());
        builder.CloseComponent();
    }
}

