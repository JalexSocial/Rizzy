using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form.Helpers;

namespace Rizzy.Components;

/// <summary>
/// An input component for editing Boolean values.
/// </summary>
public class RzInputCheckbox : InputCheckbox
{
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    [CascadingParameter]
    private Dictionary<FieldIdentifier, string>? FieldMapping { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (FieldMapping is null)
            throw new InvalidOperationException($"{nameof(RzInputCheckbox)} must be enclosed within an {nameof(RzEditForm)}.");

        // No validation
    }
}