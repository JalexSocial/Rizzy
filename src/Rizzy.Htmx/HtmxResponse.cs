using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Htmx.Serialization;
using System.Net;
using System.Buffers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

namespace Rizzy.Htmx;

/// <summary>
/// Sets htmx response headers (<c>HX-Trigger</c>, <c>HX-Retarget</c>, ...) for the current request.
/// Obtain it with <c>HttpContext.Response.Htmx()</c>.
/// </summary>
/// <remarks>
/// Every method that sets an <c>HX-*</c> header or requests an empty body throws
/// <see cref="InvalidOperationException"/> when the request is not an htmx request (no
/// <c>HX-Request</c> header), because browsers ignore those headers on normal navigation. Guard
/// actions that serve both kinds of request with <c>Request.IsHtmx()</c>, or use
/// <see cref="HtmxResponseAttribute"/>, which does nothing for non-htmx requests.
/// <see cref="StatusCode"/> works for any request.
/// </remarks>
public sealed class HtmxResponse(HttpContext context)
{
    private const string EmptyResponseBodyRequestedKey = "Rizzy.Htmx.EmptyResponseBodyRequested";
    private const string ItemsKeyPrefix = "02E0A668-6E6B-4C53-83A6-17E576073E96";
    private readonly IHeaderDictionary _headers = context.Response.Headers;
    private readonly bool _isHtmxRequest = context.Request.Headers.ContainsKey(HtmxRequestHeaderNames.HtmxRequest);

    /// <summary>
    /// Indicates whether an empty response body was requested.
    /// The value is stored directly in HttpContext.Items.
    /// </summary>
    public bool EmptyResponseBodyRequested
    {
        get
        {
            if (context.Items.TryGetValue(EmptyResponseBodyRequestedKey, out var value) && value is bool flag)
            {
                return flag;
            }
            return false;
        }
        private set => context.Items[EmptyResponseBodyRequestedKey] = value;
    }

    /// <summary>
    /// Sets the response status code to <paramref name="statusCode"/>.
    /// </summary>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse StatusCode(HttpStatusCode statusCode)
    {
        context.Response.StatusCode = (int)statusCode;
        return this;
    }

    /// <summary>
    /// Do not render any component markup to the client, even if the component would have
    /// produced markup normally. Headers and cookies are still returned as normal.
    /// </summary>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse EmptyBody()
    {
        AssertIsHtmxRequest();
        EmptyResponseBodyRequested = true;
        return this;
    }

    /// <summary>
    /// Allows you to do a client-side redirect that does not do a full page reload.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Location(string path)
    {
        AssertIsHtmxRequest();
        _headers[HtmxResponseHeaderNames.Location] = HeaderUrl.Encode(path);
        return this;
    }

    /// <summary>
    /// Allows you to do a client-side redirect that does not do a full page reload.
    /// </summary>
    /// <param name="locationTarget"></param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Location(LocationTarget locationTarget)
    {
        AssertIsHtmxRequest();
        var json = JsonSerializer.Serialize(locationTarget, HtmxAjaxJsonSerializerContext.Default.LocationTarget);
        _headers[HtmxResponseHeaderNames.Location] = json;
        return this;
    }


    /// <summary>
    /// Pushes a new url onto the history stack.
    /// </summary>
    /// <param name="url"></param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse PushUrl(string url)
    {
        AssertIsHtmxRequest();
        _headers[HtmxResponseHeaderNames.PushUrl] = HeaderUrl.Encode(url);
        return this;
    }

