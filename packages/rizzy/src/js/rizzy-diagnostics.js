// Rizzy client diagnostics (RIZZY-026, RIZZY-028, RM-D3)
//
// 1. Error reporting that follows htmx 4's logging convention: an event whose detail carries
//    `error` is logged with console.error, one carrying `warn` with console.warn, then the event
//    is dispatched. htmx core applies this convention only to events it triggers itself;
//    htmx.trigger() (the public API extensions use) dispatches without logging, so Rizzy
//    extensions report through Rizzy.diagnostics.report() instead.
//
// 2. A page check when the bundle loads. It warns once, on the console, when:
//    - the page has no <meta name="htmx-config"> (the root component does not render
//      <HeadOutlet />, so the antiforgery token and Rizzy settings are missing), or
//    - the meta tag is not valid JSON, or
//    - the server's Rizzy major version (rizzyVersion in the meta tag) differs from the major
//      version this script was built for (for example a cached or CDN copy of rizzy.js).
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
                'Rizzy: no <meta name="htmx-config"> on this page. The root component must render ' +
                '<HeadOutlet /> (Rizzy\'s HtmxApp<TLayout> does), otherwise the antiforgery token ' +
                'and Rizzy settings are not sent to the client.');
        } else {
            let config = null;
            try {
                config = JSON.parse(meta.getAttribute("content") || "");
            } catch {
                warnings.push('Rizzy: <meta name="htmx-config"> does not contain valid JSON; htmx ignores it.');
            }

            if (config?.rizzyDiagnostics === false) return [];

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
