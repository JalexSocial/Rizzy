(function () {
    const root = window.Rizzy = window.Rizzy || {};
    const nonceApi = root.nonce = root.nonce || {};

    nonceApi.getDocumentNonce = function getDocumentNonce() {
        return htmx.config.documentNonce ?? htmx.config.inlineScriptNonce ?? "";
    };

    nonceApi.getResponseNonce = function getResponseNonce(responseOrHeaders) {
        const headers = responseOrHeaders?.headers?.get ? responseOrHeaders.headers : responseOrHeaders;
        let nonce = headers?.get?.("HX-Nonce") || "";
        if (nonce) return nonce;

        const csp = headers?.get?.("content-security-policy") || "";
        if (csp) {
            const cspMatch = csp.match(/(style|script)-src[^;]*'nonce-([^']*)'/i);
            if (cspMatch) return cspMatch[2];
        }

        return "";
    };

    nonceApi.processUnsafeHtml = function processUnsafeHtml(text, documentNonce, responseNonce) {
        if (typeof text !== "string") return "";

        if (documentNonce && responseNonce) {
            text = text.replaceAll(responseNonce, documentNonce);
        }

        try {
            const doc = new DOMParser().parseFromString(text, "text/html");
            const elements = doc.querySelectorAll("script, style, link");

            elements.forEach((elt) => {
                const nonce = elt.getAttribute("nonce");
                if (nonce !== documentNonce) {
                    elt.remove();
                }
            });

            return doc.documentElement.outerHTML;
        } catch (_) {
            return "";
        }
    };

    nonceApi.processResponseHtml = function processResponseHtml(text, responseOrHeaders) {
        const documentNonce = nonceApi.getDocumentNonce();
        const responseNonce = nonceApi.getResponseNonce(responseOrHeaders);
        return nonceApi.processUnsafeHtml(text, documentNonce, responseNonce);
    };

    htmx.registerExtension("rizzy-nonce", {
        htmx_after_request: function (_elt, detail) {
            if (typeof detail?.ctx?.text === "string") {
                detail.ctx.text = nonceApi.processResponseHtml(detail.ctx.text, detail?.ctx?.response);
            }
            return true;
        }
    });
})();
