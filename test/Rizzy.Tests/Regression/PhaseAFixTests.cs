using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Rizzy.Tests.Regression;

/// <summary>Regression tests for the Phase A fixes (RIZZY-002, RIZZY-008, RIZZY-009).</summary>
public class PhaseAFixTests
{
    // ---- RIZZY-002: SerializeAsAlpineData must not allow breaking out of a string -------------

    [Theory]
    [InlineData("\\', x: alert(1)}//")]
    [InlineData("'); alert(1); //")]
    [InlineData("\"}, x: alert(1), y: {\"")]
    [InlineData("line1\nline2\rline3")]
    [InlineData("</script><script>alert(1)</script>")]
    public void SerializeAsAlpineData_round_trips_hostile_strings_as_a_single_value(string hostile)
    {
        var json = new { name = hostile, other = 1 }.SerializeAsAlpineData();

        // The output must be one JSON object whose "name" is exactly the input string
        // and which has no extra (injected) properties.
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.EnumerateObject().Select(p => p.Name).Should().Equal("name", "other");
        doc.RootElement.GetProperty("name").GetString().Should().Be(hostile);
    }

    [Fact]
    public void SerializeAsAlpineData_escapes_html_sensitive_and_line_separator_characters()
    {
        var lineSeparator = ((char)0x2028).ToString();
        var json = new { name = "<a href='x'>&" + lineSeparator }.SerializeAsAlpineData();

        json.Should().NotContain("<").And.NotContain(">").And.NotContain("'").And.NotContain("&");
        json.Should().NotContain(lineSeparator);
        json.Should().NotContain("\n");
    }

    [Fact]
    public void SerializeAsAlpineData_can_ignore_nulls()
    {
        new { a = (string?)null, b = 1 }.SerializeAsAlpineData(ignoreNullValues: true).Should().Be("{\"b\":1}");
        new { a = (string?)null, b = 1 }.SerializeAsAlpineData().Should().Be("{\"a\":null,\"b\":1}");
        ((object?)null).SerializeAsAlpineData().Should().BeEmpty();
    }

    // ---- RIZZY-008: validation metadata is cached per field, not per model type ---------------

    private sealed class Model
    {
        [Required] public string? Name { get; set; }
        [EmailAddress] public string? Email { get; set; }
        [Range(1, 10)] public int Count { get; set; }
    }

    private class BaseModel
    {
        [Required] public string? Title { get; set; }
    }

    private sealed class DerivedModel : BaseModel
    {
        [StringLength(5)] public new string? Title { get; set; }
    }

    private static DataAnnotationsProcessor CreateProcessor()
        => new(new ServiceCollection().AddLogging().BuildServiceProvider());

    [Fact]
    public void MergeAttributes_returns_each_fields_own_rules_regardless_of_order()
    {
        var processor = CreateProcessor();
        var model = new Model();

        // Render the email field first, then name, then count.
        var email = processor.MergeAttributes("test", () => model.Email, null)!;
        var name = processor.MergeAttributes("test", () => model.Name, null)!;
        var count = processor.MergeAttributes("test", () => model.Count, null)!;

        email.Should().ContainKey("data-val-email").And.NotContainKey("data-val-required");
        name.Should().ContainKey("data-val-required").And.NotContainKey("data-val-email");
        count.Should().ContainKey("data-val-range").And.NotContainKey("data-val-required");
    }

    [Fact]
    public void MergeAttributes_uses_the_most_derived_property_when_a_property_is_hidden()
    {
        var processor = CreateProcessor();
        var model = new DerivedModel();

        var title = processor.MergeAttributes("test", () => model.Title, null)!;

        title.Should().ContainKey("data-val-length").And.NotContainKey("data-val-required");
    }

    // ---- RIZZY-009: HtmxResponseAttribute properties are valid attribute arguments ------------
    // The attribute usage below would fail to compile with CS0655 if the properties were nullable.

    [Rizzy.Htmx.HtmxResponse(Refresh = true, PreventBrowserHistoryUpdate = true, PreventBrowserCurrentUrlUpdate = true, Reswap = Rizzy.Htmx.SwapStyle.outerHTML)]
    private static void DecoratedAction() { }

    [Fact]
    public void HtmxResponseAttribute_accepts_boolean_and_enum_arguments()
    {
        var attribute = typeof(PhaseAFixTests)
            .GetMethod(nameof(DecoratedAction), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .GetCustomAttributes(typeof(Rizzy.Htmx.HtmxResponseAttribute), false)
            .Cast<Rizzy.Htmx.HtmxResponseAttribute>()
            .Single();

        attribute.Refresh.Should().BeTrue();
        attribute.Reswap.Should().Be(Rizzy.Htmx.SwapStyle.outerHTML);
    }
}
