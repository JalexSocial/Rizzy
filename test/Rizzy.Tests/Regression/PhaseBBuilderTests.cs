using System.Globalization;
using Rizzy.Htmx;

namespace Rizzy.Tests.Regression;

/// <summary>
/// htmx 4 syntax for the trigger and swap builders (RIZZY-007, RIZZY-011, RIZZY-012, RIZZY-013).
/// Expected strings follow htmx 4.0.1's HCON parser: values containing whitespace or commas must be
/// quoted, and the htmx 2 <c>{...}</c> wrapper is not recognised.
/// </summary>
public class PhaseBBuilderTests
{
    // ---- RIZZY-007: selectors with whitespace / commas are quoted ------------------------------

    [Fact]
    public void From_extended_selector_with_space_is_quoted()
        => Trigger.OnEvent("submit").From("closest form").ToString().Should().Be("submit from:'closest form'");

    [Fact]
    public void Target_selector_list_is_quoted()
        => Trigger.OnEvent("click").Target(".a, .b").ToString().Should().Be("click target:'.a, .b'");

    [Fact]
    public void Simple_selectors_stay_bare()
        => Trigger.OnEvent("keydown").From("document").Target(".child").ToString()
            .Should().Be("keydown from:document target:.child");

    [Fact]
    public void Legacy_brace_selector_is_converted_to_quotes()
        => Trigger.OnEvent("click").From("{closest .row}").ToString().Should().Be("click from:'closest .row'");

    [Fact]
    public void Selector_containing_single_quote_uses_double_quotes()
        => Trigger.OnEvent("click").From("find [data-x='a b']").ToString().Should().Be("click from:\"find [data-x='a b']\"");

    [Fact]
    public void Selector_with_both_quote_kinds_throws()
        => FluentActions.Invoking(() => Trigger.OnEvent("click").From("[a='x'] [b=\"y\"]").ToString())
            .Should().Throw<ArgumentException>();

    [Fact]
    public void Root_and_root_margin_are_quoted_when_needed()
        => Trigger.Intersect(root: "#scroll area").RootMargin("10px 0px").ToString()
            .Should().Be("intersect root:'#scroll area' rootMargin:'10px 0px'");

    // ---- RIZZY-011: specs are not cached before modifiers are applied ---------------------------

    [Fact]
    public void ToString_reflects_changes_made_after_first_render()
    {
        var spec = new HtmxTriggerSpecification { Trigger = "click" };
        spec.ToString().Should().Be("click");

        spec.Once = true;
        spec.ToString().Should().Be("click once");
    }

    [Fact]
    public void With_copy_renders_its_own_values()
    {
        var spec = new HtmxTriggerSpecification { Trigger = "click", Delay = 500 };
        _ = spec.ToString();

        var copy = spec with { Delay = 2000 };

        copy.ToString().Should().Be("click delay:2s");
        spec.ToString().Should().Be("click delay:500ms");
    }

    [Fact]
    public void Second_identical_trigger_is_kept_and_receives_its_modifiers()
        => Trigger.OnEvent("click").Or().OnEvent("click").Delay(TimeSpan.FromSeconds(1)).ToString()
            .Should().Be("click, click delay:1s");

    // ---- RIZZY-012: culture-invariant formatting ------------------------------------------------

    [Fact]
    public void Fractional_seconds_render_as_milliseconds_in_any_culture()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");

            Trigger.OnEvent("keyup").Delay(TimeSpan.FromMilliseconds(1500)).ToString().Should().Be("keyup delay:1500ms");
            Trigger.Every(TimeSpan.FromSeconds(2.5)).ToString().Should().Be("every 2500ms");
            Trigger.Intersect(threshold: 0.5f).ToString().Should().Be("intersect threshold:0.5");
            new SwapStyleBuilder(SwapStyle.innerHTML).AfterSettleDelay(TimeSpan.FromSeconds(1.25)).ToString()
                .Should().Be("innerHTML settle:1250ms");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    // ---- RIZZY-007: htmx 4 trigger modifiers ----------------------------------------------------

    [Fact]
    public void Event_handling_modifiers_render()
        => Trigger.OnEvent("submit").Prevent().Stop().Halt().Capture().Passive().ToString()
            .Should().Be("submit prevent stop halt capture passive");

    [Fact]
    public void From_self_and_outside_keywords_render()
    {
        Trigger.OnEvent("click").FromSelf().ToString().Should().Be("click from:self");
        Trigger.OnEvent("click").FromOutside().ToString().Should().Be("click from:outside");
    }

[Fact]
    public void Htmx2_queue_api_is_removed()
        => typeof(TriggerModifierBuilder).GetMethod("Queue").Should().BeNull();

        // ---- RIZZY-013: htmx 4 swap modifiers -------------------------------------------------------

    [Fact]
    public void Scroll_and_show_targets_are_quoted_when_needed()
    {
        new SwapStyleBuilder(SwapStyle.innerHTML).ScrollTop("closest .panel").ToString()
            .Should().Be("innerHTML scroll:top scrollTarget:'closest .panel'");
        new SwapStyleBuilder(SwapStyle.innerHTML).ShowOnBottom("#log").ToString()
            .Should().Be("innerHTML show:bottom showTarget:#log");
    }

    [Fact]
    public void Strip_target_and_swap_empty_render()
        => SwapStyle.outerHTML.Strip().Target("find .body").SwapEmpty().ToString()
            .Should().Be("outerHTML strip:true swapEmpty:true target:'find .body'");

    [Fact]
    public void OuterSync_style_is_supported()
        => SwapStyle.outerSync.ToHtmxString().Should().Be("outerSync");

    [Fact]
    public void Focus_scroll_uses_camel_case()
        => SwapStyle.innerHTML.ScrollFocus().ToString().Should().Be("innerHTML focusScroll:true");
}
