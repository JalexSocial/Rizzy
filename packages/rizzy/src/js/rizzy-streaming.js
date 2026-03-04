/*
 * Rizzy Blazor Streaming Rendering - htmx 4, no opt-in
 *
 * Fix vs prior versions:
 * - Do NOT try to slice a <blazor-ssr> block at <blazor-ssr-end>. In real network traces,
 *   Blazor emits complete <blazor-ssr>...</blazor-ssr> blocks over time on the same connection.
 * - This implementation:
 *    1) swaps the initial HTML as soon as it is safely delimited (</html> OR before first <blazor-ssr>)
 *    2) then incrementally processes EACH complete <blazor-ssr>...</blazor-ssr> block as it arrives
 * - Each processed block is appended into a hidden DOM sink so the native <blazor-ssr-end>
 *   custom element connectedCallback fires exactly like Blazor expects.
 *
 * Notes:
 * - Marker patching uses safe Range replacement (correctness-first).
 * - Optional DEBUG toggle is included.
 */

(function () {
    const DEBUG = false;

    const STREAM_STATE_KEY = "_rizzyStreaming";
    const STREAM_SINK_ATTR = "data-rizzy-stream-sink";

    const HTML_CLOSE = "</html>";
    const SSR_OPEN = "<blazor-ssr";
    const SSR_CLOSE = "</blazor-ssr>";

    ensureBlazorSsrEndElement();

    htmx.registerExtension("rizzy-streaming", {
        htmx_before_request(elt, detail) {
            const source = detail?.ctx?.sourceElement || elt;
            cancelStream(source, "superseded");
            return true;
        },

        htmx_before_response(elt, detail) {
            const ctx = detail?.ctx;
            const response = ctx?.response?.raw;

            if (!ctx || !response?.body) return true;

            const contentType = response.headers.get("Content-Type") || "";
            if (!/text\/html/i.test(contentType)) return true;

            // Apply basic HX-* overrides that core would otherwise apply later.
            applySimpleResponseOverrides(ctx);

            // Handle immediate actions and stop.
            if (handleImmediateResponseActions(ctx)) return false;

            startStreamingHandler(ctx).catch(error => {
                cancelStream(ctx.sourceElement, "error");
                htmx.trigger(ctx.sourceElement, "htmx:error", { ctx, error });
                if (DEBUG) console.error("[rizzy-streaming] stream error", error);
            });

            // Prevent htmx core from consuming response.text().
            return false;
        },

        htmx_before_cleanup(elt) {
            cancelStream(elt, "cleanup");
            return true;
        }
    });

    // --------------------------------------------------------------------------------------------
    // Custom element: blazor-ssr-end
    // --------------------------------------------------------------------------------------------

    function ensureBlazorSsrEndElement() {
        if (customElements.get("blazor-ssr-end")) return;

        customElements.define("blazor-ssr-end", class BlazorStreamingUpdate extends HTMLElement {
            connectedCallback() {
                const blazorSsrElement = this.parentNode;

                if (!blazorSsrElement || blazorSsrElement.nodeType !== Node.ELEMENT_NODE) return;

                // Remove wrapper immediately (Blazor pattern)
                blazorSsrElement.parentNode?.removeChild(blazorSsrElement);

                const children = Array.from(blazorSsrElement.childNodes);
                for (const node of children) {
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

    // --------------------------------------------------------------------------------------------
    // Streaming handler
    // --------------------------------------------------------------------------------------------

    async function startStreamingHandler(ctx) {
        const source = ctx.sourceElement;
        const response = ctx.response.raw;
        const reader = response.body.getReader();
        const decoder = new TextDecoder();

        const state = {
            sourceElement: source,
            reader,
            cancelled: false,
            cleanedUp: false,
            abortHandler: null,
            sink: null,
            buffer: "",
            didPrimarySwap: false
        };

        state.abortHandler = () => cancelStream(source, "abort");
        source.addEventListener("htmx:abort", state.abortHandler);

        setStreamState(source, state);

        if (DEBUG) console.log("[rizzy-streaming] open", ctx.request?.action);
        htmx.trigger(source, "htmx:rizzy:stream:open", { ctx });

        try {
            while (!state.cancelled) {
                const { done, value } = await reader.read();
                if (done) break;

                state.buffer += decoder.decode(value, { stream: true });
                await drain(ctx, state, false);
            }

            state.buffer += decoder.decode();
            await drain(ctx, state, true);
        } finally {
            cleanupStreamState(state, "ended");
        }
    }

    async function drain(ctx, state, isFinal) {
        while (!state.cancelled) {
            // 1) Primary swap: do it as soon as we can, BEFORE processing any streamed blocks.
            if (!state.didPrimarySwap) {
                const split = findPrimarySplitPoint(state.buffer);

                if (split === -1) {
                    // Not enough info yet to safely primary-swap.
                    if (isFinal && state.buffer) {
                        await doPrimarySwap(ctx, state, state.buffer);
                        state.buffer = "";
                    }
                    return;
                }

                const initialHtml = state.buffer.slice(0, split);
                await doPrimarySwap(ctx, state, initialHtml);
                state.buffer = state.buffer.slice(split);
                continue;
            }

            // 2) After primary swap, process complete <blazor-ssr>...</blazor-ssr> blocks incrementally.
            const next = findNextCompleteSsrBlock(state.buffer);

            if (!next) {
                // No complete block yet
                if (isFinal) {
                    // Any tail content gets appended conservatively
                    const tail = state.buffer.trim();
                    if (tail) {
                        await htmx.swap({
                            sourceElement: ctx.sourceElement,
                            target: ctx.target,
                            text: state.buffer,
                            swap: "beforeend",
                            transition: false,
                            response: ctx.response,
                            hx: ctx.hx,
                            push: ctx.push,
                            replace: ctx.replace
                        });
                    }
                    state.buffer = "";
                }
                return;
            }

            // If there is any non-SSR text before the next SSR block, drop whitespace or append tail.
            if (next.start > 0) {
                const prefix = state.buffer.slice(0, next.start);
                // Usually this is just newlines. If not, append it.
                if (/\S/.test(prefix)) {
                    await htmx.swap({
                        sourceElement: ctx.sourceElement,
                        target: ctx.target,
                        text: prefix,
                        swap: "beforeend",
                        transition: false,
                        response: ctx.response,
                        hx: ctx.hx,
                        push: ctx.push,
                        replace: ctx.replace
                    });
                }
                state.buffer = state.buffer.slice(next.start);
                continue;
            }

            const blockHtml = state.buffer.slice(next.start, next.end);
            state.buffer = state.buffer.slice(next.end);

            appendStreamingBlockToSink(state, blockHtml);

            // Yield so custom-element reactions + paint aren’t starved.
            await Promise.resolve();

            if (DEBUG) console.log("[rizzy-streaming] processed SSR block");
            htmx.trigger(ctx.sourceElement, "htmx:rizzy:stream:block", {});
        }
    }

    function findPrimarySplitPoint(buffer) {
        const lower = buffer.toLowerCase();

        // Full document boundary (best case)
        const htmlEnd = lower.indexOf(HTML_CLOSE);
        if (htmlEnd >= 0) return htmlEnd + HTML_CLOSE.length;

        // Otherwise, if we see the first <blazor-ssr>, the initial HTML is everything before it.
        const ssrStart = lower.indexOf(SSR_OPEN);
        if (ssrStart >= 0) return ssrStart;

        return -1;
    }

    function findNextCompleteSsrBlock(buffer) {
        const lower = buffer.toLowerCase();

        const start = lower.indexOf(SSR_OPEN);
        if (start < 0) return null;

        const endStart = lower.indexOf(SSR_CLOSE, start);
        if (endStart < 0) return null;

        const end = endStart + SSR_CLOSE.length;
        return { start, end };
    }

    async function doPrimarySwap(ctx, state, html) {
        if (!html) {
            // Nothing to swap, but mark as done so streaming can proceed.
            state.didPrimarySwap = true;
            return;
        }

        ctx.text = html;
        await htmx.swap(ctx);
        state.didPrimarySwap = true;

        fireDeferredTriggerHeaders(ctx);

        if (DEBUG) console.log("[rizzy-streaming] primary swap done");
    }

    // --------------------------------------------------------------------------------------------
    // DOM sink
    // --------------------------------------------------------------------------------------------

    function appendStreamingBlockToSink(state, blockHtml) {
        const sink = getOrCreateSink(state);
        sink.insertAdjacentHTML("beforeend", blockHtml);
    }

    function getOrCreateSink(state) {
        if (state.sink && state.sink.isConnected) return state.sink;

        const sink = document.createElement("div");
        sink.hidden = true;
        sink.style.display = "none";
        sink.setAttribute("aria-hidden", "true");
        sink.setAttribute(STREAM_SINK_ATTR, "true");
        document.body.appendChild(sink);

        state.sink = sink;
        return sink;
    }

    // --------------------------------------------------------------------------------------------
    // Marker patching (safe default)
    // --------------------------------------------------------------------------------------------

    function insertStreamingContentIntoDocument(componentIdAsString, docFrag) {
        const markers = findStreamingMarkers(componentIdAsString);
        if (!markers) {
            if (DEBUG) console.warn("[rizzy-streaming] markers not found for", componentIdAsString);
            return;
        }

        const { startMarker, endMarker } = markers;
        replaceMarkerRange(startMarker, endMarker, docFrag);
    }

    function replaceMarkerRange(startMarker, endMarker, docFrag) {
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

        // We bypassed htmx.swap(), so explicitly process inserted subtrees.
        for (const el of insertedElements) {
            htmx.process(el);
        }
    }

    function findStreamingMarkers(componentIdAsString) {
        const expectedStartText = `bl:${componentIdAsString}`;
        const iterator = document.createNodeIterator(document, NodeFilter.SHOW_COMMENT);

        let startMarker = null;
        while ((startMarker = iterator.nextNode())) {
            if (startMarker.textContent === expectedStartText) break;
        }
        if (!startMarker) return null;

        const expectedEndText = `/bl:${componentIdAsString}`;
        let endMarker = null;
        while ((endMarker = iterator.nextNode())) {
            if (endMarker.textContent === expectedEndText) break;
        }

        return endMarker ? { startMarker, endMarker } : null;
    }

    // --------------------------------------------------------------------------------------------
    // Control templates
    // --------------------------------------------------------------------------------------------

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

    // --------------------------------------------------------------------------------------------
    // HX-* response handling (minimal but important)
    // --------------------------------------------------------------------------------------------

    function applySimpleResponseOverrides(ctx) {
        if (!ctx?.hx) return;

        if (ctx.hx.retarget) ctx.target = ctx.hx.retarget;
        if (ctx.hx.reswap) ctx.swap = ctx.hx.reswap;
        if (ctx.hx.reselect) ctx.select = ctx.hx.reselect;

        fireSimpleTriggerHeader(ctx.hx.trigger, ctx.sourceElement);
    }

    function handleImmediateResponseActions(ctx) {
        if (!ctx?.hx) return false;

        if (ctx.hx.refresh === "true") {
            location.reload();
            return true;
        }

        if (ctx.hx.redirect) {
            location.href = ctx.hx.redirect;
            return true;
        }

        if (ctx.hx.location) {
            let path = ctx.hx.location;
            try {
                if (path.trim().startsWith("{")) {
                    const obj = JSON.parse(path);
                    path = obj.path || path;
                }
            } catch { /* ignore */ }

            htmx.ajax("GET", path, { source: ctx.sourceElement, target: ctx.target });
            return true;
        }

        return false;
    }

    function fireDeferredTriggerHeaders(ctx) {
        fireSimpleTriggerHeader(ctx?.hx?.triggerafterswap, ctx?.sourceElement);
        fireSimpleTriggerHeader(ctx?.hx?.triggeraftersettle, ctx?.sourceElement);
    }

    function fireSimpleTriggerHeader(rawValue, defaultTarget) {
        if (!rawValue) return;

        try {
            if (rawValue.trim().startsWith("{")) {
                const obj = JSON.parse(rawValue);
                for (const [name, detail] of Object.entries(obj)) {
                    let target = defaultTarget;
                    if (detail && typeof detail === "object" && detail.target) {
                        target = htmx.find(detail.target) || defaultTarget;
                    }
                    htmx.trigger(target, name, typeof detail === "object" ? detail : { value: detail });
                }
                return;
            }
        } catch {
            // fall through
        }

        rawValue.split(",").forEach(name => {
            const evt = name.trim();
            if (evt) htmx.trigger(defaultTarget, evt, {});
        });
    }

    // --------------------------------------------------------------------------------------------
    // Stream state / cleanup
    // --------------------------------------------------------------------------------------------

    function setStreamState(elt, state) {
        if (!elt) return;
        elt._htmx ||= {};
        elt._htmx[STREAM_STATE_KEY] = state;
    }

    function getStreamState(elt) {
        return elt?._htmx?.[STREAM_STATE_KEY] || null;
    }

    function clearStreamState(elt) {
        if (elt?._htmx?.[STREAM_STATE_KEY]) {
            delete elt._htmx[STREAM_STATE_KEY];
        }
    }

    function cancelStream(elt, reason) {
        const state = getStreamState(elt);
        if (!state) return;

        state.cancelled = true;
        try { state.reader?.cancel?.(); } catch { /* ignore */ }

        cleanupStreamState(state, reason || "cancelled");
    }

    function cleanupStreamState(state, reason) {
        if (!state || state.cleanedUp) return;

        state.cleanedUp = true;
        state.cancelled = true;

        try { state.reader?.cancel?.(); } catch { /* ignore */ }

        if (state.abortHandler) {
            state.sourceElement?.removeEventListener("htmx:abort", state.abortHandler);
        }

        if (state.sink?.isConnected) {
            state.sink.remove();
        }

        clearStreamState(state.sourceElement);

        if (DEBUG) console.log("[rizzy-streaming] close:", reason);
        htmx.trigger(state.sourceElement, "htmx:rizzy:stream:close", { reason });
    }
})();