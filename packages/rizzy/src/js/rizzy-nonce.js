(function () {
    function processUnsafeHtml(text, documentNonce, newScriptNonce) {
        if (documentNonce && newScriptNonce) {
            text = text.replaceAll(newScriptNonce, documentNonce);
        }

        const parser = new DOMParser();

        try {
            const doc = parser.parseFromString(text, "text/html");

            if (doc) {
                const elements = doc.querySelectorAll("script, style, link");

                elements.forEach((elt) => {
                    const nonce = elt.getAttribute("nonce");
                    if (nonce !== documentNonce) {
                        elt.remove();
                    }
                });

                return doc.documentElement.outerHTML;
            }
        } catch (_) {
        }

        return "";
    }

    htmx.registerExtension("rizzy-nonce", {
        htmx_after_request: function (_elt, detail) {
            let documentNonce = htmx.config.documentNonce ?? htmx.config.inlineScriptNonce;
            if (!documentNonce) {
                // console.warn("rizzy-nonce extension loaded but no nonce found for document. Inline scripts may be blocked.");
                documentNonce = "";
            }

            let nonce = detail?.ctx?.response?.headers?.get("HX-Nonce");
            if (!nonce) {
                const csp = detail?.ctx?.response?.headers?.get("content-security-policy");
                if (csp) {
                    const cspMatch = csp.match(/(style|script)-src[^;]*'nonce-([^']*)'/i);
                    if (cspMatch) {
                        nonce = cspMatch[2];
                    }
                }
            }

            nonce ??= "";

            if (typeof detail?.ctx?.text === "string") {
                detail.ctx.text = processUnsafeHtml(detail.ctx.text, documentNonce, nonce);
            }

            return true;
        }
    });
})();
