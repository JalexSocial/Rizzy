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

#if NET11_0_OR_GREATER
    /// <summary>
    /// Validates the form when the component receives its parameters, including asynchronous
    /// validators (<c>AsyncValidationAttribute</c>, <c>IAsyncValidatableObject</c>; .NET 11).
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        if (EditContext is not null)
            await EditContext.ValidateAsync();
    }
#else
    /// <summary>
    /// Validates the form when the component receives its parameters.
    /// </summary>
    protected override void OnParametersSet()
    {
        EditContext?.Validate();
    }
#endif
}