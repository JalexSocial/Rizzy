using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form.Helpers;

namespace Rizzy.Components.Form;

/// <summary>
/// A dropdown selection component.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class RzInputSelect<TValue> : InputSelect<TValue>
{
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    [CascadingParameter]
    private Dictionary<FieldIdentifier, string>? FieldMapping { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (FieldMapping is null)
            throw new InvalidOperationException($"{nameof(RzInputSelect<TValue>)} must be enclosed within an {nameof(RzEditForm)}.");

        // No validation
    }
}