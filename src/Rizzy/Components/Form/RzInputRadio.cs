using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form.Helpers;

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
    private Dictionary<FieldIdentifier, string>? FieldMapping { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (FieldMapping is null)
            throw new InvalidOperationException($"{nameof(RzInputRadio<TValue>)} must be enclosed within an {nameof(RzEditForm)}.");

        // No validation
    }
}