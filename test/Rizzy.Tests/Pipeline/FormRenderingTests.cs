using System.Text.RegularExpressions;

namespace Rizzy.Tests.Pipeline;

/// <summary>
/// Renders a form built from every Rizzy input component through the real pipeline and checks
/// the client-validation attributes of each field (RIZZY-008, RIZZY-025).
/// </summary>
public class FormRenderingTests(RizzyTestHost host) : IClassFixture<RizzyTestHost>
{
    private async Task<string> RenderFormAsync(string url = "/form")
        => await (await host.GetAsync(url)).Content.ReadAsStringAsync();

    private static string InputFor(string html, string name)
    {
        var match = Regex.Match(html, $"<(input|textarea|select)[^>]*name=\"Person\\.{name}\"[^>]*>");
        match.Success.Should().BeTrue($"an input named Person.{name} should be rendered");
        return match.Value;
    }

    [Fact]
    public async Task Each_field_gets_only_its_own_validation_attributes()
    {
        var html = await RenderFormAsync();

        var name = InputFor(html, "Name");
        name.Should().Contain("data-val-required=\"The Name field is required.\"");
        name.Should().NotContain("data-val-email");

        var email = InputFor(html, "Email");
        email.Should().Contain("data-val-email=");
        email.Should().NotContain("data-val-required", "Email has no [Required] attribute (RIZZY-008)");

        var age = InputFor(html, "Age");
        age.Should().Contain("data-val-range-min=\"1\"").And.Contain("data-val-range-max=\"120\"");

        var bio = InputFor(html, "Bio");
        bio.Should().Contain("data-val-length-max=\"200\"").And.Contain("data-val-length-min=\"2\"");
    }

    [Fact]
    public async Task Every_input_component_renders_with_an_id_and_value()
    {
        var html = await RenderFormAsync();

        InputFor(html, "Name").Should().MatchRegex("id=\"[^\"]+\"");
        InputFor(html, "Age").Should().Contain("value=\"30\"");
        InputFor(html, "Subscribe").Should().Contain("type=\"checkbox\"");
        InputFor(html, "BirthDate").Should().Contain("value=\"2000-01-02\"");
        InputFor(html, "Token").Should().Contain("type=\"hidden\"").And.Contain("value=\"t1\"");
        InputFor(html, "Color").Should().StartWith("<select");
        html.Should().Contain("<option value=\"red\">Red</option>");
    }

    [Fact]
    public async Task Validation_messages_point_at_their_fields()
    {
        var html = await RenderFormAsync();

        html.Should().Contain("data-valmsg-for=\"Person.Name\"");
        html.Should().Contain("data-valmsg-for=\"Person.Email\"");
    }

    [Fact]
    public async Task Mvc_model_state_errors_are_rendered_in_the_form()
    {
        var html = await RenderFormAsync("/form-invalid");

        html.Should().Contain("Server says the name is taken.");
    }

    [Fact]
    public async Task Model_state_keys_with_the_binding_prefix_reach_the_field()
    {
        var html = await RenderFormAsync("/form-invalid-prefixed");

        // The field's own message element (not just the summary) shows the error, and the input
        // is marked invalid.
        Regex.IsMatch(html, @"<div[^>]*data-valmsg-for=""Person\.Name""[^>]*>Prefixed: the name is taken\.</div>")
            .Should().BeTrue("the error should be shown next to the Name field");
        InputFor(html, "Name").Should().Contain("invalid");
    }

    [Fact]
    public async Task Controller_views_receive_the_controller_ModelState_by_default()
    {
        var html = await RenderFormAsync("/ctrl/invalid");

        Regex.IsMatch(html, @"<div[^>]*data-valmsg-for=""Person\.Name""[^>]*>Controller says the name is taken\.</div>")
            .Should().BeTrue("RzController passes its ModelState when none is given");
    }
}
