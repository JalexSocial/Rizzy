namespace Rizzy.Htmx;

/// <summary>
/// Entry point for <see cref="HxAttributes"/>: every method starts a new attribute set, which
/// you continue fluently and pass to <c>@attributes</c>.
/// <code>
/// &lt;button @attributes="Hx.Delete(Url).Confirm(&quot;Delete?&quot;).Target(HxTarget.Closest(&quot;tr&quot;)).Swap(SwapStyle.outerHTML)"&gt;
/// </code>
/// </summary>
/// <remarks>Each method mirrors the <see cref="HxAttributes"/> method of the same name (a test keeps them in step).</remarks>
public static class Hx
{
    /// <summary>Starts an empty attribute set.</summary>
    public static HxAttributes Attributes() => new();

    /// <summary>Sets any attribute (escape hatch for attributes without a method).</summary>
    /// <param name="name">The attribute name, for example <c>hx-get</c>.</param>
    /// <param name="value">The value. <see langword="null"/> removes the attribute.</param>
    public static HxAttributes Attr(string name, object? value) => new HxAttributes().Attr(name, value);
    /// <summary><c>hx-get</c>: issues a GET request to <paramref name="url"/>.</summary>
    public static HxAttributes Get(string url) => new HxAttributes().Get(url);
    /// <summary><c>hx-post</c>: issues a POST request to <paramref name="url"/>.</summary>
    public static HxAttributes Post(string url) => new HxAttributes().Post(url);
    /// <summary><c>hx-put</c>: issues a PUT request to <paramref name="url"/>.</summary>
    public static HxAttributes Put(string url) => new HxAttributes().Put(url);
    /// <summary><c>hx-patch</c>: issues a PATCH request to <paramref name="url"/>.</summary>
    public static HxAttributes Patch(string url) => new HxAttributes().Patch(url);
    /// <summary><c>hx-delete</c>: issues a DELETE request to <paramref name="url"/>.</summary>
    public static HxAttributes Delete(string url) => new HxAttributes().Delete(url);
    /// <summary><c>hx-query</c>: issues a QUERY request to <paramref name="url"/> (htmx 4).</summary>
    public static HxAttributes Query(string url) => new HxAttributes().Query(url);
    /// <summary><c>hx-action</c>: the request URL, combined with <see cref="Method"/>.</summary>
    public static HxAttributes Action(string url) => new HxAttributes().Action(url);
    /// <summary><c>hx-method</c>: the HTTP method used with <see cref="Action"/>.</summary>
    public static HxAttributes Method(string method) => new HxAttributes().Method(method);
    /// <summary><c>hx-target</c>: where the response goes (a CSS selector or an <see cref="HxTarget"/> value).</summary>
    public static HxAttributes Target(string selector) => new HxAttributes().Target(selector);
    /// <summary><c>hx-swap</c>: how the response is swapped.</summary>
    public static HxAttributes Swap(SwapStyle style) => new HxAttributes().Swap(style);
    /// <summary><c>hx-swap</c>: how the response is swapped, with modifiers.</summary>
    public static HxAttributes Swap(SwapStyleBuilder style) => new HxAttributes().Swap(style);
    /// <summary><c>hx-swap</c>: a raw swap specification.</summary>
    public static HxAttributes Swap(string swap) => new HxAttributes().Swap(swap);
    /// <summary><c>hx-select</c>: which part of the response to swap in.</summary>
    public static HxAttributes Select(string selector) => new HxAttributes().Select(selector);
    /// <summary><c>hx-select-oob</c>: parts of the response to swap out of band (<c>#a, #b:afterbegin</c>).</summary>
    public static HxAttributes SelectOob(string selectors) => new HxAttributes().SelectOob(selectors);
    /// <summary><c>hx-swap-oob</c>: swaps this element out of band, by id (<c>true</c>) or with a swap style.</summary>
    public static HxAttributes SwapOob(string value = "true") => new HxAttributes().SwapOob(value);
    /// <summary><c>hx-swap-oob</c>: swaps this element out of band with a swap style.</summary>
    public static HxAttributes SwapOob(SwapStyle style) => new HxAttributes().SwapOob(style);
    /// <summary><c>hx-swap-oob</c>: swaps this element out of band with a swap style and modifiers.</summary>
    public static HxAttributes SwapOob(SwapStyleBuilder style) => new HxAttributes().SwapOob(style);
    /// <summary><c>hx-trigger</c>: when the request is sent.</summary>
    public static HxAttributes Trigger(TriggerModifierBuilder trigger) => new HxAttributes().Trigger(trigger);
    /// <summary><c>hx-trigger</c>: when the request is sent.</summary>
    public static HxAttributes Trigger(TriggerBuilder trigger) => new HxAttributes().Trigger(trigger);
    /// <summary><c>hx-trigger</c>: a raw trigger specification.</summary>
    public static HxAttributes Trigger(string trigger) => new HxAttributes().Trigger(trigger);
    /// <summary><c>hx-vals</c>: extra values to send, from an object or dictionary (encoded as JSON).</summary>
    public static HxAttributes Vals(object values) => new HxAttributes().Vals(values);
    /// <summary><c>hx-headers</c>: extra request headers, from an object or dictionary (encoded as JSON).</summary>
    public static HxAttributes Headers(object headers) => new HxAttributes().Headers(headers);
    /// <summary><c>hx-include</c>: other elements whose values are sent (a selector or <see cref="HxTarget"/> value).</summary>
    public static HxAttributes Include(string selector) => new HxAttributes().Include(selector);
    /// <summary><c>hx-encoding</c>: the request body encoding.</summary>
    public static HxAttributes Encoding(string encoding) => new HxAttributes().Encoding(encoding);
    /// <summary><c>hx-encoding="multipart/form-data"</c>, needed for file uploads.</summary>
    public static HxAttributes EncodingMultipart() => new HxAttributes().EncodingMultipart();
    /// <summary><c>hx-validate</c>: run the browser's form validation before sending (forms validate by default).</summary>
    public static HxAttributes Validate(bool validate = true) => new HxAttributes().Validate(validate);
    /// <summary><c>hx-config</c>: per-request settings (for example <c>new { timeout = 5000 }</c>), encoded as JSON.</summary>
    public static HxAttributes Config(object config) => new HxAttributes().Config(config);
    /// <summary><c>hx-confirm</c>: asks before sending, with the browser's <c>confirm()</c> or a custom dialog.</summary>
    public static HxAttributes Confirm(string text) => new HxAttributes().Confirm(text);
    /// <summary><c>hx-confirm</c> with props for a custom dialog (the <c>rizzy-confirm</c> extension).</summary>
    public static HxAttributes Confirm(HxConfirm props) => new HxAttributes().Confirm(props);
    /// <summary><c>hx-indicator</c>: elements that get the request class while the request runs.</summary>
    public static HxAttributes Indicator(string selector) => new HxAttributes().Indicator(selector);
    /// <summary><c>hx-disable</c>: elements disabled while the request runs (htmx 4).</summary>
    public static HxAttributes Disable(string selector) => new HxAttributes().Disable(selector);
    /// <summary><c>hx-sync</c>: a raw sync specification (<c>closest form:abort</c>).</summary>
    public static HxAttributes Sync(string sync) => new HxAttributes().Sync(sync);
    /// <summary><c>hx-sync</c>: synchronizes requests with those of another element.</summary>
    /// <param name="selector">The element to synchronize with (<c>this</c>, <c>closest form</c>, ...).</param>
    /// <param name="strategy">What happens to a request while another is in flight.</param>
    public static HxAttributes Sync(string selector, HxSyncStrategy strategy) => new HxAttributes().Sync(selector, strategy);
    /// <summary><c>hx-status:&lt;code&gt;</c>: how to handle a response status (<c>Status(422, "target:#errors")</c>).</summary>
    public static HxAttributes Status(int statusCode, string spec) => new HxAttributes().Status(statusCode, spec);
    /// <summary><c>hx-status:&lt;pattern&gt;</c>: how to handle a status pattern such as <c>4xx</c> or <c>5xx</c>.</summary>
    public static HxAttributes Status(string pattern, string spec) => new HxAttributes().Status(pattern, spec);
    /// <summary><c>hx-boost</c>: turns links and forms inside into htmx requests.</summary>
    public static HxAttributes Boost(bool boost = true) => new HxAttributes().Boost(boost);
    /// <summary><c>hx-boost</c> with settings applied to boosted requests (for example <c>new { target = "#main" }</c>).</summary>
    public static HxAttributes Boost(object settings) => new HxAttributes().Boost(settings);
    /// <summary><c>hx-push-url</c>: push the request URL (<see langword="true"/>) into history, or not.</summary>
    public static HxAttributes PushUrl(bool push = true) => new HxAttributes().PushUrl(push);
    /// <summary><c>hx-push-url</c>: push <paramref name="url"/> into history.</summary>
    public static HxAttributes PushUrl(string url) => new HxAttributes().PushUrl(url);
    /// <summary><c>hx-replace-url</c>: replace the current URL with the request URL (<see langword="true"/>), or not.</summary>
    public static HxAttributes ReplaceUrl(bool replace = true) => new HxAttributes().ReplaceUrl(replace);
    /// <summary><c>hx-replace-url</c>: replace the current URL with <paramref name="url"/>.</summary>
    public static HxAttributes ReplaceUrl(string url) => new HxAttributes().ReplaceUrl(url);
    /// <summary><c>hx-history-elt</c>: on back/forward, restore only this element.</summary>
    public static HxAttributes HistoryElt() => new HxAttributes().HistoryElt();
    /// <summary><c>hx-ignore</c>: htmx does not process this element or its descendants.</summary>
    public static HxAttributes Ignore() => new HxAttributes().Ignore();
    /// <summary><c>hx-preserve</c>: keep this element (matched by id) unchanged across swaps.</summary>
    public static HxAttributes Preserve() => new HxAttributes().Preserve();
    /// <summary><c>hx-morph-skip</c>: morph swaps leave this element unchanged.</summary>
    public static HxAttributes MorphSkip() => new HxAttributes().MorphSkip();
    /// <summary><c>hx-morph-skip-children</c>: morph swaps leave this element's children unchanged.</summary>
    public static HxAttributes MorphSkipChildren() => new HxAttributes().MorphSkipChildren();
    /// <summary>
    /// <c>hx-on:&lt;event&gt;</c>: runs <paramref name="script"/> when <paramref name="eventName"/>
    /// fires (for example <c>On(HtmxEvents.AfterSwap, "...")</c>). This is inline script: a
    /// nonce-based Content Security Policy blocks it unless the page allows it.
    /// </summary>
    public static HxAttributes On(string eventName, string script) => new HxAttributes().On(eventName, script);
    /// <summary>
    /// <c>hx-on</c> with several handlers in one attribute, each with a full trigger
    /// specification (<c>("click once", "...")</c>). Inline script, like <see cref="On(string, string)"/>.
    /// </summary>
    public static HxAttributes On(params (string Trigger, string Script)[] handlers) => new HxAttributes().On(handlers);
    /// <summary><c>hx-sse:connect</c> (<see cref="HtmxExtension.Sse"/>): opens a Server-Sent Events connection.</summary>
    public static HxAttributes SseConnect(string url) => new HxAttributes().SseConnect(url);
    /// <summary><c>hx-sse:close</c> (<see cref="HtmxExtension.Sse"/>): closes the connection when this SSE event arrives.</summary>
    public static HxAttributes SseClose(string eventName) => new HxAttributes().SseClose(eventName);
    /// <summary><c>hx-ws:connect</c> (<see cref="HtmxExtension.Ws"/>): opens a WebSocket.</summary>
    public static HxAttributes WsConnect(string url) => new HxAttributes().WsConnect(url);
    /// <summary><c>hx-ws:send</c> (<see cref="HtmxExtension.Ws"/>): sends this element's values over the WebSocket when triggered.</summary>
    public static HxAttributes WsSend() => new HxAttributes().WsSend();
    /// <summary><c>hx-multipart:connect</c> (<see cref="HtmxExtension.Multipart"/>): streams a multipart response.</summary>
    public static HxAttributes MultipartConnect(string url) => new HxAttributes().MultipartConnect(url);
    /// <summary><c>hx-multipart:close</c> (<see cref="HtmxExtension.Multipart"/>): closes the stream when this part event arrives.</summary>
    public static HxAttributes MultipartClose(string eventName) => new HxAttributes().MultipartClose(eventName);
    /// <summary><c>hx-preload</c> (<see cref="HtmxExtension.Preload"/>): preloads the request, by default on hover/focus.</summary>
    public static HxAttributes Preload() => new HxAttributes().Preload();
    /// <summary><c>hx-preload</c> (<see cref="HtmxExtension.Preload"/>): preloads the request on the given trigger.</summary>
    public static HxAttributes Preload(TriggerModifierBuilder trigger) => new HxAttributes().Preload(trigger);
    /// <summary><c>hx-preload</c> (<see cref="HtmxExtension.Preload"/>): preloads the request on a raw trigger specification.</summary>
    public static HxAttributes Preload(string trigger) => new HxAttributes().Preload(trigger);
    /// <summary><c>hx-head</c> (<see cref="HtmxExtension.Head"/>): how a response's head is merged, or <c>re-eval</c> on a head element.</summary>
    public static HxAttributes Head(HxHeadStrategy strategy) => new HxAttributes().Head(strategy);
    /// <summary><c>hx-prompt</c> (<see cref="HtmxExtension.Prompt"/>): asks for text before sending; the answer arrives as <c>HX-Prompt</c>.</summary>
    public static HxAttributes Prompt(string text) => new HxAttributes().Prompt(text);
    /// <summary><c>hx-live</c> (<see cref="HtmxExtension.Live"/>): a reactive expression re-evaluated as the page changes.</summary>
    public static HxAttributes Live(string expression) => new HxAttributes().Live(expression);
    /// <summary><c>hx-pending</c> (<see cref="HtmxExtension.Pending"/>): selector of a template shown in the target while the request runs.</summary>
    public static HxAttributes Pending(string templateSelector) => new HxAttributes().Pending(templateSelector);
    /// <summary><c>hx-ptag</c> (<see cref="HtmxExtension.PTag"/>): the initial polling tag; later tags come from <c>HX-PTag</c>.</summary>
    public static HxAttributes PTag(string initialTag) => new HxAttributes().PTag(initialTag);
    /// <summary><c>hx-targets</c> (<see cref="HtmxExtension.Targets"/>): swaps the response into every element matching the selector.</summary>
    public static HxAttributes Targets(string selector) => new HxAttributes().Targets(selector);
    /// <summary><c>hx-browser-indicator</c> (<see cref="HtmxExtension.BrowserIndicator"/>): show the browser's loading indicator.</summary>
    public static HxAttributes BrowserIndicator(bool show = true) => new HxAttributes().BrowserIndicator(show);
    /// <summary><c>hx-history="false"</c> (<see cref="HtmxExtension.HistoryCache"/>): keep this page out of the history cache (for example pages with sensitive data).</summary>
    public static HxAttributes History(bool cache) => new HxAttributes().History(cache);
    /// <summary><c>hx-nonce</c> (<see cref="HtmxExtension.Csp"/>): the page nonce, required on every htmx element by the hx-csp extension.</summary>
    public static HxAttributes Nonce(string nonce) => new HxAttributes().Nonce(nonce);
}
