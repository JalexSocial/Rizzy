using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Rizzy;

/// <summary>
/// A validator component that ensures model state errors are added to the EditContext.
/// </summary>
public class RzInitialValidator : ComponentBase
{
    /// <summary>
    /// Gets or sets the EditContext for this component.
    /// </summary>
    [CascadingParameter]
    public EditContext? EditContext { get; set; }

    /// <summary>
    /// Invoked when the component has received parameters from its parent.
    /// </summary>
    protected override void OnParametersSet()
    {
        EditContext?.Validate();
    }
}
