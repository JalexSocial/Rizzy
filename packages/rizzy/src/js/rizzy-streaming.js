/*
 * Rizzy Blazor Streaming Rendering - htmx 4, no opt-in, Blazor-aligned flush boundary
 *
 * Key points:
 * - Registers <blazor-ssr-end> globally (like Blazor)
 * - Intercepts ALL text/html htmx responses at htmx:before:response
 * - Streams the response body and:
 *    - Performs the "primary" htmx swap with any leading non-stream HTML (once)
 *    - Then processes streamed units incrementally as soon as </blazor-ssr-end> is present
 *      (this is the true Blazor "safe boundary")
 * - Feeds each ready unit into a hidden live DOM sink so <blazor-ssr-end> connectedCallback
 *   runs at the correct time (Blazor's intended mechanism)
 *
 * Notes:
 * - DOM preservation: Blazor uses synchronizeDomContent(...) in some cases.
 *   This implementation uses safe range replacement by default (correctness over minimal DOM churn).
 *   You can port synchronizeDomContent later if you need strict parity.
 */

(function () {
    let api;
    let enableDomPreservation = true;

    const STREAM_STATE_KEY = "_rizzyStreaming";
    const STREAM_SINK_ATTR = "data-rizzy-stream-sink";

    const BLAZOR_SSR_START_RE = /<blazor-ssr(?=[\s>])/i;
    const BLAZOR_SSR_CLOSE = "</blazor-ssr>";
    const BLAZOR_SSR_END_CLOSE = "</blazor-ssr-end>";

    ensureBlazorSsrEndElement();

    htmx.registerExtension("rizzy-streaming", {
        init(apiRef) {
            api = apiRef;
        },

        // If the same element issues a new request, cancel any active stream for it.
        htmx_before_request(elt, detail) {
            const source = detail?.ctx?.sourceElement || elt;
            cancelStream(source, "superseded");
            return true;
        },

        // htmx 4 hook: response exists, body has not been consumed yet.
        htmx_before_response(elt, detail) {
            const ctx = detail?.ctx;
            const response = ctx?.response?.raw;

            if (!ctx || !response?.body) {
                return true;
            }

            const contentType = response.headers.get("Content-Type") || "";
            if (!/text\/html/i.test(contentType)) {
                return true;
            }

            // htmx core already extracted HX-* headers into ctx.hx before this hook.
            applySimpleResponseOverrides(ctx);

            // If an immediate action header is present, honor it and stop core.
            if (handleImmediateResponseActions(ctx)) {
                return false;
            }

            startStreamingHandler(ctx).catch(error => {
                cancelStream(ctx.sourceElement, "error");
                htmx.trigger(ctx.sourceElement, "htmx:error", { ctx, error });
            });

            // Prevent htmx core from calling response.text().
            // Core will finalize the request lifecycle immediately; streaming continues independently.
            return false;
        },

        htmx_before_cleanup(elt) {
            cancelStream(elt, "cleanup");
            return true;
        }
    });

    // --------------------------------------------------------------------------------------------
    // Custom Element: blazor-ssr-end
    // --------------------------------------------------------------------------------------------

    function ensureBlazorSsrEndElement() {
        if (customElements.get("blazor-ssr-end")) {
            return;
        }

        // Mirrors the idea in Blazor's StreamingRendering.ts: by the time <blazor-ssr-end>
        // is connected, preceding content within the same <blazor-ssr> is present, so process it.
        customElements.define("blazor-ssr-end", class BlazorStreamingUpdate extends HTMLElement {
            connectedCallback() {
                const blazorSsrElement = this.parentNode;

                if (!blazorSsrElement || blazorSsrElement.nodeType !== Node.ELEMENT_NODE) {
                    return;
                }

                // Remove wrapper immediately, like Blazor does.
                blazorSsrElement.parentNode?.removeChild(blazorSsrElement);

                const childNodes = Array.from(blazorSsrElement.childNodes);

                for (const node of childNodes) {
                    if (!(node instanceof HTMLTemplateElement)) {
                        continue;
                    }

                    const componentId = node.getAttribute("blazor-component-id");
                    if (componentId) {
                        insertStreamingContentIntoDocument(componentId, node.content);
                        continue;
                    }

                    handleControlTemplate(node);
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
            abortHandler: null,
            sink: null,
            buffer: "",
            sawStreamingBlock: false,
            didPrimarySwap: false
        };

        state.abortHandler = () => cancelStream(source, "abort");
        source.addEventListener("htmx:abort", state.abortHandler);
        setStreamState(source, state);

        htmx.trigger(source, "htmx:rizzy:stream:open", { ctx });

        try {
            while (!state.cancelled) {
                const { done, value } = await reader.read();

                if (done) {
                    break;
                }

                state.buffer += decoder.decode(value, { stream: true });
                await drainProcessableBuffer(ctx, state, false);
            }

            // Flush decoder
            state.buffer += decoder.decode();
            await drainProcessableBuffer(ctx, state, true);
        } finally {
            cleanupStreamState(state, "ended");
        }
    }

    async function drainProcessableBuffer(ctx, state, isFinal) {
        while (!state.cancelled) {
            // If we previously synthesized a </blazor-ssr>, the real close may arrive later.
            // Strip any leading standalone closers before proceeding.
            state.buffer = stripLeadingStandaloneSsrClosers(state.buffer);

            const startIndex = findFirstBlazorBlockStart(state.buffer);

            // No <blazor-ssr> currently present
            if (startIndex < 0) {
                if (isFinal && state.buffer) {
                    await flushHtmlSegment(ctx, state, state.buffer);
                    state.buffer = "";
                }
                return;
            }

            // Flush plain HTML before the first <blazor-ssr>
            if (startIndex > 0) {
                const plainHtml = state.buffer.slice(0, startIndex);
                await flushHtmlSegment(ctx, state, plainHtml);
                state.buffer = state.buffer.slice(startIndex);
                continue;
            }

            // Buffer starts with <blazor-ssr...>
            const unit = extractNextSafeStreamingUnit(state.buffer, isFinal);

            if (!unit) {
                // Not enough data to safely process a streaming unit yet.
                if (isFinal) {
                    // Tail fallback: if final and still incomplete, dump remainder as HTML
                    await flushHtmlSegment(ctx, state, state.buffer);
                    state.buffer = "";
                }
                return;
            }

            state.sawStreamingBlock = true;

            appendStreamingBlockToSink(state, unit.htmlForSink);

            // Consume only what came off the wire (not synthetic close)
            state.buffer = state.buffer.slice(unit.consumed);

            htmx.trigger(ctx.sourceElement, "htmx:rizzy:stream:block", {
                syntheticClose: unit.syntheticClose
            });
        }
    }

    async function flushHtmlSegment(ctx, state, html) {
        if (!html) {
            return;
        }

        // First non-stream segment uses the original ctx swap semantics (main swap).
        // After streaming begins, or after primary swap, we append plain tail segments.
        if (!state.didPrimarySwap && !state.sawStreamingBlock) {
            ctx.text = html;
            await htmx.swap(ctx);
            state.didPrimarySwap = true;

            fireSimpleTriggerHeader(ctx.hx?.triggerafterswap, ctx.sourceElement);
            return;
        }

        await htmx.swap({
            sourceElement: ctx.sourceElement,
            target: ctx.target,
            text: html,
            swap: "beforeend",
            transition: false,
            response: ctx.response,
            hx: ctx.hx
        });

        fireSimpleTriggerHeader(ctx.hx?.triggerafterswap, ctx.sourceElement);
    }

    // --------------------------------------------------------------------------------------------
    // Streaming unit parsing
    //
    // IMPORTANT:
    // We flush as soon as </blazor-ssr-end> is present, not when </blazor-ssr> arrives.
    // If </blazor-ssr> isn't present yet, we synthesize it for the DOM sink fragment.
    // --------------------------------------------------------------------------------------------

    function findFirstBlazorBlockStart(html) {
        const match = BLAZOR_SSR_START_RE.exec(html);
        return match ? match.index : -1;
    }

    function extractNextSafeStreamingUnit(buffer, isFinal) {
        if (findFirstBlazorBlockStart(buffer) !== 0) {
            return null;
        }

        const lower = buffer.toLowerCase();
        const sentinelStart = lower.indexOf(BLAZOR_SSR_END_CLOSE);

        // Not safe until </blazor-ssr-end> is present
        if (sentinelStart < 0) {
            return null;
        }

        const sentinelEnd = sentinelStart + BLAZOR_SSR_END_CLOSE.length;

        // Consume whitespace immediately after sentinel (belongs with the wrapper)
        let consumed = sentinelEnd;
        const afterSentinel = buffer.slice(consumed);
        const wsMatch = afterSentinel.match(/^\s*/);
        consumed += (wsMatch ? wsMatch[0].length : 0);

        let htmlForSink = buffer.slice(0, consumed);

        const remainder = buffer.slice(consumed);
        const remainderLower = remainder.toLowerCase();
        const closeLower = BLAZOR_SSR_CLOSE.toLowerCase();

        // If the real close is already here, consume it too (best case)
        if (remainderLower.startsWith(closeLower)) {
            htmlForSink += buffer.slice(consumed, consumed + BLAZOR_SSR_CLOSE.length);
            consumed += BLAZOR_SSR_CLOSE.length;

            return { htmlForSink, consumed, syntheticClose: false };
        }

        // If remainder is a partial prefix of </blazor-ssr>, wait for more (avoid split-tag issues)
        if (!isFinal && remainder.length > 0 && closeLower.startsWith(remainderLower)) {
            return null;
        }

        // Otherwise, synthesize the close so the fragment parses as a complete wrapper now
        htmlForSink += BLAZOR_SSR_CLOSE;

        return { htmlForSink, consumed, syntheticClose: true };
    }

    function stripLeadingStandaloneSsrClosers(buffer) {
        let value = buffer;

        while (true) {
            const match = value.match(/^\s*<\/blazor-ssr>/i);
            if (!match) {
                break;
            }
            value = value.slice(match[0].length);
        }

        return value;
    }

    // --------------------------------------------------------------------------------------------
    // DOM sink
    // --------------------------------------------------------------------------------------------

    function appendStreamingBlockToSink(state, blockHtml) {
        const sink = getOrCreateSink(state);
        sink.insertAdjacentHTML("beforeend", blockHtml);
    }

    function getOrCreateSink(state) {
        if (state.sink && state.sink.isConnected) {
            return state.sink;
        }

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
    // Blazor template processing + marker patching
    // --------------------------------------------------------------------------------------------

    function insertStreamingContentIntoDocument(componentIdAsString, docFrag) {
        const markers = findStreamingMarkers(componentIdAsString);
        if (!markers) {
            return;
        }

        const { startMarker, endMarker } = markers;
        enableDomPreservation = !isCommentNodeInHead(startMarker);

        if (enableDomPreservation) {
            // Placeholder for future synchronizeDomContent port. Safe fallback:
            replaceMarkerRange(startMarker, endMarker, docFrag);
        } else {
            replaceMarkerRange(startMarker, endMarker, docFrag);
        }
    }

    function replaceMarkerRange(startMarker, endMarker, docFrag) {
        const destinationRoot = endMarker.parentNode;
        if (!destinationRoot) {
            return;
        }

        const existingContent = new Range();
        existingContent.setStart(startMarker, startMarker.textContent?.length || 0);
        existingContent.setEnd(endMarker, 0);
        existingContent.deleteContents();

        const insertedElements = [];

        while (docFrag.childNodes[0]) {
            const node = docFrag.childNodes[0];
            destinationRoot.insertBefore(node, endMarker);

            if (node.nodeType === Node.ELEMENT_NODE) {
                insertedElements.push(node);
            }
        }

        // Because insertion bypasses htmx.swap(), process inserted subtrees.
        for (const el of insertedElements) {
            htmx.process(el);
        }
    }

    function findStreamingMarkers(componentIdAsString) {
        const expectedStartText = `bl:${componentIdAsString}`;
        const iterator = document.createNodeIterator(document, NodeFilter.SHOW_COMMENT);

        let startMarker = null;
        while ((startMarker = iterator.nextNode())) {
            if (startMarker.textContent === expectedStartText) {
                break;
            }
        }

        if (!startMarker) {
            return null;
        }

        const expectedEndText = `/bl:${componentIdAsString}`;
        let endMarker = null;

        while ((endMarker = iterator.nextNode())) {
            if (endMarker.textContent === expectedEndText) {
                break;
            }
        }

        return endMarker ? { startMarker, endMarker } : null;
    }

    function isCommentNodeInHead(commentNode) {
        if (!commentNode || commentNode.nodeType !== Node.COMMENT_NODE) {
            return false;
        }

        let currentNode = commentNode.parentNode;
        while (currentNode) {
            if (currentNode === document.head) {
                return true;
            }
            currentNode = currentNode.parentNode;
        }

        return false;
    }

    function handleControlTemplate(node) {
        const type = node.getAttribute("type");
        const text = (node.content.textContent || "").trim();

        switch (type) {
            case "redirection":
            case "not-found":
                if (text) {
                    location.replace(text);
                }
                break;

            case "error":
                // Blazor typically shows an error UI; keep it simple.
                document.body.textContent = text || "Error";
                break;
        }
    }

    // --------------------------------------------------------------------------------------------
    // HX-* response header handling (minimal but important)
    // --------------------------------------------------------------------------------------------

    function applySimpleResponseOverrides(ctx) {
        if (!ctx?.hx) {
            return;
        }

        // Apply basic retarget/reswap/reselect overrides.
        if (ctx.hx.retarget) {
            ctx.target = ctx.hx.retarget;
        }

        if (ctx.hx.reswap) {
            ctx.swap = ctx.hx.reswap;
        }

        if (ctx.hx.reselect) {
            ctx.select = ctx.hx.reselect;
        }

        // Fire HX-Trigger immediately (matches core behavior timing).
        fireSimpleTriggerHeader(ctx.hx.trigger, ctx.sourceElement);
    }

    function handleImmediateResponseActions(ctx) {
        if (!ctx?.hx) {
            return false;
        }

        if (ctx.hx.refresh === "true") {
            location.reload();
            return true;
        }

        if (ctx.hx.redirect) {
            location.href = ctx.hx.redirect;
            return true;
        }

        // Minimal HX-Location support: issue a GET to the path.
        if (ctx.hx.location) {
            let path = ctx.hx.location;

            try {
                if (path.trim().startsWith("{")) {
                    const obj = JSON.parse(path);
                    path = obj.path || path;
                }
            } catch {
                // ignore
            }

            htmx.ajax("GET", path, {
                source: ctx.sourceElement,
                target: ctx.target
            });

            return true;
        }

        return false;
    }

    function fireSimpleTriggerHeader(rawValue, defaultTarget) {
        if (!rawValue) {
            return;
        }

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
            if (evt) {
                htmx.trigger(defaultTarget, evt, {});
            }
        });
    }

    // --------------------------------------------------------------------------------------------
    // Stream state + cleanup
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

        try {
            state.reader?.cancel?.();
        } catch {
            // ignore
        }

        cleanupStreamState(state, reason || "cancelled");
    }

    function cleanupStreamState(state, reason) {
        state.cancelled = true;

        try {
            state.reader?.cancel?.();
        } catch {
            // ignore
        }

        if (state.abortHandler) {
            state.sourceElement?.removeEventListener("htmx:abort", state.abortHandler);
        }

        if (state.sink?.isConnected) {
            state.sink.remove();
        }

        clearStreamState(state.sourceElement);
        htmx.trigger(state.sourceElement, "htmx:rizzy:stream:close", { reason });
    }
})();