    /// <summary>
    /// Pushes a new url onto the history stack.
    /// </summary>
    /// <param name="url"></param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse PushUrl(Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);
        return PushUrl(HeaderUrl.Encode(url));
    }

    /// <summary>
    /// Prevents the browser’s history from being updated.
    /// Overwrites PushUrl response if already present.
    /// </summary>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse PreventBrowserHistoryUpdate()
    {
        AssertIsHtmxRequest();
        _headers[HtmxResponseHeaderNames.PushUrl] = "false";
        return this;
    }

    /// <summary>
    /// Prevents the browser’s current url from being updated
    /// Overwrites ReplaceUrl response if already present.
    /// </summary>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse PreventBrowserCurrentUrlUpdate()
    {
        AssertIsHtmxRequest();
        _headers[HtmxResponseHeaderNames.ReplaceUrl] = "false";
        return this;
    }

    /// <summary>
    /// Can be used to do a client-side redirect to a new location.
    /// </summary>
    /// <param name="url">The url to redirect to.</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Redirect(string url)
    {
        AssertIsHtmxRequest();
        _headers[HtmxResponseHeaderNames.Redirect] = HeaderUrl.Encode(url);
        EmptyResponseBodyRequested = true;
        return this;
    }

    /// <summary>
    /// Can be used to do a client-side redirect to a new location.
    /// </summary>
    /// <param name="url">The url to redirect to.</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Redirect(Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);
        return Redirect(HeaderUrl.Encode(url));
    }

    /// <summary>
    /// Enables a client-side full refresh of the page.
    /// </summary>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Refresh()
    {
        AssertIsHtmxRequest();
        _headers[HtmxResponseHeaderNames.Refresh] = "true";
        EmptyResponseBodyRequested = true;
        return this;
    }

    /// <summary>
    /// Replaces the current URL in the location bar.
    /// </summary>
    /// <param name="url"></param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse ReplaceUrl(string url)
    {
        AssertIsHtmxRequest();
        _headers[HtmxResponseHeaderNames.ReplaceUrl] = HeaderUrl.Encode(url);
        return this;
    }

    /// <summary>
    /// Replaces the current URL in the location bar.
    /// </summary>
    /// <param name="url"></param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse ReplaceUrl(Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);
        return ReplaceUrl(HeaderUrl.Encode(url));
    }

    /// <summary>
    /// Allows you to specify how the response will be swapped.
    /// </summary>
    /// <param name="modifier">The hx-swap attributes supports modifiers for changing the behavior of the swap.</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Reswap(string modifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modifier);
        AssertIsHtmxRequest();
        _headers[HtmxResponseHeaderNames.Reswap] = modifier;
        return this;
    }

    /// <summary>
    /// Allows you to specify how the response will be swapped.
    /// </summary>
    /// <param name="swapStyle"></param>
    /// <param name="modifier">The hx-swap attributes supports modifiers for changing the behavior of the swap.</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Reswap(SwapStyle swapStyle, string? modifier = null)
    {
        AssertIsHtmxRequest();

        if (swapStyle is SwapStyle.Default)
        {
            // Default means "keep the requested swap style"; only the modifiers (if any) change.
            if (!string.IsNullOrWhiteSpace(modifier))
                _headers[HtmxResponseHeaderNames.Reswap] = modifier;
            return this;
        }

        var style = swapStyle.ToHtmxString();
        var value = !string.IsNullOrWhiteSpace(modifier)
            ? $"{style} {modifier}"
            : style;

        _headers[HtmxResponseHeaderNames.Reswap] = value;

        return this;
    }

    /// <summary>
    /// Allows you to specify how the response will be swapped.
    /// </summary>
    /// <param></param>
    /// <param name="swapStyle"></param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Reswap(SwapStyleBuilder swapStyle)
    {
        ArgumentNullException.ThrowIfNull(swapStyle);
        AssertIsHtmxRequest();

        _headers[HtmxResponseHeaderNames.Reswap] = swapStyle.Build();

        return this;
    }

    /// <summary>
    /// Sends a new polling tag (<c>HX-PTag</c>) for the hx-ptag extension
    /// (<see cref="HtmxExtension.PTag"/>). The browser sends it back on the next poll
    /// (<see cref="HtmxRequest.PTag"/>); answer 304 when nothing changed.
    /// </summary>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse PTag(string tag)
    {
        AssertIsHtmxRequest();
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);
        _headers[HtmxResponseHeaderNames.PTag] = tag;
        return this;
    }

    /// <summary>
    /// Tells the hx-download extension (<see cref="HtmxExtension.Download"/>) to fetch
    /// <paramref name="url"/> as a file download (<c>HX-Download</c>), for when the file cannot be
    /// streamed as the htmx response itself.
    /// </summary>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Download(string url)
    {
        AssertIsHtmxRequest();
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        _headers[HtmxResponseHeaderNames.Download] = url;
        return this;
    }

    /// <summary>
    /// A CSS selector that updates the target of the content update to a different element on the page.
    /// </summary>
    /// <param name="selector"></param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Retarget(string selector)
    {
        AssertIsHtmxRequest();

        _headers[HtmxResponseHeaderNames.Retarget] = selector;

        return this;
    }

    /// <summary>
    /// A CSS selector that allows you to choose which part of the response is used to be swapped in.
    /// </summary>
    /// <param name="selector"></param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Reselect(string selector)
    {
        AssertIsHtmxRequest();

        _headers[HtmxResponseHeaderNames.Reselect] = selector;

        return this;
    }

    /// <summary>
    /// Triggers a client-side event (<c>HX-Trigger</c>) on the element that issued the request.
    /// </summary>
    /// <remarks>
    /// Calling this more than once adds events in call order. Calling it again for the same event
    /// name (case-sensitive, like DOM event names) replaces that event's earlier detail.
    /// </remarks>
    /// <param name="eventName">The name of the event to trigger.</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Trigger(string eventName)
    {
        AssertIsHtmxRequest();
        MergeTrigger(eventName, null, null);
        return this;
    }

    /// <summary>
    /// Triggers a client-side event (<c>HX-Trigger</c>) with a JSON detail object on the element that
    /// issued the request.
    /// </summary>
    /// <remarks>
    /// Calling this again for the same event name replaces that event's earlier detail.
    /// </remarks>
    /// <param name="eventName">The name of the event to trigger.</param>
    /// <param name="detail">The event detail, serialized to JSON.</param>
    /// <param name="jsonSerializerOptions">Serializer options; defaults to the app's <c>JsonOptions</c>.</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse Trigger<TEventDetail>(string eventName, TEventDetail detail, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        AssertIsHtmxRequest();
        MergeTrigger(eventName, SerializeDetail(detail, jsonSerializerOptions), null);
        return this;
    }

    /// <summary>
    /// Triggers a client-side event (<c>HX-Trigger</c>) on the first element matching
    /// <paramref name="targetSelector"/> instead of the element that issued the request.
    /// </summary>
    /// <param name="eventName">The name of the event to trigger.</param>
    /// <param name="targetSelector">A CSS selector for the element to dispatch the event on.</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse TriggerOn(string eventName, string targetSelector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSelector);
        AssertIsHtmxRequest();
        MergeTrigger(eventName, null, targetSelector);
        return this;
    }

    /// <summary>
    /// Triggers a client-side event (<c>HX-Trigger</c>) with a JSON detail on the first element
    /// matching <paramref name="targetSelector"/>.
    /// </summary>
    /// <remarks>
    /// An object detail gets a <c>target</c> property added. Any other detail is sent as
    /// <c>{"value": detail, "target": ...}</c>, which htmx delivers the same way it delivers a bare
    /// value.
    /// </remarks>
    /// <param name="eventName">The name of the event to trigger.</param>
    /// <param name="targetSelector">A CSS selector for the element to dispatch the event on.</param>
    /// <param name="detail">The event detail, serialized to JSON.</param>
    /// <param name="jsonSerializerOptions">Serializer options; defaults to the app's <c>JsonOptions</c>.</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse TriggerOn<TEventDetail>(string eventName, string targetSelector, TEventDetail detail, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSelector);
        AssertIsHtmxRequest();
        MergeTrigger(eventName, SerializeDetail(detail, jsonSerializerOptions), targetSelector);
        return this;
    }

    /// <summary>
    /// Like <see cref="Redirect(string)"/>, but only for local URLs (<c>/path</c> or <c>~/path</c>).
    /// Use it when the URL comes from user input, such as a <c>returnUrl</c> query value, to avoid
    /// open redirects.
    /// </summary>
    /// <param name="localUrl">A local URL.</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    /// <exception cref="InvalidOperationException">The URL is not local.</exception>
    public HtmxResponse LocalRedirect(string localUrl) => Redirect(RequireLocal(localUrl));

    /// <summary>
    /// Like <see cref="Location(string)"/>, but only for local URLs (<c>/path</c> or <c>~/path</c>).
    /// </summary>
    /// <param name="localUrl">A local URL.</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    /// <exception cref="InvalidOperationException">The URL is not local.</exception>
    public HtmxResponse LocalLocation(string localUrl) => Location(RequireLocal(localUrl));

    /// <summary>
    /// Returns whether <paramref name="url"/> is local to the app: it starts with a single <c>/</c>
    /// (not <c>//</c> or <c>/\</c>) or with <c>~/</c>. Same rules as MVC's <c>Url.IsLocalUrl</c>.
    /// </summary>
    public static bool IsLocalUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return false;

        if (url[0] == '/')
        {
            if (url.Length == 1)
                return true;

            return url[1] != '/' && url[1] != '\\' && !HasControlCharacter(url.AsSpan(1));
        }

        if (url[0] == '~' && url.Length > 1 && url[1] == '/')
        {
            if (url.Length == 2)
                return true;

            return url[2] != '/' && url[2] != '\\' && !HasControlCharacter(url.AsSpan(2));
        }

        return false;

        static bool HasControlCharacter(ReadOnlySpan<char> value)
        {
            foreach (var ch in value)
            {
                if (char.IsControl(ch))
                    return true;
            }

            return false;
        }
    }

    private string RequireLocal(string localUrl)
    {
        if (!IsLocalUrl(localUrl))
            throw new InvalidOperationException($"'{localUrl}' is not a local URL.");

        return localUrl.StartsWith("~/", StringComparison.Ordinal)
            ? context.Request.PathBase.Add(new PathString(localUrl[1..])).ToString()
            : localUrl;
    }

    /// <summary>
    /// Marks the response as a validation failure: status 422, which htmx 4 swaps like any other
    /// response. Re-render the form (with its errors) as the response body.
    /// </summary>
    /// <remarks>
    /// Without arguments the form replaces the request's normal target. Pass
    /// <paramref name="retargetSelector"/> and/or <paramref name="reswap"/> to send the errors
    /// elsewhere, or let the page decide with <c>hx-status:422="..."</c> (see <see cref="HtmxStatus"/>).
    /// </remarks>
    /// <param name="retargetSelector">Optional CSS selector to swap into instead (<c>HX-Retarget</c>).</param>
    /// <param name="reswap">Optional swap style override (<c>HX-Reswap</c>).</param>
    /// <returns>This <see cref="HtmxResponse"/> object instance.</returns>
    public HtmxResponse ValidationFailed(string? retargetSelector = null, SwapStyle reswap = SwapStyle.Default)
    {
        AssertIsHtmxRequest();
        context.Response.StatusCode = HtmxStatus.ValidationFailed;

        if (!string.IsNullOrWhiteSpace(retargetSelector))
            _headers[HtmxResponseHeaderNames.Retarget] = retargetSelector;

        if (reswap != SwapStyle.Default)
            _headers[HtmxResponseHeaderNames.Reswap] = reswap.ToHtmxString();

        return this;
    }

    private JsonNode? SerializeDetail<TEventDetail>(TEventDetail detail, JsonSerializerOptions? jsonSerializerOptions)
    {
        if (detail is null)
            return null;

        jsonSerializerOptions ??= context.RequestServices?.GetService<IOptions<JsonOptions>>()?.Value.SerializerOptions
                                  ?? JsonSerializerOptions.Web;

        return JsonSerializer.SerializeToNode(detail, jsonSerializerOptions);
    }

    private void MergeTrigger(string eventName, JsonNode? detail, string? targetSelector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventName);

        var itemsKey = ItemsKeyPrefix + HtmxResponseHeaderNames.Trigger;
        if (!context.Items.TryGetValue(itemsKey, out var current) || current is not List<TriggerEvent> events)
        {
            events = [];
            context.Items[itemsKey] = events;
        }

        if (targetSelector is not null)
        {
            detail = detail is JsonObject obj
                ? obj
                : detail is null ? new JsonObject() : new JsonObject { ["value"] = detail };
            ((JsonObject)detail)["target"] = targetSelector;
        }

        var index = events.FindIndex(e => string.Equals(e.Name, eventName, StringComparison.Ordinal));
        if (index >= 0)
            events[index] = new TriggerEvent(eventName, detail);
        else
            events.Add(new TriggerEvent(eventName, detail));

        _headers[HtmxResponseHeaderNames.Trigger] = BuildTriggerHeader(events);
    }

    private static string BuildTriggerHeader(List<TriggerEvent> events)
    {
        // Plain comma-separated names are only valid when no event has a detail and no name
        // contains characters that the comma form cannot carry.
        if (events.TrueForAll(e => e.Detail is null && e.Name.AsSpan().IndexOfAny(",{\"\'") < 0 && e.Name.Trim() == e.Name))
            return string.Join(',', events.Select(e => e.Name));

        var buffer = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions { Encoder = JavaScriptEncoder.Default }))
        {
            writer.WriteStartObject();
            foreach (var e in events)
            {
                writer.WritePropertyName(e.Name);
                if (e.Detail is null)
                    writer.WriteNullValue();
                else
                    e.Detail.WriteTo(writer);
            }
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    private readonly record struct TriggerEvent(string Name, JsonNode? Detail);

    private void AssertIsHtmxRequest()
    {
        if (!_isHtmxRequest)
        {
            throw new InvalidOperationException(
                "The active request is not an htmx request. " +
                "Setting response headers during request has no effect.");
        }
    }
}
