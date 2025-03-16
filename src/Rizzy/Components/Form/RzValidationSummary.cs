using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Rizzy;

/// <summary>
/// A custom validation summary component that displays validation messages.
/// </summary>
public class RzValidationSummary : ValidationSummary
{
    /// <summary>
    /// Gets or sets the <see cref="EditContext"/> for the form.
    /// </summary>
    [CascadingParameter] EditContext EditContext { get; set; } = default!;

    /// <summary>
    /// Builds the render tree for the component.
    /// </summary>
    /// <param name="builder">The <see cref="RenderTreeBuilder"/> used to build the render tree.</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // As an optimization, only evaluate the messages enumerable once, and
        // only produce the enclosing <ul> if there's at least one message
        var validationMessages = Model is null ?
            EditContext.GetValidationMessages().ToList() :
            EditContext.GetValidationMessages(new FieldIdentifier(Model, string.Empty)).ToList();

        if (validationMessages.Count > 0)
        {
            var first = true;
            foreach (var error in validationMessages)
            {
                if (first)
                {
                    first = false;

                    builder.OpenElement(0, "ul");
                    builder.AddAttribute(1, "class", "validation-errors");
                    builder.AddMultipleAttributes(2, AdditionalAttributes);
                }

                builder.OpenElement(3, "li");
                builder.AddAttribute(4, "class", "validation-message");
                builder.AddContent(5, error);
                builder.CloseElement();
            }

            if (!first)
            {
                // We have at least one validation message.
                builder.CloseElement();
            }
        }
    }
}
