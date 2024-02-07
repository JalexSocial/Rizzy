using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form.Helpers;

namespace Rizzy.Components.Form;

/// <summary>
/// An input component for editing numeric values. Supported numeric types are
/// Int32, Int64, Int16, Single, Double, Decimal.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class RzInputNumber<TValue> : InputNumber<TValue>
{
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    [CascadingParameter]
    private Dictionary<FieldIdentifier, string>? FieldMapping { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (FieldMapping is null)
            throw new InvalidOperationException($"{nameof(RzInputNumber<TValue>)} must be enclosed within an {nameof(RzEditForm)}.");

        FieldMapping.TryAdd(FieldIdentifier, NameAttributeValue);

        AdditionalAttributes = DataAnnotationsProcessor.MergeAttributes(nameof(RzInputNumber<TValue>), ValueExpression, AdditionalAttributes);
    }
}