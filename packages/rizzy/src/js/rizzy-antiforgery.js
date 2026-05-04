// Rizzy Antiforgery HTMX 4 Extension
// - Injects an ASP.NET antiforgery token into non-GET requests.
// - On boosted (full-page) navigations, refreshes htmx.config.antiforgery from the response's
//   <meta name="htmx-config" content="..."> so subsequent requests use the latest token.
//
// Expected htmx-config shape (example):
// <meta name="htmx-config" content='{"antiforgery":{"headerName":"RequestVerificationToken","requestToken":"...","formFieldName":"__RequestVerificationToken"}}' />

(() => {
    // Avoid duplicate registration if bundled multiple times
    const EXT_NAME = "rizzy-antiforgery";

    htmx.registerExtension(EXT_NAME, {
        /**
         * htmx:config:request
         * Runs after htmx has built ctx.request (method/action/headers/body), but before it finalizes
         * query-param conversions (GET/DELETE) and before the request is issued.
         *
         * Goal: Add antiforgery token to request headers or body.
         */
        htmx_config_request: (elt, detail) => {
            const ctx = detail?.ctx;
            const req = ctx?.request;
            if (!req) return;

            // Skip safe/idempotent methods that should not include antiforgery.
            // (HEAD is uncommon here but treat it like GET.)
            const method = (req.method || "").toUpperCase();
            if (method === "GET" || method === "HEAD") return;

            const antiforgery = htmx.config?.antiforgery;
            if (!antiforgery) return;

            const { headerName, requestToken, formFieldName } = antiforgery;

            // Require a token value to inject.
            if (!requestToken) return;

            // Require at least one injection strategy.
            if (!headerName && !formFieldName) return;

            // Prefer header injection when configured because it works for:
            // - POST/PUT/PATCH
            // - DELETE (which htmx may treat as query-param request)
            // - requests where body is null or transformed
            if (headerName) {
                req.headers ||= {};

                // Support both plain-object headers and Headers instances.
                if (typeof req.headers.set === "function") {
                    // Headers instance
                    req.headers.set(headerName, requestToken);
                } else {
                    // Plain object
                    req.headers[headerName] = requestToken;
                }

                return;
            }

            // Body injection fallback (only if headerName is NOT configured):
            //
            // IMPORTANT: In htmx 4 DELETE is treated like GET for query-param conversion.
            // If we append a token to the body here, htmx may later move it into the query string.
            // So only do body injection for methods that actually send a body.
            if (method !== "POST" && method !== "PUT" && method !== "PATCH") return;

            const body = req.body;
            if (!body) return;

            // If the token is already present, do not override it.
            if (typeof body.has === "function" && body.has(formFieldName)) return;

            // Append for FormData / URLSearchParams
            if (typeof body.append === "function") {
                body.append(formFieldName, requestToken);
            } else if (typeof body.set === "function") {
                // URLSearchParams also supports set()
                body.set(formFieldName, requestToken);
            }
        },

        /**
         * htmx:after:swap
         * Runs after htmx has performed the swap. This is a safer point than after:request
         * because swaps can be cancelled or bypassed by headers (redirect/refresh) or status rules.
         *
         * Goal: If this was a boosted (full-page-ish) navigation, refresh antiforgery config
         * from the server-provided meta config in the response text.
         */
        htmx_after_swap: (elt, detail) => {
            const ctx = detail?.ctx;
            if (!ctx?.text) return;

            // Boosted detection:
            // Prefer the internal "boosted" marker on the source element.
            // Fall back to the HX-Boosted header if needed.
            const sourceBoosted = !!ctx.sourceElement?._htmx?.boosted;
            const headerBoosted = ctx.request?.headers?.["HX-Boosted"] === "true";
            const boosted = sourceBoosted || headerBoosted;

            if (!boosted) return;

            // Parse the response HTML to extract the meta htmx-config.
            // htmx itself strips <head> from swaps, so we must read it from ctx.text.
            let doc;
            try {
                doc = new DOMParser().parseFromString(ctx.text, "text/html");
            } catch {
                return;
            }

            const meta = doc.querySelector('meta[name="htmx-config"]');
            const contentValue = meta?.getAttribute("content");
            if (!contentValue) return;

            // Update only the antiforgery portion of runtime config.
            try {
                const parsed = JSON.parse(contentValue);
                const newAnti = parsed?.antiforgery;
                if (!newAnti) return;

                htmx.config.antiforgery = newAnti;
            } catch (e) {
                console.error("[rizzy-antiforgery] Error parsing htmx-config JSON:", e);
                return;
            }

            const current = document.querySelector('meta[name="htmx-config"]');
            if (current) {
                current.setAttribute("content", contentValue);
            } else {
                const newMeta = document.createElement("meta");
                newMeta.setAttribute("name", "htmx-config");
                newMeta.setAttribute("content", contentValue);
                document.head.appendChild(newMeta);
            }
        }
    });
})();