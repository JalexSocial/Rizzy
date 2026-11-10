/*
 * Rizzy Blazor streaming rendering for htmx 4 (RIZZY-003, RIZZY-019)
 *
 * Blazor SSR streaming sends the initial page (ending in </html>; Rizzy wraps htmx fragments in a
 * minimal <html><body> document when streaming is possible) and then keeps the connection open,
 * sending <blazor-ssr>...</blazor-ssr> update blocks as async work completes.
 *
 * This extension cooperates with htmx core instead of replacing its response handling:
 *
 * 1. It wraps ctx.fetch. The wrapper reads the response body until the stream ends or the
 *    initial document is complete (</html>), then hands htmx core an ordinary Response that
 *    holds just that initial HTML. Core therefore runs its normal pipeline: htmx:after:request,
 *    htmx:response:error, HX-* headers, hx-status, the swap, HX-Trigger (once, after the swap),
 *    indicators and hx-sync queueing.
 * 2. If more data follows, the rest of the stream is read in the background and exposed as
 *    ctx.extensionPromise, which htmx core awaits before it fires HX-Trigger, hides indicators
 *    and releases the hx-sync queue. The request timeout is cleared while streaming.
 * 3. Each complete <blazor-ssr> block is appended to a hidden sink once the main swap is done,
 *    where the <blazor-ssr-end> custom element patches the streamed component content in place
 *    (the same mechanism Blazor uses).
 *
 * Non-HTML responses, responses without a body and responses that are already complete pass
 * through unchanged (complete responses are re-wrapped from the buffered text).
 *
 * Events: htmx:rizzy:stream:open, htmx:rizzy:stream:block, htmx:rizzy:stream:close (on the
 * source element).
 */

