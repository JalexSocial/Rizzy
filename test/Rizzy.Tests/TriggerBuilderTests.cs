using System.Reflection;
using Rizzy.Htmx;

namespace Rizzy.Tests;

public class TriggerBuilderTests
{
    [Fact]
    public void Trigger_OnEvent_Message_SerializesToMessage()
        => Trigger.OnEvent("message").Build().Key.Should().Be("message");

    [Fact]
    public void Trigger_ShouldNotExposeSseMethod()
        => typeof(Trigger).GetMethods(BindingFlags.Public | BindingFlags.Static).Any(m => m.Name == "Sse").Should().BeFalse();

    [Fact]
    public void TriggerBuilder_ShouldNotExposeSseMethod()
        => typeof(TriggerBuilder).GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(m => m.Name == "Sse").Should().BeFalse();

    [Fact]
    public void HtmxTriggerSpecification_ShouldNotExposeSseEventProperty()
        => typeof(HtmxTriggerSpecification).GetProperty("SseEvent", BindingFlags.Public | BindingFlags.Instance).Should().BeNull();
}
