namespace Rizzy.Htmx;

/// <summary>
/// Names of the events htmx 4, its official extensions and Rizzy dispatch. Use them with
/// <c>Trigger.OnEvent(...)</c>, <c>Hx.On(...)</c>, <c>HtmxResponse.Trigger(...)</c> or
/// <c>addEventListener</c>.
/// </summary>
/// <remarks>
/// When <see cref="HtmxConfig.MetaCharacter"/> is set, htmx dispatches event names with <c>:</c>
/// replaced by that character; <see cref="WithMetaCharacter"/> converts a name.
/// Checked against htmx's source by <c>packages/rizzy/tests/htmx4-coverage.test.mjs</c>.
/// </remarks>
public static class HtmxEvents
{
    /// <summary><c>htmx:abort</c></summary>
    public const string Abort = "htmx:abort";
    /// <summary><c>htmx:after:cleanup</c></summary>
    public const string AfterCleanup = "htmx:after:cleanup";
    /// <summary><c>htmx:after:history:push</c></summary>
    public const string AfterHistoryPush = "htmx:after:history:push";
    /// <summary><c>htmx:after:history:replace</c></summary>
    public const string AfterHistoryReplace = "htmx:after:history:replace";
    /// <summary><c>htmx:after:history:update</c></summary>
    public const string AfterHistoryUpdate = "htmx:after:history:update";
    /// <summary><c>htmx:after:implicitInheritance</c></summary>
    public const string AfterImplicitInheritance = "htmx:after:implicitInheritance";
    /// <summary><c>htmx:after:init</c></summary>
    public const string AfterInit = "htmx:after:init";
    /// <summary><c>htmx:after:process</c></summary>
    public const string AfterProcess = "htmx:after:process";
    /// <summary><c>htmx:after:request</c></summary>
    public const string AfterRequest = "htmx:after:request";
    /// <summary><c>htmx:after:settle</c></summary>
    public const string AfterSettle = "htmx:after:settle";
    /// <summary><c>htmx:after:swap</c></summary>
    public const string AfterSwap = "htmx:after:swap";
    /// <summary><c>htmx:after:viewTransition</c></summary>
    public const string AfterViewTransition = "htmx:after:viewTransition";
    /// <summary><c>htmx:before:cleanup</c></summary>
    public const string BeforeCleanup = "htmx:before:cleanup";
    /// <summary><c>htmx:before:history:restore</c></summary>
    public const string BeforeHistoryRestore = "htmx:before:history:restore";
    /// <summary><c>htmx:before:history:update</c></summary>
    public const string BeforeHistoryUpdate = "htmx:before:history:update";
    /// <summary><c>htmx:before:init</c></summary>
    public const string BeforeInit = "htmx:before:init";
    /// <summary><c>htmx:before:morph:attr</c></summary>
    public const string BeforeMorphAttr = "htmx:before:morph:attr";
    /// <summary><c>htmx:before:morph:node</c></summary>
    public const string BeforeMorphNode = "htmx:before:morph:node";
    /// <summary><c>htmx:before:on:init</c></summary>
    public const string BeforeOnInit = "htmx:before:on:init";
    /// <summary><c>htmx:before:process</c></summary>
    public const string BeforeProcess = "htmx:before:process";
    /// <summary><c>htmx:before:request</c></summary>
    public const string BeforeRequest = "htmx:before:request";
    /// <summary><c>htmx:before:response</c></summary>
    public const string BeforeResponse = "htmx:before:response";
    /// <summary><c>htmx:before:settle</c></summary>
    public const string BeforeSettle = "htmx:before:settle";
    /// <summary><c>htmx:before:swap</c></summary>
    public const string BeforeSwap = "htmx:before:swap";
    /// <summary><c>htmx:before:viewTransition</c></summary>
    public const string BeforeViewTransition = "htmx:before:viewTransition";
    /// <summary><c>htmx:config:request</c></summary>
    public const string ConfigRequest = "htmx:config:request";
    /// <summary><c>htmx:confirm</c></summary>
    public const string Confirm = "htmx:confirm";
    /// <summary><c>htmx:error</c></summary>
    public const string Error = "htmx:error";
    /// <summary><c>htmx:finally:request</c></summary>
    public const string FinallyRequest = "htmx:finally:request";
    /// <summary><c>htmx:finally:swap</c></summary>
    public const string FinallySwap = "htmx:finally:swap";
    /// <summary><c>htmx:response:error</c></summary>
    public const string ResponseError = "htmx:response:error";
    /// <summary><c>htmx:scope</c></summary>
    public const string Scope = "htmx:scope";

