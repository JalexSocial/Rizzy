namespace Rizzy.Htmx;

internal sealed record HtmxSwapSpecification
{
    public SwapStyle SwapStyle { get; set; }

    public string? SwapDelay { get; set; }
    public string? SettleDelay { get; set; }

    public string? Scroll { get; set; }
    public string? ScrollTarget { get; set; }

    public string? Show { get; set; }
    public string? ShowTarget { get; set; }

    public bool? Transition { get; set; }
    public bool? IgnoreTitle { get; set; }
    public bool? FocusScroll { get; set; }

    public bool? Strip { get; set; }
    public string? Target { get; set; }
}