(function () {
    const DEBUG = false;
    const report = (elt, name, detail) => window.Rizzy?.diagnostics?.report
        ? window.Rizzy.diagnostics.report(elt, name, detail)
        : htmx.trigger(elt, name, detail);

    const STREAM_SINK_ATTR = "data-rizzy-stream-sink";
    const HTML_CLOSE = "</html>";
    const SSR_OPEN = "<blazor-ssr";
    const SSR_CLOSE = "</blazor-ssr>";
    const NULL_BODY_STATUSES = new Set([101, 103, 204, 205, 304]);

    const streams = new WeakMap(); // ctx -> stream state

    ensureBlazorSsrEndElement();

    htmx.registerExtension("rizzy-streaming", {
        htmx_before_request(_elt, detail) {
            const ctx = detail?.ctx;
            if (!ctx || typeof ctx.fetch !== "function" || ctx.fetch._rizzyStreaming) return;

            // Other extensions may change ctx.fetch after this hook runs, in two ways:
            //   - replace it (hx-preload installs `() => prefetchedResponse`), or
            //   - wrap it (`const prev = ctx.fetch; ctx.fetch = (u, i) => prev(u, i)`).
            // ctx.fetch therefore becomes an accessor. Reads always return the streaming wrapper;
            // each assignment adds a layer. A wrapping extension captured the wrapper as `prev`,
            // so the wrapper is re-entered from inside the layer above; each re-entry calls the
            // next layer down, which reproduces the plain-assignment chain (newest layer first,
            // down to the fetch that was in place before this hook) with streaming applied once
            // to the final response.
            const layers = [ctx.fetch];
            let depth = 0;
            const wrapped = async (url, init) => {
                if (depth > 0) {
                    // Step one layer down for the duration of this call, then back up, so a layer
                    // that calls its fetch again (a retry) reaches the same layer below it.
                    const layer = layers[Math.max(0, layers.length - 1 - depth)];
                    depth++;
                    try {
                        return await layer(url, init);
                    } finally {
                        depth--;
                    }
                }
                depth = 1;
                let response;
                try {
                    response = await layers[layers.length - 1](url, init);
                } finally {
                    depth = 0;
                }
                return splitStreamingResponse(ctx, response);
            };
            wrapped._rizzyStreaming = true;
            Object.defineProperty(ctx, "fetch", {
                configurable: true,
                enumerable: true,
                get: () => wrapped,
                set: (value) => {
                    if (typeof value === "function" && !value._rizzyStreaming) layers.push(value);
                },
            });
        },

        htmx_after_swap(_elt, detail) {
            const state = detail?.ctx && streams.get(detail.ctx);
            if (state && !state.swapped) {
                state.swapped = true;
                flushPendingBlocks(state);
            }
        }
    });

    // --------------------------------------------------------------------------------------------
    // Response splitting
    // --------------------------------------------------------------------------------------------

    async function splitStreamingResponse(ctx, response) {
        if (!response?.body || NULL_BODY_STATUSES.has(response.status)) return response;

        const contentType = response.headers.get("Content-Type") || "";
        if (!/text\/html/i.test(contentType)) return response;

        const reader = response.body.getReader();
        const decoder = new TextDecoder();
        let buffer = "";

        while (true) {
            const { done, value } = await reader.read();
            if (done) {
                buffer += decoder.decode();
                return rewrap(response, buffer);
            }

            buffer += decoder.decode(value, { stream: true });

            const htmlEnd = buffer.toLowerCase().indexOf(HTML_CLOSE);
            if (htmlEnd >= 0) {
                const split = htmlEnd + HTML_CLOSE.length;
                const initial = buffer.slice(0, split);
                const rest = buffer.slice(split);
                startBackgroundStream(ctx, response, reader, decoder, rest);
                return rewrap(response, initial);
            }
        }
    }

    function rewrap(original, text) {
        const response = new Response(text, {
            status: original.status,
            statusText: original.statusText,
            headers: original.headers
        });
        // Keep url/redirected for extensions that inspect ctx.response.raw (e.g. origin checks).
        try {
            Object.defineProperty(response, "url", { value: original.url });
            Object.defineProperty(response, "redirected", { value: original.redirected });
        } catch { /* ignore */ }
        return response;
    }

    // --------------------------------------------------------------------------------------------
    // Background streaming of <blazor-ssr> blocks
    // --------------------------------------------------------------------------------------------

    function startBackgroundStream(ctx, response, reader, decoder, initialRest) {
        const state = {
            ctx,
            response,
            reader,
            buffer: initialRest,
            pending: [],
            swapped: false,
            sink: null,
            opened: false
        };
        streams.set(ctx, state);

        ctx.extensionPromise = readRemainder(state, decoder)
            .catch((error) => {
                if (error?.name !== "AbortError") {
                    // Logged per htmx's detail.error convention, then dispatched (RM-D3).
                    report(ctx.sourceElement, "htmx:error", { ctx, error });
                }
            })
            .finally(() => closeStream(state));
    }

    async function readRemainder(state, decoder) {
        collectBlocks(state, false);

        while (true) {
            const { done, value } = await state.reader.read();
            if (done) break;

            if (!state.opened) {
                state.opened = true;
                // The response really is streaming: the request timeout must not abort it.
                clearTimeout(state.ctx.requestTimeout);
                htmx.trigger(state.ctx.sourceElement, "htmx:rizzy:stream:open", { ctx: state.ctx });
            }

            state.buffer += decoder.decode(value, { stream: true });
            collectBlocks(state, false);
        }

        state.buffer += decoder.decode();
        collectBlocks(state, true);
    }

    function collectBlocks(state, isFinal) {
        while (true) {
            const lower = state.buffer.toLowerCase();
            const start = lower.indexOf(SSR_OPEN);
            if (start < 0) break;

            const closeStart = lower.indexOf(SSR_CLOSE, start);
            if (closeStart < 0) break;

            const end = closeStart + SSR_CLOSE.length;
            state.pending.push(state.buffer.slice(start, end));
            state.buffer = state.buffer.slice(end);
        }

        if (isFinal) {
            if (DEBUG && state.buffer.trim()) console.warn("[rizzy-streaming] ignoring trailing content", state.buffer);
            state.buffer = "";
        }

        if (state.swapped) flushPendingBlocks(state);
    }

    function flushPendingBlocks(state) {
        while (state.pending.length) {
            const blockHtml = state.pending.shift();
            const html = window.Rizzy?.nonce?.processResponseHtml
                ? window.Rizzy.nonce.processResponseHtml(blockHtml, state.response)
                : blockHtml;

            getOrCreateSink(state).insertAdjacentHTML("beforeend", html);
            htmx.trigger(state.ctx.sourceElement, "htmx:rizzy:stream:block", { ctx: state.ctx });
            if (DEBUG) console.log("[rizzy-streaming] processed SSR block");
        }
    }

    function closeStream(state) {
        // Blocks that arrived when no swap happened (noSwap status, redirect, ...) are dropped.
        state.pending.length = 0;
        if (state.sink?.isConnected) state.sink.remove();
        streams.delete(state.ctx);
        if (state.opened) {
            htmx.trigger(state.ctx.sourceElement, "htmx:rizzy:stream:close", { ctx: state.ctx });
        }
        if (DEBUG) console.log("[rizzy-streaming] close");
    }

    function getOrCreateSink(state) {
        if (state.sink?.isConnected) return state.sink;

        const sink = document.createElement("div");
        sink.hidden = true;
        sink.setAttribute("aria-hidden", "true");
        sink.setAttribute(STREAM_SINK_ATTR, "true");
        document.body.appendChild(sink);

        state.sink = sink;
        return sink;
    }

    // --------------------------------------------------------------------------------------------
    // Custom element: blazor-ssr-end (patches streamed content into the component markers)
    // --------------------------------------------------------------------------------------------

    function ensureBlazorSsrEndElement() {
        if (customElements.get("blazor-ssr-end")) return;

        customElements.define("blazor-ssr-end", class BlazorStreamingUpdate extends HTMLElement {
            connectedCallback() {
                const blazorSsrElement = this.parentNode;
                if (!blazorSsrElement || blazorSsrElement.nodeType !== Node.ELEMENT_NODE) return;

                blazorSsrElement.parentNode?.removeChild(blazorSsrElement);

                for (const node of Array.from(blazorSsrElement.childNodes)) {
                    if (!(node instanceof HTMLTemplateElement)) continue;

                    const componentId = node.getAttribute("blazor-component-id");
                    if (componentId) {
                        insertStreamingContentIntoDocument(componentId, node.content);
                    } else {
                        handleControlTemplate(node);
                    }
                }
            }
        });
    }

    function insertStreamingContentIntoDocument(componentIdAsString, docFrag) {
        const markers = findStreamingMarkers(componentIdAsString);
        if (!markers) {
            if (DEBUG) console.warn("[rizzy-streaming] markers not found for", componentIdAsString);
            return;
        }

        const { startMarker, endMarker } = markers;
        const destinationRoot = endMarker.parentNode;
        if (!destinationRoot) return;

        const existing = new Range();
        existing.setStart(startMarker, startMarker.textContent?.length || 0);
        existing.setEnd(endMarker, 0);
        existing.deleteContents();

        const insertedElements = [];
        while (docFrag.firstChild) {
            const node = docFrag.firstChild;
            destinationRoot.insertBefore(node, endMarker);
            if (node.nodeType === Node.ELEMENT_NODE) insertedElements.push(node);
        }

        // Content bypassed htmx.swap(), so initialize any htmx attributes it contains.
        for (const el of insertedElements) htmx.process(el);
    }

    function findStreamingMarkers(componentIdAsString) {
        const expectedStartText = `bl:${componentIdAsString}`;
        const expectedEndText = `/bl:${componentIdAsString}`;
        const iterator = document.createNodeIterator(document, NodeFilter.SHOW_COMMENT);

        let startMarker = null;
        while ((startMarker = iterator.nextNode())) {
            if (startMarker.textContent === expectedStartText) break;
        }
        if (!startMarker) return null;

        let endMarker = null;
        while ((endMarker = iterator.nextNode())) {
            if (endMarker.textContent === expectedEndText) break;
        }

        return endMarker ? { startMarker, endMarker } : null;
    }

    function handleControlTemplate(node) {
        const type = node.getAttribute("type");
        const text = (node.content.textContent || "").trim();

        switch (type) {
            case "redirection":
            case "not-found":
                if (text) location.replace(text);
                break;
            case "error":
                document.body.textContent = text || "Error";
                break;
        }
    }
})();
