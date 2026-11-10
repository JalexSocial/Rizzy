// Rizzy client diagnostics (RIZZY-026, RIZZY-028, RM-D3)
//
// 1. Error reporting that follows htmx 4's logging convention: an event whose detail carries
//    `error` is logged with console.error, one carrying `warn` with console.warn, then the event
//    is dispatched. htmx core applies this convention only to events it triggers itself;
//    htmx.trigger() (the public API extensions use) dispatches without logging, so Rizzy
//    extensions report through Rizzy.diagnostics.report() instead.
//
// 2. A page check when the bundle loads. It warns once, on the console, when:
//    - the page has no <meta name="htmx-config"> (the root layout does not render
//      <RzHeadOutlet />, so the antiforgery token and Rizzy settings are missing), or
//    - the meta tag is not valid JSON, or
//    - the server's Rizzy major version (rizzyVersion in the meta tag) differs from the major
//      version this script was built for (for example a cached or CDN copy of rizzy.js), or
//    - htmx rejected an extension because the htmx.config.extensions allow-list does not name it,
//      or an extension enabled on the server (rizzyExtensions in the meta tag) never registered.
//
// For the extension check, this script wraps htmx.registerExtension before Rizzy's own
// extensions register. <RzScripts /> loads rizzy.js before the extension scripts, so every
// registration is seen.
//
// Set htmx.config.rizzyDiagnostics = false (in the meta config) to silence the page check.

(function () {
    const root = window.Rizzy = window.Rizzy || {};
    const diagnostics = root.diagnostics = root.diagnostics || {};

    // Major version of Rizzy this script belongs to. Keep in step with <Version> in
    // Directory.Build.props; tests/diagnostics.test.mjs fails when they differ.
    const CLIENT_MAJOR = 6;
    diagnostics.clientMajor = CLIENT_MAJOR;

    /**
     * Logs per htmx's convention (detail.error → console.error, detail.warn → console.warn) and
     * dispatches the event on elt (or document). Returns htmx.trigger's result.
     */
    diagnostics.report = function report(elt, eventName, detail = {}) {
        if (detail.error) {
            const message = detail.error.message ?? detail.error;
            console.error(`htmx: ${eventName}: ${message}`, detail.error, { elt, detail });
        } else if (detail.warn) {
            console.warn(`htmx: ${eventName}: ${detail.warn}`, { elt, detail });
        }

        if (typeof htmx === "undefined") return true;
        return htmx.trigger(elt && elt.isConnected ? elt : document, eventName, detail);
    };

    // ---- Extension registration tracking --------------------------------------------------------
    const registered = diagnostics.registeredExtensions = diagnostics.registeredExtensions || new Set();
    const rejected = diagnostics.rejectedExtensions = diagnostics.rejectedExtensions || new Set();

    if (typeof htmx !== "undefined" && typeof htmx.registerExtension === "function" && !htmx.registerExtension._rizzyTracked) {
        const original = htmx.registerExtension.bind(htmx);
        const tracked = function registerExtension(name, extension) {
            const result = original(name, extension);
            if (result === false) {
                // htmx returns false for a name missing from a non-empty allow-list, and for a
                // duplicate registration (including one registered before rizzy.js loaded).
                const allowList = String(htmx.config?.extensions ?? "")
                    .split(",").map((n) => n.trim()).filter(Boolean);
                if (allowList.length > 0 && !allowList.includes(name)) {
                    rejected.add(name);
                } else {
                    registered.add(name);   // a duplicate: the extension is registered
                }
            } else {
                registered.add(name);
                rejected.delete(name);
            }
            return result;
        };
        tracked._rizzyTracked = true;
        htmx.registerExtension = tracked;
    }

    function majorOf(version) {
        const match = /^(\d+)\./.exec(String(version ?? "").trim());
        return match ? Number(match[1]) : null;
    }

    /**
     * Inspects the page's Rizzy configuration and returns the warnings found (an array of
     * strings). Each warning is reported once per page as htmx:rizzy:config:warn.
     */
    diagnostics.checkPage = function checkPage(doc = document) {
        const warnings = [];
        const meta = doc.querySelector('meta[name="htmx-config"]');

        if (!meta) {
            warnings.push(
                'Rizzy: no <meta name="htmx-config"> on this page, so htmx has no antiforgery token or ' +
                'Rizzy settings. Render <RzHeadOutlet /> instead of <HeadOutlet /> in the <head> of your ' +
                'root layout, before the htmx script (Rizzy 6 no longer adds the tag through HeadContent).');
        } else {
            let config = null;
            try {
                config = JSON.parse(meta.getAttribute("content") || "");
            } catch {
                warnings.push('Rizzy: <meta name="htmx-config"> does not contain valid JSON; htmx ignores it.');
            }

            if (config?.rizzyDiagnostics === false) return [];

            for (const name of rejected) {
                warnings.push(
                    `Rizzy: htmx rejected the "${name}" extension because htmx.config.extensions ` +
                    `("${config?.extensions ?? ""}") does not list it. Enable extensions with ` +
                    `HtmxConfig.UseExtensions(...), which keeps the allow-list in step, or add the name ` +
                    `to HtmxConfig.Extensions.`);
            }
            for (const name of config?.rizzyExtensions ?? []) {
                if (!registered.has(name) && !rejected.has(name)) {
                    warnings.push(
                        `Rizzy: the "${name}" extension is enabled on the server but rizzy.js did not see ` +
                        `it register. Render <RzScripts /> in the root layout (after <RzHeadOutlet />). ` +
                        `If you load the scripts yourself, load the extension after htmx and after ` +
                        `rizzy.js: an extension registered before rizzy.js is not tracked and is ` +
                        `reported here even though it works.`);
                }
            }

            const serverMajor = majorOf(config?.rizzyVersion);
            if (serverMajor !== null && serverMajor !== CLIENT_MAJOR) {
                warnings.push(
                    `Rizzy: the server runs Rizzy ${config.rizzyVersion} but rizzy.js is for Rizzy ` +
                    `${CLIENT_MAJOR}.x. Load the rizzy.js shipped with the Rizzy package ` +
                    `(_content/Rizzy/js/rizzy.js) or the matching @jalexsocial/rizzy version.`);
            }
        }

        for (const warn of warnings) {
            diagnostics.report(doc, "htmx:rizzy:config:warn", { warn });
        }
        return warnings;
    };

    if (typeof document !== "undefined" && !diagnostics.pageChecked) {
        diagnostics.pageChecked = true;
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", () => diagnostics.checkPage(document), { once: true });
        } else {
            diagnostics.checkPage(document);
        }
    }
})();
