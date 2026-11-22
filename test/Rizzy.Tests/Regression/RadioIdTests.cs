using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Rizzy.Tests.Regression;

/// <summary>RizzyUI Summit SF-047: a radio without an Id rendered id="", invalid and unreferenceable.</summary>
public class RadioIdTests : BunitContext
{
    private sealed class Choice { public string? Value { get; set; } }

    private IRenderedComponent<CascadingValue<EditContext>> RenderRadio(string? id)
    {
        var model = new Choice();
        RenderFragment radios = builder =>
        {
            builder.OpenComponent<InputRadioGroup<string>>(0);
            builder.AddComponentParameter(1, "Value", model.Value);
            builder.AddComponentParameter(2, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Value));
            builder.AddComponentParameter(3, "ChildContent", (RenderFragment)(radio =>
            {
                radio.OpenComponent<RzInputRadioBase<string>>(0);
                radio.AddComponentParameter(1, "Value", "a");
                if (id is not null) radio.AddComponentParameter(2, "Id", id);
                radio.CloseComponent();
            }));
            builder.CloseComponent();
        };

        return Render<CascadingValue<EditContext>>(p => p
            .Add(x => x.Value, new EditContext(model))
            .Add(x => x.ChildContent, radios));
    }

    [Fact]
    public void A_radio_without_an_id_renders_no_id_attribute()
    {
        var input = RenderRadio(null).Find("input[type=radio]");
        input.HasAttribute("id").Should().BeFalse();
    }

    [Fact]
    public void A_radio_with_an_id_renders_it()
    {
        RenderRadio("choice-a").Find("input[type=radio]").GetAttribute("id").Should().Be("choice-a");
    }
}
