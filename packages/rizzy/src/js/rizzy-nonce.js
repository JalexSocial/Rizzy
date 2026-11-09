// Rizzy CSP nonce extension for htmx 4 (RIZZY-004, RIZZY-027)
//
// Every Rizzy response is rendered with its own CSP nonce (sent in the HX-Nonce header). Scripts
// and styles in a swapped fragment carry that response nonce, but the page only trusts the nonce
// it was loaded with. This extension rewrites the response nonce to the page nonce in the raw
// response text, so the browser's own CSP decides what may run:
//
// - Server-rendered scripts/styles (response nonce) are promoted to the page nonce and run.
// - Injected markup without a nonce stays without one and is blocked by a strict CSP.
// - Any copy of the page nonce already present in a response is stripped: the server never
//   knows the page nonce, so its presence means it was stolen and echoed back.
//
// The response is never parsed or re-serialized here, so table rows, full documents and
// un-nonced <style>/<link> elements (when no CSP is in use) swap exactly as htmx would swap
// them without Rizzy. When the page has no nonce, the extension does nothing.
//
// The page nonce is read from the nonce *property* of a <script nonce> element, which browsers
// keep readable after hiding the attribute (unlike a value in the htmx-config meta tag, which
// CSS attribute selectors can read). htmx.config.documentNonce is still honored for pages
// rendered by older Rizzy servers.
//
// Leave htmx.config.inlineScriptNonce unset when using this extension: htmx core stamps that
// nonce on *every* script it inserts, which would also trust injected scripts.

(function () {
    const root = window.Rizzy = window.Rizzy || {};
    const nonceApi = root.nonce = root.nonce || {};

    let cachedPageNonce;

    function escapeRegex(value) {
        return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    }

    /** The nonce the current page was loaded with, or "" when the page uses no nonce. */
    nonceApi.getDocumentNonce = function getDocumentNonce() {
        if (cachedPageNonce) return cachedPageNonce;

        const fromScript = document.querySelector("script[nonce]")?.nonce
            || document.querySelector("script[nonce]")?.getAttribute("nonce")
            || "";
        const fromConfig = (typeof htmx !== "undefined" && htmx.config?.documentNonce) || "";

        cachedPageNonce = fromScript || fromConfig || "";
        return cachedPageNonce;
    };

    /** The nonce a response was rendered with (HX-Nonce header, else the CSP header). */
    nonceApi.getResponseNonce = function getResponseNonce(responseOrHeaders) {
        const headers = responseOrHeaders?.headers?.get ? responseOrHeaders.headers : responseOrHeaders;
        const nonce = headers?.get?.("HX-Nonce") || "";
        if (nonce) return nonce;

        const csp = headers?.get?.("Content-Security-Policy") || "";
        return csp.match(/(?:script-src|style-src|default-src)[^;]*'nonce-([^']+)'/i)?.[1] || "";
    };

    /**
     * Rewrites nonce attributes in raw HTML: strips any stolen page nonce, then promotes the
     * response nonce to the page nonce. Returns the text unchanged when the page has no nonce.
     */
    nonceApi.rewriteNonces = function rewriteNonces(text, documentNonce, responseNonce) {
        if (typeof text !== "string" || !text || !documentNonce) return text ?? "";

        const attr = (value) => new RegExp(`(\\snonce\\s*=\\s*)(["']?)${escapeRegex(value)}\\2(?=[\\s/>])`, "gi");

        // 1. Stolen page nonce: remove the attribute entirely.
        if (responseNonce !== documentNonce) {
            text = text.replace(attr(documentNonce), "");
        }

        // 2. Response nonce: promote to the page nonce.
        if (responseNonce && responseNonce !== documentNonce) {
            text = text.replace(attr(responseNonce), (_, prefix, quote) => `${prefix}${quote}${documentNonce}${quote}`);
        }

        return text;
    };

    /** Applies nonce rewriting to a response body using the page and response nonces. */
    nonceApi.processResponseHtml = function processResponseHtml(text, responseOrHeaders) {
        return nonceApi.rewriteNonces(text, nonceApi.getDocumentNonce(), nonceApi.getResponseNonce(responseOrHeaders));
    };

    htmx.registerExtension("rizzy-nonce", {
        htmx_after_request(_elt, detail) {
            const ctx = detail?.ctx;
            if (typeof ctx?.text === "string") {
                ctx.text = nonceApi.processResponseHtml(ctx.text, ctx.response);
            }
        }
    });
})();
