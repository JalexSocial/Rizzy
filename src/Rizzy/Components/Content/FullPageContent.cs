using Microsoft.AspNetCore.Components;

namespace Rizzy.Components.Content;

public class FullPageContent : IComponent
{
    private RenderHandle renderHandle;

    [CascadingParameter] private HtmxRequestViewContext ViewContext { get; set; } = default!;

    [Parameter, EditorRequired] public required RenderFragment ChildContent { get; set; }

    public void Attach(RenderHandle renderHandle) => this.renderHandle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        if (!ViewContext.Context.Request.IsHtmx)
        {
            renderHandle.Render(ChildContent);
        }
        return Task.CompletedTask;
    }
}
