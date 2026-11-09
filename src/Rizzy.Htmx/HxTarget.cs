namespace Rizzy.Htmx;

/// <summary>
/// htmx 4 extended selectors, for <c>hx-target</c>, <c>hx-include</c>, <c>hx-indicator</c>,
/// <c>hx-disable</c> and the <c>from:</c> trigger modifier.
/// </summary>
public static class HxTarget
{
    /// <summary>The element itself (<c>this</c>).</summary>
    public const string This = "this";
    /// <summary>The document (<c>document</c>).</summary>
    public const string Document = "document";
    /// <summary>The window (<c>window</c>).</summary>
    public const string Window = "window";
    /// <summary>The body (<c>body</c>).</summary>
    public const string Body = "body";
    /// <summary>The shadow root's host element (<c>host</c>).</summary>
    public const string Host = "host";

    /// <summary>The element with this id, CSS-escaped (<c>#item\:1</c>).</summary>
    public static string Id(string id) => CssSelector.Id(id);
    /// <summary>The closest ancestor (or self) matching <paramref name="selector"/>.</summary>
    public static string Closest(string selector) => "closest " + Require(selector);
    /// <summary>The first descendant matching <paramref name="selector"/>.</summary>
    public static string Find(string selector) => "find " + Require(selector);
    /// <summary>All descendants matching <paramref name="selector"/>.</summary>
    public static string FindAll(string selector) => "findAll " + Require(selector);
    /// <summary>The next sibling, or the next element in the document matching <paramref name="selector"/>.</summary>
    public static string Next(string? selector = null) => string.IsNullOrWhiteSpace(selector) ? "next" : "next " + selector;
    /// <summary>The previous sibling, or the previous element in the document matching <paramref name="selector"/>.</summary>
    public static string Previous(string? selector = null) => string.IsNullOrWhiteSpace(selector) ? "previous" : "previous " + selector;
    /// <summary>Searches the whole document, crossing shadow roots (<c>global ...</c>).</summary>
    public static string Global(string selector) => "global " + Require(selector);

    private static string Require(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        return selector;
    }
}

/// <summary>Strategies for <c>hx-sync</c>: what happens to a request while another one on the same element is in flight.</summary>
public enum HxSyncStrategy
{
    /// <summary>Ignore the new request (<c>drop</c>).</summary>
    Drop,
    /// <summary>Let the in-flight request continue and abort the new one if the in-flight one is still running (<c>abort</c>).</summary>
    Abort,
    /// <summary>Abort the in-flight request and send the new one (<c>replace</c>).</summary>
    Replace,
    /// <summary>Queue the first request that arrives while one is in flight (<c>queue first</c>, htmx's default).</summary>
    QueueFirst,
    /// <summary>Queue only the last request that arrives (<c>queue last</c>).</summary>
    QueueLast,
    /// <summary>Queue every request (<c>queue all</c>).</summary>
    QueueAll,
}

/// <summary>Merge strategies for the <c>hx-head</c> extension attribute.</summary>
public enum HxHeadStrategy
{
    /// <summary>Merge the response's head into the page's (<c>merge</c>; the default for full-page swaps).</summary>
    Merge,
    /// <summary>Append the response's head elements (<c>append</c>; the default for partial swaps).</summary>
    Append,
    /// <summary>Re-insert (and so re-run) this element on every merge (<c>re-eval</c>).</summary>
    ReEval,
}

/// <summary>
/// Props for the <c>rizzy-confirm</c> extension's JSON <c>hx-confirm</c> value, read by a custom
/// dialog (<c>rz:confirm-dialog</c>). Without a custom dialog, the browser's <c>confirm()</c> shows
/// <see cref="Title"/> and <see cref="Text"/>.
/// </summary>
public sealed record class HxConfirm
{
    /// <summary>Dialog title.</summary>
    public string? Title { get; init; }
    /// <summary>Dialog text.</summary>
    public string? Text { get; init; }
    /// <summary>Show the confirm button (default true).</summary>
    public bool? ShowConfirmButton { get; init; }
    /// <summary>Show a deny button (default false).</summary>
    public bool? ShowDenyButton { get; init; }
    /// <summary>Show a cancel button (default false).</summary>
    public bool? ShowCancelButton { get; init; }
    /// <summary>Confirm button text (default "OK").</summary>
    public string? ConfirmButtonText { get; init; }
    /// <summary>Deny button text (default "No").</summary>
    public string? DenyButtonText { get; init; }
    /// <summary>Cancel button text (default "Cancel").</summary>
    public string? DismissButtonText { get; init; }
}
