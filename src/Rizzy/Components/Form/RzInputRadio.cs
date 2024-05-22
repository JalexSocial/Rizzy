using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form;
using Rizzy.Components.Form.Helpers;
using System.Collections.ObjectModel;

namespace Rizzy.Components;

/// <summary>
/// An input component used for selecting a value from a group of choices.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class RzInputRadio<TValue> : InputRadio<TValue>
{
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    [CascadingParameter]
    private RzEditForm? EditForm { get; set; }

    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditForm is null)
            throw new InvalidOperationException($"{nameof(RzInputRadio<TValue>)} must be enclosed within an {nameof(RzEditForm)}.");

        // No validation

        var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
        attrib.TryAdd("id", Id);

        AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
    }
}