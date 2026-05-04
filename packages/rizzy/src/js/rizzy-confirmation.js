// Rizzy Confirm Dialog HTMX 4 Extension
//
// Replaces the built-in window.confirm() behavior for hx-confirm by:
// - Intercepting the "htmx:confirm" hook (htmx_confirm extension method)
// - Emitting a custom window event "rz:confirm-dialog" with merged props
// - Resolving the HTMX confirm via detail.issueRequest() / detail.dropRequest()
//
// Usage:
//   <button hx-post="/do-thing" hx-confirm="Are you sure?">...</button>
//
// Optional JSON form (so you can configure title/buttons/etc):
//   <button hx-post="/do-thing" hx-confirm='{"title":"Proceed?","text":"Delete item?","showCancelButton":true}'>...</button>
//
// Your UI layer should listen for "rz:confirm-dialog" and render a dialog,
// then call one of the callbacks on event.detail: confirm(value), deny(), dismiss()

(function () {
    const EXT_NAME = "rizzy-confirm";

    /**
     * Public confirm() helper (same shape as your original).
     * Dispatches rz:confirm-dialog and returns a promise resolved by callbacks.
     */
    function confirm(props = {}) {
        return new Promise((resolve) => {
            const result = {
                isConfirmed: false,
                isDenied: false,
                isDismissed: false,
                value: null
            };

            // Callbacks the dialog UI will invoke
            const resultEvents = {
                confirm: function (value) {
                    result.isConfirmed = true;
                    result.value = value ?? null;
                    resolve(result);
                },
                deny: function () {
                    result.isDenied = true;
                    resolve(result);
                },
                dismiss: function () {
                    result.isDismissed = true;
                    resolve(result);
                }
            };

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

            const eventData = { ...defaultEventData, ...props, ...resultEvents };

            window.dispatchEvent(
                new CustomEvent("rz:confirm-dialog", {
                    detail: eventData
                })
            );
        });
    }

    /**
     * Parse hx-confirm content:
     * - If JSON parseable => treated as props
     * - Else => treated as message text
     */
    function parseConfirmValue(confirmValue) {
        if (!confirmValue) return null;

        // If it's already an object (someone set ctx.confirm programmatically), accept it.
        if (typeof confirmValue === "object") return confirmValue;

        if (typeof confirmValue !== "string") {
            return { title: "Proceed?", text: String(confirmValue) };
        }

        const trimmed = confirmValue.trim();
        if (!trimmed) return null;

        // Attempt JSON first
        if (trimmed.startsWith("{") || trimmed.startsWith("[")) {
            try {
                return JSON.parse(trimmed);
            } catch {
                // fall through to plain text
            }
        }

        return { title: "Proceed?", text: confirmValue };
    }

    htmx.registerExtension(EXT_NAME, {
        /**
         * htmx_confirm maps to the "htmx:confirm" hook in htmx 4
         * (colons become underscores in extension method names).
         *
         * In the htmx 4 source you pasted, the confirm hook detail is:
         *   { ctx, issueRequest: () => resolve(true), dropRequest: () => resolve(false) }
         *
         * We must call issueRequest() to continue or dropRequest() to cancel.
         */
        htmx_confirm: (elt, detail) => {
            const ctx = detail?.ctx;
            const confirmValue = ctx?.confirm;

            // If there is no confirm configured, allow htmx to continue normal flow.
            if (!confirmValue) return;

            // Stop the default confirm flow (i.e., prevent built-in window.confirm behavior).
            // In htmx 4, returning false (or setting detail.cancelled) cancels further processing.
            detail.cancelled = true;

            const props = parseConfirmValue(confirmValue);
            if (!props) {
                // Nothing meaningful to confirm => just proceed.
                detail.issueRequest();
                return false;
            }

            // Show your custom dialog
            confirm(props).then((result) => {
                if (result.isConfirmed) {
                    detail.issueRequest();
                } else {
                    detail.dropRequest();
                }
            });

            return false;
        }
    });

    // Optional: export confirm for non-htmx callers if you are bundling this as a module.
    // If you're not using modules, remove the export line.
    // export default confirm;

    // If you *are* in an ES module context and want to export:
    // window.Rizzy = window.Rizzy || {};
    // window.Rizzy.confirm = confirm;

})();