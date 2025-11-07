using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Rizzy;

/// <summary>
/// A component that transfer MVC model state errors to the Razor component
/// </summary>
public class RzModelStateProvider : ComponentBase
{
    /// <summary>
    /// Gets or sets the EditContext for this component.
    /// </summary>
    [CascadingParameter]
    public EditContext? EditContext { get; set; }

    /// <summary>
    /// Gets or sets the ModelStateDictionary for this component.
    /// </summary>
    [CascadingParameter]
    public ModelStateDictionary? ModelState { get; set; }

    /// <summary>
    /// Invoked when the component has received parameters from its parent.
    /// </summary>
    protected override void OnParametersSet()
    {
        if (ModelState != null && EditContext != null && !ModelState.IsValid)
        {
            var messages = new ValidationMessageStore(EditContext);

            foreach (var error in ModelState.SelectMany(s => s.Value?.Errors.Select(e => (s.Key, e.ErrorMessage)) ?? Enumerable.Empty<(string, string)>()))
            {
                messages.Add(EditContext.Field(error.Item1), error.Item2);
            }

            EditContext.NotifyValidationStateChanged();
        }
    }
}