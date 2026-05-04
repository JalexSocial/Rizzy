(function() {
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
    htmx_after_request: function(_elt, detail) {
      let documentNonce = htmx.config.documentNonce ?? htmx.config.inlineScriptNonce;
      if (!documentNonce) {
        console.warn("rizzy-nonce extension loaded but no nonce found for document. Inline scripts may be blocked.");
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
      nonce ?? (nonce = "");
      if (typeof detail?.ctx?.text === "string") {
        detail.ctx.text = processUnsafeHtml(detail.ctx.text, documentNonce, nonce);
      }
      return true;
    }
  });
})();
(function() {
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
      applySimpleResponseOverrides(ctx);
      if (handleImmediateResponseActions(ctx)) return false;
      startStreamingHandler(ctx).catch((error) => {
        cancelStream(ctx.sourceElement, "error");
        htmx.trigger(ctx.sourceElement, "htmx:error", { ctx, error });
      });
      return false;
    },
    htmx_before_cleanup(elt) {
      cancelStream(elt, "cleanup");
      return true;
    }
  });
  function ensureBlazorSsrEndElement() {
    if (customElements.get("blazor-ssr-end")) return;
    customElements.define("blazor-ssr-end", class BlazorStreamingUpdate extends HTMLElement {
      connectedCallback() {
        const blazorSsrElement = this.parentNode;
        if (!blazorSsrElement || blazorSsrElement.nodeType !== Node.ELEMENT_NODE) return;
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
      if (!state.didPrimarySwap) {
        const split = findPrimarySplitPoint(state.buffer);
        if (split === -1) {
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
      const next = findNextCompleteSsrBlock(state.buffer);
      if (!next) {
        if (isFinal) {
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
      if (next.start > 0) {
        const prefix = state.buffer.slice(0, next.start);
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
      await Promise.resolve();
      htmx.trigger(ctx.sourceElement, "htmx:rizzy:stream:block", {});
    }
  }
  function findPrimarySplitPoint(buffer) {
    const lower = buffer.toLowerCase();
    const htmlEnd = lower.indexOf(HTML_CLOSE);
    if (htmlEnd >= 0) return htmlEnd + HTML_CLOSE.length;
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
      state.didPrimarySwap = true;
      return;
    }
    ctx.text = html;
    await htmx.swap(ctx);
    state.didPrimarySwap = true;
    fireDeferredTriggerHeaders(ctx);
  }
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
  function insertStreamingContentIntoDocument(componentIdAsString, docFrag) {
    const markers = findStreamingMarkers(componentIdAsString);
    if (!markers) {
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
    for (const el of insertedElements) {
      htmx.process(el);
    }
  }
  function findStreamingMarkers(componentIdAsString) {
    const expectedStartText = `bl:${componentIdAsString}`;
    const iterator = document.createNodeIterator(document, NodeFilter.SHOW_COMMENT);
    let startMarker = null;
    while (startMarker = iterator.nextNode()) {
      if (startMarker.textContent === expectedStartText) break;
    }
    if (!startMarker) return null;
    const expectedEndText = `/bl:${componentIdAsString}`;
    let endMarker = null;
    while (endMarker = iterator.nextNode()) {
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
      } catch {
      }
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
    }
    rawValue.split(",").forEach((name) => {
      const evt = name.trim();
      if (evt) htmx.trigger(defaultTarget, evt, {});
    });
  }
  function setStreamState(elt, state) {
    if (!elt) return;
    elt._htmx || (elt._htmx = {});
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
    }
    cleanupStreamState(state, reason || "cancelled");
  }
  function cleanupStreamState(state, reason) {
    if (!state || state.cleanedUp) return;
    state.cleanedUp = true;
    state.cancelled = true;
    try {
      state.reader?.cancel?.();
    } catch {
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
(() => {
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
      const method = (req.method || "").toUpperCase();
      if (method === "GET" || method === "HEAD") return;
      const antiforgery = htmx.config?.antiforgery;
      if (!antiforgery) return;
      const { headerName, requestToken, formFieldName } = antiforgery;
      if (!requestToken) return;
      if (!headerName && !formFieldName) return;
      if (headerName) {
        req.headers || (req.headers = {});
        if (typeof req.headers.set === "function") {
          req.headers.set(headerName, requestToken);
        } else {
          req.headers[headerName] = requestToken;
        }
        return;
      }
      if (method !== "POST" && method !== "PUT" && method !== "PATCH") return;
      const body = req.body;
      if (!body) return;
      if (typeof body.has === "function" && body.has(formFieldName)) return;
      if (typeof body.append === "function") {
        body.append(formFieldName, requestToken);
      } else if (typeof body.set === "function") {
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
      const sourceBoosted = !!ctx.sourceElement?._htmx?.boosted;
      const headerBoosted = ctx.request?.headers?.["HX-Boosted"] === "true";
      const boosted = sourceBoosted || headerBoosted;
      if (!boosted) return;
      let doc;
      try {
        doc = new DOMParser().parseFromString(ctx.text, "text/html");
      } catch {
        return;
      }
      const meta = doc.querySelector('meta[name="htmx-config"]');
      const contentValue = meta?.getAttribute("content");
      if (!contentValue) return;
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
(function() {
  const EXT_NAME = "rizzy-confirm";
  function confirm(props = {}) {
    return new Promise((resolve) => {
      const result = {
        isConfirmed: false,
        isDenied: false,
        isDismissed: false,
        value: null
      };
      const resultEvents = {
        confirm: function(value) {
          result.isConfirmed = true;
          result.value = value ?? null;
          resolve(result);
        },
        deny: function() {
          result.isDenied = true;
          resolve(result);
        },
        dismiss: function() {
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
  function parseConfirmValue(confirmValue) {
    if (!confirmValue) return null;
    if (typeof confirmValue === "object") return confirmValue;
    if (typeof confirmValue !== "string") {
      return { title: "Proceed?", text: String(confirmValue) };
    }
    const trimmed = confirmValue.trim();
    if (!trimmed) return null;
    if (trimmed.startsWith("{") || trimmed.startsWith("[")) {
      try {
        return JSON.parse(trimmed);
      } catch {
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
      if (!confirmValue) return;
      detail.cancelled = true;
      const props = parseConfirmValue(confirmValue);
      if (!props) {
        detail.issueRequest();
        return false;
      }
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
})();
//# sourceMappingURL=rizzy.es.mjs.map