    /// <summary>Prefix of <c>htmx:process:&lt;type&gt;</c>, dispatched for each <c>&lt;hx-*&gt;</c> template element (for example <c>htmx:process:partial</c>).</summary>
    public const string ProcessPrefix = "htmx:process:";

    /// <summary>The <c>htmx:process:&lt;type&gt;</c> event for an <c>&lt;hx-type&gt;</c> element, for example <c>Process("partial")</c>.</summary>
    public static string Process(string type) => ProcessPrefix + type;

    /// <summary>Returns <paramref name="eventName"/> with <c>:</c> replaced by <paramref name="metaCharacter"/> (no change when it is null or empty).</summary>
    public static string WithMetaCharacter(string eventName, string? metaCharacter)
        => string.IsNullOrEmpty(metaCharacter) ? eventName : eventName.Replace(":", metaCharacter, StringComparison.Ordinal);

    /// <summary>Events of the <c>hx-csp</c> extension (<see cref="HtmxExtension.Csp"/>).</summary>
    public static class Csp
    {
        /// <summary><c>htmx:security:strip</c></summary>
        public const string Strip = "htmx:security:strip";
        /// <summary><c>htmx:security:violation</c></summary>
        public const string Violation = "htmx:security:violation";
    }

    /// <summary>Events of the <c>hx-download</c> extension (<see cref="HtmxExtension.Download"/>).</summary>
    public static class Download
    {
        /// <summary><c>htmx:download:complete</c></summary>
        public const string Complete = "htmx:download:complete";
        /// <summary><c>htmx:download:progress</c></summary>
        public const string Progress = "htmx:download:progress";
        /// <summary><c>htmx:download:start</c></summary>
        public const string Start = "htmx:download:start";
    }

    /// <summary>Events of the <c>hx-head</c> extension (<see cref="HtmxExtension.Head"/>).</summary>
    public static class Head
    {
        /// <summary><c>htmx:head:after:merge</c></summary>
        public const string AfterMerge = "htmx:head:after:merge";
        /// <summary><c>htmx:head:before:add</c></summary>
        public const string BeforeAdd = "htmx:head:before:add";
        /// <summary><c>htmx:head:before:merge</c></summary>
        public const string BeforeMerge = "htmx:head:before:merge";
        /// <summary><c>htmx:head:before:remove</c></summary>
        public const string BeforeRemove = "htmx:head:before:remove";
    }

    /// <summary>Events of the <c>hx-history-cache</c> extension (<see cref="HtmxExtension.HistoryCache"/>).</summary>
    public static class HistoryCache
    {
        /// <summary><c>htmx:history:cache:after:restore</c></summary>
        public const string AfterRestore = "htmx:history:cache:after:restore";
        /// <summary><c>htmx:history:cache:after:save</c></summary>
        public const string AfterSave = "htmx:history:cache:after:save";
        /// <summary><c>htmx:history:cache:before:restore</c></summary>
        public const string BeforeRestore = "htmx:history:cache:before:restore";
        /// <summary><c>htmx:history:cache:before:save</c></summary>
        public const string BeforeSave = "htmx:history:cache:before:save";
        /// <summary><c>htmx:history:cache:hit</c></summary>
        public const string Hit = "htmx:history:cache:hit";
        /// <summary><c>htmx:history:cache:miss</c></summary>
        public const string Miss = "htmx:history:cache:miss";
    }

    /// <summary>Events of the <c>hx-multipart</c> extension (<see cref="HtmxExtension.Multipart"/>).</summary>
    public static class Multipart
    {
        /// <summary><c>htmx:multipart:after:connection</c></summary>
        public const string AfterConnection = "htmx:multipart:after:connection";
        /// <summary><c>htmx:multipart:after:part</c></summary>
        public const string AfterPart = "htmx:multipart:after:part";
        /// <summary><c>htmx:multipart:before:connection</c></summary>
        public const string BeforeConnection = "htmx:multipart:before:connection";
        /// <summary><c>htmx:multipart:before:part</c></summary>
        public const string BeforePart = "htmx:multipart:before:part";
        /// <summary><c>htmx:multipart:close</c></summary>
        public const string Close = "htmx:multipart:close";
        /// <summary><c>htmx:multipart:error</c></summary>
        public const string Error = "htmx:multipart:error";
    }

