using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rizzy.Htmx;

/// <summary>
/// A fluent set of htmx 4 attributes for Blazor attribute splatting. Start with a method on
/// <see cref="Hx"/> and pass the result to <c>@attributes</c>:
/// <code>
/// &lt;button @attributes="Hx.Post(&quot;/save&quot;).Target(HxTarget.Closest(&quot;form&quot;)).Swap(SwapStyle.outerHTML)"&gt;
/// </code>
/// </summary>
/// <remarks>
/// <para>There is a method for every attribute htmx 4.0.1 and its official extensions read; plain
/// <c>hx-*</c> attributes keep working alongside. Calling a method again replaces that attribute.</para>
/// <para>Attribute names use the canonical <c>hx-</c> prefix, which htmx always accepts (a configured
/// <see cref="HtmxConfig.Prefix"/> is an alternative, not a replacement). When
/// <see cref="HtmxConfig.MetaCharacter"/> is set, <c>:</c> in names (<c>hx-on:click</c>,
/// <c>hx-status:422</c>, <c>:inherited</c>) is replaced with it, because htmx then looks the names
/// up that way.</para>
/// <para>An instance is mutable and its attributes are read when Blazor renders it. Build one per
/// render (inline in markup, or from a method or property that returns a new instance); don't keep
/// one in a field and keep adding to it, or every later render sees the additions.</para>
/// <para>Methods for extension attributes need the extension enabled with
/// <see cref="HtmxConfig.UseExtensions"/>; htmx ignores the attributes otherwise.</para>
/// </remarks>
public sealed class HxAttributes : IEnumerable<KeyValuePair<string, object>>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    // Ordered, so the rendered attributes follow the call order.
    private readonly List<(string Name, object Value)> _attributes = [];
    private int _lastIndex = -1;
    private string? _metaCharacter;
    private bool _metaCharacterSet;

    /// <summary>
    /// The meta character applied when no <see cref="WithMetaCharacter"/> override is given. Set
    /// from <see cref="HtmxConfig.MetaCharacter"/> when the app starts (<c>AddHtmx</c>).
    /// </summary>
    public static string? DefaultMetaCharacter { get; internal set; }

    /// <summary>Overrides <see cref="DefaultMetaCharacter"/> for this attribute set.</summary>
    public HxAttributes WithMetaCharacter(string? metaCharacter)
    {
        _metaCharacter = metaCharacter;
        _metaCharacterSet = true;
        return this;
    }

    /// <summary>Sets any attribute (escape hatch for attributes without a method).</summary>
    /// <param name="name">The attribute name, for example <c>hx-get</c>.</param>
    /// <param name="value">The value. <see langword="null"/> removes the attribute.</param>
    public HxAttributes Attr(string name, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        // The same attribute with or without :inherited/:append is still the same attribute.
        var baseName = BaseName(name);
        var index = _attributes.FindIndex(a => BaseName(a.Name) == baseName);
        if (value is null)
        {
            if (index >= 0) _attributes.RemoveAt(index);
            _lastIndex = -1;
            return this;
        }
        if (index >= 0)
        {
            _attributes[index] = (name, value);
            _lastIndex = index;
        }
        else
        {
            _attributes.Add((name, value));
            _lastIndex = _attributes.Count - 1;
        }
        return this;
    }

    private static string BaseName(string name)
    {
        foreach (var suffix in Suffixes)
        {
            if (name.EndsWith(suffix, StringComparison.Ordinal))
                return name[..^suffix.Length];
        }
        return name;
    }

    private static readonly string[] Suffixes = [":inherited:append", ":inherited", ":append"];

    private HxAttributes Flag(string name) => Attr(name, "true");

    private HxAttributes Json(string name, object? value)
        => Attr(name, value is null ? null : value as string ?? JsonSerializer.Serialize(value, JsonOptions));

    // ---- Inheritance modifiers ------------------------------------------------------------------

    /// <summary>
    /// Makes the attribute set by the previous call inherited by descendants
    /// (<c>hx-target:inherited</c>). htmx 4 does not inherit attributes otherwise.
    /// </summary>
    public HxAttributes Inherited() => Suffix(":inherited");

    /// <summary>
    /// Makes the attribute set by the previous call append to the value inherited from an
    /// ancestor (<c>hx-include:append</c>). Combined with <see cref="Inherited"/> it renders
    /// <c>name:inherited:append</c>.
    /// </summary>
    public HxAttributes Append() => Suffix(":append");

    private HxAttributes Suffix(string suffix)
    {
        if (_lastIndex < 0)
            throw new InvalidOperationException($"Call an attribute method before {suffix.TrimStart(':')}().");
        var (name, value) = _attributes[_lastIndex];
        var hasInherited = name.EndsWith(":inherited", StringComparison.Ordinal) || name.EndsWith(":inherited:append", StringComparison.Ordinal);
        var hasAppend = name.EndsWith(":append", StringComparison.Ordinal);
        if (suffix == ":inherited") hasInherited = true;
        else hasAppend = true;
        // htmx 4 reads name, name:inherited, name:append and name:inherited:append.
        _attributes[_lastIndex] = (BaseName(name) + (hasInherited ? ":inherited" : "") + (hasAppend ? ":append" : ""), value);
        return this;
    }

    // ---- Requests -------------------------------------------------------------------------------

    /// <summary><c>hx-get</c>: issues a GET request to <paramref name="url"/>.</summary>
    public HxAttributes Get(string url) => Attr("hx-get", url);
    /// <summary><c>hx-post</c>: issues a POST request to <paramref name="url"/>.</summary>
    public HxAttributes Post(string url) => Attr("hx-post", url);
    /// <summary><c>hx-put</c>: issues a PUT request to <paramref name="url"/>.</summary>
    public HxAttributes Put(string url) => Attr("hx-put", url);
    /// <summary><c>hx-patch</c>: issues a PATCH request to <paramref name="url"/>.</summary>
    public HxAttributes Patch(string url) => Attr("hx-patch", url);
    /// <summary><c>hx-delete</c>: issues a DELETE request to <paramref name="url"/>.</summary>
    public HxAttributes Delete(string url) => Attr("hx-delete", url);
    /// <summary><c>hx-query</c>: issues a QUERY request to <paramref name="url"/> (htmx 4).</summary>
    public HxAttributes Query(string url) => Attr("hx-query", url);
    /// <summary><c>hx-action</c>: the request URL, combined with <see cref="Method"/>.</summary>
    public HxAttributes Action(string url) => Attr("hx-action", url);
    /// <summary><c>hx-method</c>: the HTTP method used with <see cref="Action"/>.</summary>
    public HxAttributes Method(string method) => Attr("hx-method", method);

    // ---- Targeting and swapping -------------------------------------------------------------------

    /// <summary><c>hx-target</c>: where the response goes (a CSS selector or an <see cref="HxTarget"/> value).</summary>
    public HxAttributes Target(string selector) => Attr("hx-target", selector);
    /// <summary><c>hx-swap</c>: how the response is swapped.</summary>
    public HxAttributes Swap(SwapStyle style) => Attr("hx-swap", style.ToHtmxString());
    /// <summary><c>hx-swap</c>: how the response is swapped, with modifiers.</summary>
    public HxAttributes Swap(SwapStyleBuilder style) => Attr("hx-swap", style.ToString());
    /// <summary><c>hx-swap</c>: a raw swap specification.</summary>
    public HxAttributes Swap(string swap) => Attr("hx-swap", swap);
    /// <summary><c>hx-select</c>: which part of the response to swap in.</summary>
    public HxAttributes Select(string selector) => Attr("hx-select", selector);
    /// <summary><c>hx-select-oob</c>: parts of the response to swap out of band (<c>#a, #b:afterbegin</c>).</summary>
    public HxAttributes SelectOob(string selectors) => Attr("hx-select-oob", selectors);
    /// <summary><c>hx-swap-oob</c>: swaps this element out of band, by id (<c>true</c>) or with a swap style.</summary>
    public HxAttributes SwapOob(string value = "true") => Attr("hx-swap-oob", value);
    /// <summary><c>hx-swap-oob</c>: swaps this element out of band with a swap style.</summary>
    public HxAttributes SwapOob(SwapStyle style) => Attr("hx-swap-oob", style.ToHtmxString());
    /// <summary><c>hx-swap-oob</c>: swaps this element out of band with a swap style and modifiers.</summary>
    public HxAttributes SwapOob(SwapStyleBuilder style) => Attr("hx-swap-oob", style.ToString());

    // ---- Triggers ---------------------------------------------------------------------------------

    /// <summary><c>hx-trigger</c>: when the request is sent.</summary>
    public HxAttributes Trigger(TriggerModifierBuilder trigger) => Attr("hx-trigger", trigger.ToString());
    /// <summary><c>hx-trigger</c>: when the request is sent.</summary>
    public HxAttributes Trigger(TriggerBuilder trigger) => Attr("hx-trigger", trigger.ToString());
    /// <summary><c>hx-trigger</c>: a raw trigger specification.</summary>
    public HxAttributes Trigger(string trigger) => Attr("hx-trigger", trigger);

    // ---- Request content --------------------------------------------------------------------------

    /// <summary><c>hx-vals</c>: extra values to send, from an object or dictionary (encoded as JSON).</summary>
    public HxAttributes Vals(object values) => Json("hx-vals", values);
    /// <summary><c>hx-headers</c>: extra request headers, from an object or dictionary (encoded as JSON).</summary>
    public HxAttributes Headers(object headers) => Json("hx-headers", headers);
    /// <summary><c>hx-include</c>: other elements whose values are sent (a selector or <see cref="HxTarget"/> value).</summary>
    public HxAttributes Include(string selector) => Attr("hx-include", selector);
    /// <summary><c>hx-encoding</c>: the request body encoding.</summary>
    public HxAttributes Encoding(string encoding) => Attr("hx-encoding", encoding);
    /// <summary><c>hx-encoding="multipart/form-data"</c>, needed for file uploads.</summary>
    public HxAttributes EncodingMultipart() => Encoding("multipart/form-data");
    /// <summary><c>hx-validate</c>: run the browser's form validation before sending (forms validate by default).</summary>
    public HxAttributes Validate(bool validate = true) => Attr("hx-validate", validate ? "true" : "false");
    /// <summary><c>hx-config</c>: per-request settings (for example <c>new { timeout = 5000 }</c>), encoded as JSON.</summary>
    public HxAttributes Config(object config) => Json("hx-config", config);

    // ---- Request behavior -------------------------------------------------------------------------

    /// <summary><c>hx-confirm</c>: asks before sending, with the browser's <c>confirm()</c> or a custom dialog.</summary>
    public HxAttributes Confirm(string text) => Attr("hx-confirm", text);
    /// <summary><c>hx-confirm</c> with props for a custom dialog (the <c>rizzy-confirm</c> extension).</summary>
    public HxAttributes Confirm(HxConfirm props) => Json("hx-confirm", props);
    /// <summary><c>hx-indicator</c>: elements that get the request class while the request runs.</summary>
    public HxAttributes Indicator(string selector) => Attr("hx-indicator", selector);
    /// <summary><c>hx-disable</c>: elements disabled while the request runs (htmx 4).</summary>
    public HxAttributes Disable(string selector) => Attr("hx-disable", selector);
    /// <summary><c>hx-sync</c>: a raw sync specification (<c>closest form:abort</c>).</summary>
    public HxAttributes Sync(string sync) => Attr("hx-sync", sync);
    /// <summary><c>hx-sync</c>: synchronizes requests with those of another element.</summary>
    /// <param name="selector">The element to synchronize with (<c>this</c>, <c>closest form</c>, ...).</param>
    /// <param name="strategy">What happens to a request while another is in flight.</param>
    public HxAttributes Sync(string selector, HxSyncStrategy strategy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        var value = strategy switch
        {
            HxSyncStrategy.Drop => "drop",
            HxSyncStrategy.Abort => "abort",
            HxSyncStrategy.Replace => "replace",
            HxSyncStrategy.QueueFirst => "queue first",
            HxSyncStrategy.QueueLast => "queue last",
            HxSyncStrategy.QueueAll => "queue all",
            _ => throw new ArgumentOutOfRangeException(nameof(strategy)),
        };
        return Attr("hx-sync", $"{selector}:{value}");
    }

    /// <summary><c>hx-status:&lt;code&gt;</c>: how to handle a response status (<c>Status(422, "target:#errors")</c>).</summary>
    public HxAttributes Status(int statusCode, string spec) => Attr(HtmxStatus.Attribute(statusCode), spec);
    /// <summary><c>hx-status:&lt;pattern&gt;</c>: how to handle a status pattern such as <c>4xx</c> or <c>5xx</c>.</summary>
    public HxAttributes Status(string pattern, string spec) => Attr(HtmxStatus.Attribute(pattern), spec);

    // ---- Navigation and history -------------------------------------------------------------------

    /// <summary><c>hx-boost</c>: turns links and forms inside into htmx requests.</summary>
    public HxAttributes Boost(bool boost = true) => Attr("hx-boost", boost ? "true" : "false");
    /// <summary><c>hx-boost</c> with settings applied to boosted requests (for example <c>new { target = "#main" }</c>).</summary>
    public HxAttributes Boost(object settings) => Json("hx-boost", settings);
    /// <summary><c>hx-push-url</c>: push the request URL (<see langword="true"/>) into history, or not.</summary>
    public HxAttributes PushUrl(bool push = true) => Attr("hx-push-url", push ? "true" : "false");
    /// <summary><c>hx-push-url</c>: push <paramref name="url"/> into history.</summary>
    public HxAttributes PushUrl(string url) => Attr("hx-push-url", url);
    /// <summary><c>hx-replace-url</c>: replace the current URL with the request URL (<see langword="true"/>), or not.</summary>
    public HxAttributes ReplaceUrl(bool replace = true) => Attr("hx-replace-url", replace ? "true" : "false");
    /// <summary><c>hx-replace-url</c>: replace the current URL with <paramref name="url"/>.</summary>
    public HxAttributes ReplaceUrl(string url) => Attr("hx-replace-url", url);
    /// <summary><c>hx-history-elt</c>: on back/forward, restore only this element.</summary>
    public HxAttributes HistoryElt() => Flag("hx-history-elt");

    // ---- Processing control -----------------------------------------------------------------------

    /// <summary><c>hx-ignore</c>: htmx does not process this element or its descendants.</summary>
    public HxAttributes Ignore() => Flag("hx-ignore");
    /// <summary><c>hx-preserve</c>: keep this element (matched by id) unchanged across swaps.</summary>
    public HxAttributes Preserve() => Flag("hx-preserve");
    /// <summary><c>hx-morph-skip</c>: morph swaps leave this element unchanged.</summary>
    public HxAttributes MorphSkip() => Flag("hx-morph-skip");
    /// <summary><c>hx-morph-skip-children</c>: morph swaps leave this element's children unchanged.</summary>
    public HxAttributes MorphSkipChildren() => Flag("hx-morph-skip-children");

    // ---- Event handlers ---------------------------------------------------------------------------

    /// <summary>
    /// <c>hx-on:&lt;event&gt;</c>: runs <paramref name="script"/> when <paramref name="eventName"/>
    /// fires (for example <c>On(HtmxEvents.AfterSwap, "...")</c>). This is inline script: a
    /// nonce-based Content Security Policy blocks it unless the page allows it.
    /// </summary>
    public HxAttributes On(string eventName, string script)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventName);
        return Attr("hx-on:" + eventName, script);
    }

    /// <summary>
    /// <c>hx-on</c> with several handlers in one attribute, each with a full trigger
    /// specification (<c>("click once", "...")</c>). Inline script, like <see cref="On(string, string)"/>.
    /// </summary>
    public HxAttributes On(params (string Trigger, string Script)[] handlers)
    {
        ArgumentNullException.ThrowIfNull(handlers);
        return Attr("hx-on", string.Join("; ", handlers.Select(h => $"{h.Trigger} -> {h.Script}")));
    }

    // ---- Extension attributes ---------------------------------------------------------------------

    /// <summary><c>hx-sse:connect</c> (<see cref="HtmxExtension.Sse"/>): opens a Server-Sent Events connection.</summary>
    public HxAttributes SseConnect(string url) => Attr("hx-sse:connect", url);
    /// <summary><c>hx-sse:close</c> (<see cref="HtmxExtension.Sse"/>): closes the connection when this SSE event arrives.</summary>
    public HxAttributes SseClose(string eventName) => Attr("hx-sse:close", eventName);
    /// <summary><c>hx-ws:connect</c> (<see cref="HtmxExtension.Ws"/>): opens a WebSocket.</summary>
    public HxAttributes WsConnect(string url) => Attr("hx-ws:connect", url);
    /// <summary><c>hx-ws:send</c> (<see cref="HtmxExtension.Ws"/>): sends this element's values over the WebSocket when triggered.</summary>
    public HxAttributes WsSend() => Flag("hx-ws:send");
    /// <summary><c>hx-multipart:connect</c> (<see cref="HtmxExtension.Multipart"/>): streams a multipart response.</summary>
    public HxAttributes MultipartConnect(string url) => Attr("hx-multipart:connect", url);
    /// <summary><c>hx-multipart:close</c> (<see cref="HtmxExtension.Multipart"/>): closes the stream when this part event arrives.</summary>
    public HxAttributes MultipartClose(string eventName) => Attr("hx-multipart:close", eventName);
    /// <summary><c>hx-preload</c> (<see cref="HtmxExtension.Preload"/>): preloads the request, by default on hover/focus.</summary>
    public HxAttributes Preload() => Attr("hx-preload", "");
    /// <summary><c>hx-preload</c> (<see cref="HtmxExtension.Preload"/>): preloads the request on the given trigger.</summary>
    public HxAttributes Preload(TriggerModifierBuilder trigger) => Attr("hx-preload", trigger.ToString());
    /// <summary><c>hx-preload</c> (<see cref="HtmxExtension.Preload"/>): preloads the request on a raw trigger specification.</summary>
    public HxAttributes Preload(string trigger) => Attr("hx-preload", trigger);
    /// <summary><c>hx-head</c> (<see cref="HtmxExtension.Head"/>): how a response's head is merged, or <c>re-eval</c> on a head element.</summary>
    public HxAttributes Head(HxHeadStrategy strategy) => Attr("hx-head", strategy switch
    {
        HxHeadStrategy.Merge => "merge",
        HxHeadStrategy.Append => "append",
        HxHeadStrategy.ReEval => "re-eval",
        _ => throw new ArgumentOutOfRangeException(nameof(strategy)),
    });
    /// <summary><c>hx-prompt</c> (<see cref="HtmxExtension.Prompt"/>): asks for text before sending; the answer arrives as <c>HX-Prompt</c>.</summary>
    public HxAttributes Prompt(string text) => Attr("hx-prompt", text);
    /// <summary><c>hx-live</c> (<see cref="HtmxExtension.Live"/>): a reactive expression re-evaluated as the page changes.</summary>
    public HxAttributes Live(string expression) => Attr("hx-live", expression);
    /// <summary><c>hx-pending</c> (<see cref="HtmxExtension.Pending"/>): selector of a template shown in the target while the request runs.</summary>
    public HxAttributes Pending(string templateSelector) => Attr("hx-pending", templateSelector);
    /// <summary><c>hx-ptag</c> (<see cref="HtmxExtension.PTag"/>): the initial polling tag; later tags come from <c>HX-PTag</c>.</summary>
    public HxAttributes PTag(string initialTag) => Attr("hx-ptag", initialTag);
    /// <summary><c>hx-targets</c> (<see cref="HtmxExtension.Targets"/>): swaps the response into every element matching the selector.</summary>
    public HxAttributes Targets(string selector) => Attr("hx-targets", selector);
    /// <summary><c>hx-browser-indicator</c> (<see cref="HtmxExtension.BrowserIndicator"/>): show the browser's loading indicator.</summary>
    public HxAttributes BrowserIndicator(bool show = true) => Attr("hx-browser-indicator", show ? "true" : "false");
    /// <summary><c>hx-history="false"</c> (<see cref="HtmxExtension.HistoryCache"/>): keep this page out of the history cache (for example pages with sensitive data).</summary>
    public HxAttributes History(bool cache) => Attr("hx-history", cache ? "true" : "false");
    /// <summary><c>hx-nonce</c> (<see cref="HtmxExtension.Csp"/>): the page nonce, required on every htmx element by the hx-csp extension.</summary>
    public HxAttributes Nonce(string nonce) => Attr("hx-nonce", nonce);

    // ---- Enumeration ------------------------------------------------------------------------------

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        var metaCharacter = _metaCharacterSet ? _metaCharacter : DefaultMetaCharacter;
        foreach (var (name, value) in _attributes)
        {
            var resolved = string.IsNullOrEmpty(metaCharacter) ? name : name.Replace(":", metaCharacter, StringComparison.Ordinal);
            yield return new KeyValuePair<string, object>(resolved, value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Renders the attributes as HTML (for tests and debugging); values are HTML-attribute-encoded.</summary>
    public override string ToString()
        => string.Join(" ", this.Select(a => $"{a.Key}=\"{System.Net.WebUtility.HtmlEncode(a.Value.ToString())}\""));
}
