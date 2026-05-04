using Rizzy.Htmx;

namespace Rizzy.Tests;

public class HtmxAttributeNameTests
{
    [Fact] public void Resolve_Default() => HtmxAttributeName.Resolve("hx-target", null).Should().Be("hx-target");
    [Fact] public void Resolve_EmptyPrefix() => HtmxAttributeName.Resolve("hx-target", "").Should().Be("hx-target");
    [Fact] public void Resolve_DataPrefix() => HtmxAttributeName.Resolve("hx-target", "data-hx-").Should().Be("data-hx-target");
    [Fact] public void Resolve_SsePrefix() => HtmxAttributeName.Resolve("hx-sse:connect", "data-hx-").Should().Be("data-hx-sse:connect");
    [Fact] public void Resolve_NonHx() => HtmxAttributeName.Resolve("class", "data-hx-").Should().Be("class");
}
