// Rizzy Confirm Dialog htmx 4 extension
//
// Lets a custom dialog replace the browser's window.confirm() for hx-confirm:
// - Hooks "htmx:confirm" (the htmx_confirm extension method)
// - Dispatches a cancelable window event "rz:confirm-dialog" with the dialog props
// - If a dialog takes the event, resolves htmx's confirm through
//   detail.issueRequest() / detail.dropRequest()
// - Otherwise falls back to the native window.confirm() (RIZZY-005)
//
// Usage:
//   <button hx-post="/do-thing" hx-confirm="Are you sure?">...</button>
//
// Optional JSON form (configure title/buttons/etc):
//   <button hx-post="/do-thing" hx-confirm='{"title":"Proceed?","text":"Delete item?","showCancelButton":true}'>...</button>
//
// A dialog component takes over by listening for "rz:confirm-dialog" and either calling
// event.preventDefault() or setting event.detail.handled = true, then calling one of
// event.detail.confirm(value), event.detail.deny() or event.detail.dismiss().
// Apps can also opt in globally with htmx.config.rizzyConfirmDialog = true (for example via
// the htmx-config meta tag) when their dialog listener cannot claim the event synchronously.
//
// hx-confirm values starting with "js:" or "javascript:" are left to htmx core.

(function () {
    const EXT_NAME = "rizzy-confirm";
    const DIALOG_EVENT = "rz:confirm-dialog";

    const defaultEventData = {
        title: "Proceed?",
        text: "",
        showConfirmButton: true,
        showDenyButton: false,
        showCancelButton: false,
        confirmButtonText: "OK",
        denyButtonText: "No",
        dismissButtonText: "Cancel"
    };

    /**
     * Dispatches rz:confirm-dialog and returns whether a dialog took it, plus a promise that the
     * dialog's callbacks resolve.
     */
    function requestDialog(props) {
        let resolveResult;
        const promise = new Promise((resolve) => { resolveResult = resolve; });
        const result = { isConfirmed: false, isDenied: false, isDismissed: false, value: null };

        const eventData = {
            ...defaultEventData,
            ...props,
            handled: false,
            confirm(value) {
                result.isConfirmed = true;
                result.value = value ?? null;
                resolveResult(result);
            },
            deny() {
                result.isDenied = true;
                resolveResult(result);
            },
            dismiss() {
                result.isDismissed = true;
                resolveResult(result);
            }
        };

        const event = new CustomEvent(DIALOG_EVENT, { detail: eventData, cancelable: true });
        window.dispatchEvent(event);

        const optedIn = typeof htmx !== "undefined" && htmx.config?.rizzyConfirmDialog === true;
        return { handled: event.defaultPrevented || eventData.handled === true || optedIn, promise };
    }

    /**
     * Public confirm() helper for non-htmx callers. Resolves through a custom dialog when one takes
     * the event, otherwise through window.confirm().
     */
    function confirm(props = {}) {
        const { handled, promise } = requestDialog(props);
        if (handled) return promise;

        const confirmed = window.confirm(nativeMessage(props));
        return Promise.resolve({ isConfirmed: confirmed, isDenied: false, isDismissed: !confirmed, value: null });
    }

    function nativeMessage(props) {
        return [props.title, props.text].filter(Boolean).join("\n\n") || defaultEventData.title;
    }

    /**
     * Parse hx-confirm content:
     * - JSON object => dialog props
     * - anything else => message text
     * Returns { props, isJson } or null when there is nothing to confirm.
     */
    function parseConfirmValue(confirmValue) {
        if (confirmValue == null) return null;

        if (typeof confirmValue === "object") return { props: confirmValue, isJson: true };

        const text = String(confirmValue);
        const trimmed = text.trim();
        if (!trimmed) return null;

        if (trimmed.startsWith("{")) {
            try {
                return { props: JSON.parse(trimmed), isJson: true };
            } catch {
                // fall through to plain text
            }
        }

        return { props: { title: defaultEventData.title, text }, isJson: false };
    }

    function isJavascriptConfirm(value) {
        return typeof value === "string" && /^\s*(js|javascript):/.test(value);
    }

    htmx.registerExtension(EXT_NAME, {
        // htmx 4 confirm hook detail: { ctx, issueRequest(), dropRequest() }. Setting
        // detail.cancelled skips htmx's own window.confirm and waits for one of the callbacks.
        htmx_confirm: (elt, detail) => {
            const confirmValue = detail?.ctx?.confirm;
            if (!confirmValue || isJavascriptConfirm(confirmValue)) return;

            const parsed = parseConfirmValue(confirmValue);
            if (!parsed) return;

            const { handled, promise } = requestDialog(parsed.props);

            if (!handled) {
                // No dialog: plain text goes to htmx's native window.confirm. JSON props would show
                // up as raw JSON there, so confirm with a readable message instead.
                if (!parsed.isJson) return;

                detail.cancelled = true;
                if (window.confirm(nativeMessage(parsed.props))) detail.issueRequest();
                else detail.dropRequest();
                return false;
            }

            detail.cancelled = true;
            promise.then((result) => {
                if (result.isConfirmed) detail.issueRequest();
                else detail.dropRequest();
            });
            return false;
        }
    });

    window.Rizzy = window.Rizzy || {};
    window.Rizzy.confirm = confirm;
})();
