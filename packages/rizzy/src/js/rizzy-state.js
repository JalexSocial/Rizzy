// Ensure Rizzy global and store structure exists
window.Rizzy = window.Rizzy || {};
window.Rizzy.store = window.Rizzy.store || {};

// Initialize or augment the 'rz' namespaced store for state features
window.Rizzy.store.rz = Alpine.reactive({
    token: document.querySelector(htmx.config.rizzyStateSelector || `#${RizzyStateConstants.RzStateScriptTagId}`)?.textContent?.trim() ?? '',
    patch: [],
    q: Promise.resolve(),
    isProcessing: false, // To potentially indicate if a stateful request is in flight via the queue
    operationId: null    // Stores the current RZ-Operation-Id for debugging or advanced scenarios
});

/**
 * Alpine.js magic helper to enqueue a JSON Patch operation.
 * Usage: <button @click="$mut('replace', '/propertyName', newValue)">...</button>
 * @param {string} op - The JSON Patch operation (e.g., 'add', 'remove', 'replace', 'move', 'copy', 'test').
 * @param {string} path - The JSON Pointer path to the target property.
 * @param {any} value - The value for the operation (e.g., new value for 'replace' or 'add').
 */
Alpine.magic('mut', () => (op, path, value) => {
    window.Rizzy.store.rz.patch.push({ op, path, value });
});

/**
 * Enqueues an htmx request promise onto a FIFO queue to ensure sequential processing.
 * @param {object} htmxEventDetail - The event.detail object from htmx:configRequest.
 * @returns {Promise} A promise that resolves or rejects when the htmx request lifecycle completes.
 */
function enqueueHtmxRequest(htmxEventDetail) {
    const { xhr } = htmxEventDetail;
    const store = window.Rizzy.store.rz;

    const requestPromise = new Promise((resolve, reject) => {
        // 'loadend' fires for both success and error, after 'load'/'error'/'abort'
        xhr.addEventListener('loadend', () => {
            // Resolve with enough info for the chain to know if it should proceed or if it's an error state
            if (xhr.status >= 200 && xhr.status < 300) {
                resolve({ status: xhr.status, responseText: xhr.responseText, xhr });
            } else {
                // For htmx, even errors might be "handled" by swapping error content.
                // We resolve so the queue continues, but error handlers (like htmx:responseError) can still act.
                resolve({ status: xhr.status, error: true, xhr });
            }
        }, { once: true });

        // Explicit error/abort also "resolve" the queue's link, but state-error event handles UX
        xhr.addEventListener('error', () => resolve({ error: true, aborted: false, xhr }), { once: true });
        xhr.addEventListener('abort', () => resolve({ error: true, aborted: true, xhr }), { once: true });
    });

    store.q = store.q
        .catch(() => { /* Swallow previous error to allow queue to continue */ })
        .then(() => {
            store.isProcessing = true; // Mark as processing before starting the actual request
            store.operationId = htmxEventDetail.headers['RZ-Operation-Id']; // Store current op ID
            return requestPromise;
        })
        .finally(() => {
            // This simplistic 'isProcessing' might need refinement for highly concurrent UI updates.
            // If queue length > 0 after this, it's still processing.
            store.isProcessing = false; // Mark as done after this request settles
            store.operationId = null;
        });
    return store.q;
}

// htmx event listeners
htmx.on('htmx:configRequest', event => {
    const detail = event.detail;
    const store = window.Rizzy.store.rz;
    const targetElement = event.target;

    // Only add state/patch for mutating verbs unless explicitly configured otherwise
    if (detail.verb && detail.verb.toLowerCase() !== 'get') {
        detail.headers = detail.headers || {}; // Ensure headers object exists
        detail.headers[RizzyStateConstants.HtmxRequestHeaders.RZRequest] = '1';
        detail.headers['RZ-Operation-Id'] = crypto.randomUUID();

        detail.parameters = detail.parameters || {};
        detail.parameters[RizzyStateConstants.HtmxRequestHeaders.RZState] = store.token;
        detail.parameters[RizzyStateConstants.HtmxRequestHeaders.RZPatch] = JSON.stringify(store.patch);

        store.patch = []; // Clear patch after adding to current request
    }

    // Queue the request unless it's marked to bypass
    if (targetElement && targetElement.closest('.x-rz-long') && targetElement.getAttribute('hx-sync') === 'this:drop') {
        // Element is marked to bypass Rizzy's queue and use htmx's own sync="this:drop"
    } else if (detail.verb && detail.verb.toLowerCase() !== 'get' ) { // Typically only queue mutating verbs
        enqueueHtmxRequest(detail);
    }
});

htmx.on('htmx:afterOnLoad', event => {
    const newToken = event.detail.xhr.getResponseHeader(RizzyStateConstants.HtmxResponseHeaders.RZState);
    if (newToken) {
        window.Rizzy.store.rz.token = newToken;
    }
    // Potentially re-initialize Alpine.js on new content if htmx.process did not handle it.
    // However, htmx.process(event.detail.target) is usually sufficient or done by default.
});

htmx.on('htmx:responseError', event => {
    // Dispatch a global event for UI components to listen to
    window.dispatchEvent(new CustomEvent('rizzy:state-error', {
        bubbles: true,
        composed: true,
        detail: {
            xhr: event.detail.xhr,
            error: event.detail.error, // Native XHR error event if network error
            target: event.detail.target,
            message: `Request to ${event.detail.pathInfo.requestPath} failed with status ${event.detail.xhr.status}.`
        }
    }));
});

// Expose constants to JS if needed, e.g., for the script tag ID
// This assumes RizzyStateConstants.cs is somehow made available or its values are hardcoded here.
// For simplicity, we'll assume #rz-state is the default and can be overridden by htmx.config.rizzyStateSelector
const RizzyStateConstants = {
    RzStateScriptTagId: "rz-state", // Must match server-side default
    HtmxRequestHeaders: { // For client-side use if constructing hx-headers manually
        RZRequest: "RZ-Request",
        RZState: "rz-state", // form param
        RZPatch: "rz-patch"  // form param
    },
    HtmxResponseHeaders: {
        RZState: "RZ-State" // http header
    }
};