    /// <summary>Events of the <c>hx-prompt</c> extension (<see cref="HtmxExtension.Prompt"/>).</summary>
    public static class Prompt
    {
        /// <summary><c>htmx:prompt</c></summary>
        public const string Answered = "htmx:prompt";
    }

    /// <summary>Events of the <c>hx-sse</c> extension (<see cref="HtmxExtension.Sse"/>).</summary>
    public static class Sse
    {
        /// <summary><c>htmx:sse:after:connection</c></summary>
        public const string AfterConnection = "htmx:sse:after:connection";
        /// <summary><c>htmx:sse:after:message</c></summary>
        public const string AfterMessage = "htmx:sse:after:message";
        /// <summary><c>htmx:sse:before:connection</c></summary>
        public const string BeforeConnection = "htmx:sse:before:connection";
        /// <summary><c>htmx:sse:before:message</c></summary>
        public const string BeforeMessage = "htmx:sse:before:message";
        /// <summary><c>htmx:sse:close</c></summary>
        public const string Close = "htmx:sse:close";
        /// <summary><c>htmx:sse:error</c></summary>
        public const string Error = "htmx:sse:error";
    }

    /// <summary>Events of the <c>hx-ws</c> extension (<see cref="HtmxExtension.Ws"/>).</summary>
    public static class Ws
    {
        /// <summary><c>htmx:ws:after:connection</c></summary>
        public const string AfterConnection = "htmx:ws:after:connection";
        /// <summary><c>htmx:ws:after:message:incoming</c></summary>
        public const string AfterMessageIncoming = "htmx:ws:after:message:incoming";
        /// <summary><c>htmx:ws:after:message:outgoing</c></summary>
        public const string AfterMessageOutgoing = "htmx:ws:after:message:outgoing";
        /// <summary><c>htmx:ws:before:connection</c></summary>
        public const string BeforeConnection = "htmx:ws:before:connection";
        /// <summary><c>htmx:ws:before:message:incoming</c></summary>
        public const string BeforeMessageIncoming = "htmx:ws:before:message:incoming";
        /// <summary><c>htmx:ws:before:message:outgoing</c></summary>
        public const string BeforeMessageOutgoing = "htmx:ws:before:message:outgoing";
        /// <summary><c>htmx:ws:close</c></summary>
        public const string Close = "htmx:ws:close";
        /// <summary><c>htmx:ws:error</c></summary>
        public const string Error = "htmx:ws:error";
    }

    /// <summary>Events dispatched by Rizzy (rizzy.js and <c>UseHtmxErrorHandling</c>).</summary>
    public static class Rizzy
    {
        /// <summary><c>htmx:rizzy:stream:open</c>: A streaming response started delivering <c>&lt;blazor-ssr&gt;</c> updates.</summary>
        public const string StreamOpen = "htmx:rizzy:stream:open";
        /// <summary><c>htmx:rizzy:stream:block</c>: A streamed update block was applied.</summary>
        public const string StreamBlock = "htmx:rizzy:stream:block";
        /// <summary><c>htmx:rizzy:stream:close</c>: A streaming response ended.</summary>
        public const string StreamClose = "htmx:rizzy:stream:close";
        /// <summary><c>htmx:rizzy:config:warn</c>: rizzy.js found a configuration problem (detail.warn).</summary>
        public const string ConfigWarn = "htmx:rizzy:config:warn";
        /// <summary><c>rz:confirm-dialog</c>: An <c>hx-confirm</c> dialog is requested; a custom dialog handles it.</summary>
        public const string ConfirmDialog = "rz:confirm-dialog";
        /// <summary><c>rz:server-error</c>: <c>UseHtmxErrorHandling</c> kept a 5xx response out of the target (detail.status).</summary>
        public const string ServerError = "rz:server-error";
    }
}
