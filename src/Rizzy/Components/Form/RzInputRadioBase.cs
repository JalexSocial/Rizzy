using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.ObjectModel;

namespace Rizzy;

/// <summary>
/// An input component used for selecting a value from a group of choices.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
[RizzyParameterize] 
public partial class RzInputRadioBase<TValue> : InputRadio<TValue>
{
    /// <summary>
    /// Gets or sets the Id of the input radio.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Method invoked when the component has received parameters from its parent in the render tree.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
        attrib.TryAdd("id", Id);

        AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
    }
}
