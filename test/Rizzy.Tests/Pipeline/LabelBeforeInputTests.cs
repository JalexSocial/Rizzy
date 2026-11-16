using System.Text.RegularExpressions;

namespace Rizzy.Tests.Pipeline;

/// <summary>
/// A label that renders before its input reserves the input's id through
/// <see cref="HttpContextExtensions.GetOrReserveFieldId"/>; every Rizzy input adopts it.
/// </summary>
public class LabelBeforeInputTests(RizzyTestHost host) : IClassFixture<RizzyTestHost>
{
    private async Task<string> RenderAsync()
        => await (await host.GetAsync("/form-label-first")).Content.ReadAsStringAsync();

    private static List<string> LabelsFor(string html, string field)
        => Regex.Matches(html, $"<label for=\"([^\"]*)\" data-label-for=\"{field}\"")
            .Select(m => m.Groups[1].Value).ToList();

    private static string IdOf(string html, string name)
    {
        var tag = Regex.Match(html, $"<(input|textarea|select)[^>]*name=\"Person\\.{name}\"[^>]*>");
        tag.Success.Should().BeTrue($"an input named Person.{name} should be rendered");
        return Regex.Match(tag.Value, "\\bid=\"([^\"]+)\"").Groups[1].Value;
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("Age")]
    [InlineData("Subscribe")]
    [InlineData("BirthDate")]
    [InlineData("Bio")]
    [InlineData("Token")]
    [InlineData("Color")]
    public async Task A_label_before_the_input_points_at_it(string field)
    {
        var html = await RenderAsync();

        var id = IdOf(html, field);
        id.Should().NotBeNullOrEmpty();
        LabelsFor(html, field).Should().Equal(id);
        Regex.Matches(html, $"\\bid=\"{Regex.Escape(id)}\"").Count.Should().Be(1, "the id must be unique in the page");
    }

    [Fact]
    public async Task A_validation_message_before_the_input_still_uses_the_field_name()
    {
        var html = await RenderAsync();

        html.Should().Contain("data-valmsg-for=\"Person.Name\"");
        html.Should().NotContain("data-valmsg-for=\"\"");
    }

    [Fact]
    public async Task An_explicit_id_is_kept_and_labels_after_the_input_point_at_it()
    {
        var html = await RenderAsync();

        IdOf(html, "Email").Should().Be("explicit-email");
        var labels = LabelsFor(html, "Email");
        labels.Should().HaveCount(2);
        // The first label rendered before the input and keeps the id it reserved; use ForId for it.
        labels[1].Should().Be("explicit-email");
    }
}
