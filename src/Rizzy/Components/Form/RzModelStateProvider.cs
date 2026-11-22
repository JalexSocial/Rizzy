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
    /// treated as the binding prefix. Indexers walk lists, arrays and dictionaries
    /// (<c>Attendees[1].Name</c>, <c>Groups[0][2].Name</c>, <c>ByCode[vip].Name</c>). Keys the walk
    /// cannot follow (a missing property, an index out of range, a null object, a key that ends with
    /// an indexer) fall back to the raw key.
    /// </summary>
    internal static FieldIdentifier ResolveField(EditContext editContext, string key)
    {
        if (string.IsNullOrEmpty(key))
            return editContext.Field(key);

        var steps = ParsePath(key);
        if (steps is null || steps.Count == 0 || steps[^1].IsIndex)
            return editContext.Field(key);

        object model = editContext.Model;

        // "Person.Name" for a parameter named person: the first segment is the binding prefix when it
        // is followed by a property segment and is not itself a property of the model.
        var start = 0;
        if (steps.Count > 1 && !steps[0].IsIndex && !steps[1].IsIndex
            && model.GetType().GetProperty(steps[0].Value) is null)
            start = 1;

        for (var i = start; i < steps.Count - 1; i++)
        {
            var next = steps[i].IsIndex ? Index(model, steps[i].Value) : model.GetType().GetProperty(steps[i].Value)?.GetValue(model);
            if (next is null)
                return editContext.Field(key);
            model = next;
        }

        return new FieldIdentifier(model, steps[^1].Value);
    }

    private readonly record struct PathStep(string Value, bool IsIndex);

    /// <summary>Splits <c>A.B[1][x].C</c> into A, B, [1], [x], C; null when the key is malformed.</summary>
    private static List<PathStep>? ParsePath(string key)
    {
        var steps = new List<PathStep>();
        var i = 0;
        while (i < key.Length)
        {
            if (key[i] == '.')
            {
                i++;
                continue;
            }

            if (key[i] == '[')
            {
                var close = key.IndexOf(']', i + 1);
                if (close < 0) return null;
                steps.Add(new PathStep(key[(i + 1)..close], true));
                i = close + 1;
                continue;
            }

            var end = key.IndexOfAny(['.', '['], i);
            if (end < 0) end = key.Length;
            steps.Add(new PathStep(key[i..end], false));
            i = end;
        }

        return steps;
    }

    /// <summary>The element at <paramref name="index"/> of a list, array or dictionary, or null.</summary>
    private static object? Index(object collection, string index)
    {
        switch (collection)
        {
            case System.Collections.IList list when int.TryParse(index, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var position):
                return position < list.Count ? list[position] : null;
            case System.Collections.IDictionary dictionary:
                foreach (System.Collections.DictionaryEntry entry in dictionary)
                {
                    if (string.Equals(Convert.ToString(entry.Key, System.Globalization.CultureInfo.InvariantCulture), index, StringComparison.Ordinal))
                        return entry.Value;
                }
                return null;
            case System.Collections.IEnumerable sequence when int.TryParse(index, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var position):
                return sequence.Cast<object?>().Skip(position).FirstOrDefault();
            default:
                return null;
        }
    }
}
