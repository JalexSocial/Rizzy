using System.Diagnostics.CodeAnalysis;

namespace Rizzy.Htmx;

/// <summary>
/// Useful string constants in Htmx components.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Only containers for constants here. Nothing to see!")]
internal static class Constants
{
    /// <summary>
    /// Default antiforgery cookie name
    /// </summary>
	public const string AntiforgeryCookieName = "HX-XSRF-TOKEN";

    internal static class Nonce
    {
        /// <summary>
        /// Nonce headers for use with HTMX
        /// </summary>
        public const string NonceResponseHeader = "HX-Nonce";
    }

    internal static class HttpContextKeys
    {
        public const string HtmxRequestKey = "Rizzy.Http:HtmxRequest";
        public const string FormFieldMappingsKey = "Rizzy.Http:AllFieldMappings";
    }

    /// <summary>
    /// Htmx attribute names.
    /// </summary>
    public static class Attributes
    {
        internal const string Prefix = "hx-";

        public const string Id = "id";

        public const string HxGet = "hx-get";
        public const string HxPost = "hx-post";
        public const string HxPut = "hx-put";
        public const string HxDelete = "hx-delete";
        public const string HxPatch = "hx-patch";
        public const string HxTrigger = "hx-trigger";
        public const string HxTarget = "hx-target";
        public const string HxSwap = "hx-swap";
        public const string HxHeaders = "hx-headers";
    }

    /// <summary>
    /// Htmx trigger values for <c>hx-trigger</c>.
    /// </summary>
    public static class Triggers
    {
        public const string Every = "every";
        public const string Intersect = "intersect";
        public const string Load = "load";
        public const string Revealed = "revealed";
        public const string Sse = "sse";
    }

    /// <summary>
    /// Htmx trigger modifier values for <c>hx-trigger</c>.
    /// </summary>
    public static class TriggerModifiers
    {
        public const string SseEvent = "sseEvent";
        public const string Once = "once";
        public const string Changed = "changed";
        public const string Delay = "delay";
        public const string Throttle = "throttle";
        public const string From = "from";
        public const string Target = "target";
        public const string Consume = "consume";
        public const string Queue = "queue";
        public const string Root = "root";
        public const string Threshold = "threshold";
    }

    /// <summary>
    /// Htmx swap style values.
    /// </summary>
    public static class SwapStyles
    {
        public const string InnerHTML = "innerHTML";
        public const string OuterHTML = "outerHTML";
        public const string BeforeBegin = "beforebegin";
        public const string AfterBegin = "afterbegin";
        public const string BeforeEnd = "beforeend";
        public const string AfterEnd = "afterend";
        public const string Delete = "delete";
        public const string None = "none";
        public const string Default = "";
    }
}
