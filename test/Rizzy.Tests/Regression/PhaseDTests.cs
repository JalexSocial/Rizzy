using Rizzy.Htmx;

namespace Rizzy.Tests.Regression;

/// <summary>Client diagnostics configuration (RM-D3).</summary>
public class PhaseDTests
{
    [Fact]
    public void RizzyDiagnostics_is_serialized_only_when_set()
    {
        new HtmxConfig().Serialize().Should().NotContain("rizzyDiagnostics");
        new HtmxConfig { RizzyDiagnostics = false }.Serialize().Should().Contain("\"rizzyDiagnostics\":false");
    }
}
