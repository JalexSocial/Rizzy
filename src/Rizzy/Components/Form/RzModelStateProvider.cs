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
                messages.Add(ResolveField(EditContext, error.Item1), error.Item2);
            }

            EditContext.NotifyValidationStateChanged();
        }
    }

    /// <summary>
    /// Maps a model state key to the field Blazor's validation components look for. MVC keys carry
    /// the binding prefix and the property path (<c>Person.Address.City</c> for a parameter named
    /// <c>person</c>), while Blazor identifies a field by its owning object and property name
    /// (<c>(address, "City")</c>). A leading segment that is not a property of the form model is
    /// treated as the binding prefix. Keys the path walk cannot follow (indexers, null objects)
    /// fall back to the raw key.
    /// </summary>
    internal static FieldIdentifier ResolveField(EditContext editContext, string key)
    {
        if (string.IsNullOrEmpty(key) || key.Contains('['))
            return editContext.Field(key);

        var segments = key.Split('.');
        object model = editContext.Model;

        var start = 0;
        if (segments.Length > 1 && model.GetType().GetProperty(segments[0]) is null)
            start = 1;

        for (var i = start; i < segments.Length - 1; i++)
        {
            var next = model.GetType().GetProperty(segments[i])?.GetValue(model);
            if (next is null)
                return editContext.Field(key);
            model = next;
        }

        return new FieldIdentifier(model, segments[^1]);
    }
}
