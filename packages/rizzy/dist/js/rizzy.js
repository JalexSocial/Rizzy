(function(factory) {
  typeof define === "function" && define.amd ? define(factory) : factory();
})((function() {
  "use strict";
  (function() {
    const root = window.Rizzy = window.Rizzy || {};
    const diagnostics = root.diagnostics = root.diagnostics || {};
    const CLIENT_MAJOR = 6;
    diagnostics.clientMajor = CLIENT_MAJOR;
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
    const registered = diagnostics.registeredExtensions = diagnostics.registeredExtensions || /* @__PURE__ */ new Set();
    const rejected = diagnostics.rejectedExtensions = diagnostics.rejectedExtensions || /* @__PURE__ */ new Set();
    if (typeof htmx !== "undefined" && typeof htmx.registerExtension === "function" && !htmx.registerExtension._rizzyTracked) {
      const original = htmx.registerExtension.bind(htmx);
      const tracked = function registerExtension(name, extension) {
        const result = original(name, extension);
        if (result === false) {
          if (!registered.has(name)) rejected.add(name);
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
    diagnostics.checkPage = function checkPage(doc = document) {
      const warnings = [];
      const meta = doc.querySelector('meta[name="htmx-config"]');
      if (!meta) {
        warnings.push(
          'Rizzy: no <meta name="htmx-config"> on this page, so htmx has no antiforgery token or Rizzy settings. Render <RzHeadOutlet /> instead of <HeadOutlet /> in the <head> of your root layout, before the htmx script (Rizzy 6 no longer adds the tag through HeadContent).'
        );
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
            `Rizzy: htmx rejected the "${name}" extension because htmx.config.extensions ("${config?.extensions ?? ""}") does not list it. Enable extensions with HtmxConfig.UseExtensions(...), which keeps the allow-list in step, or add the name to HtmxConfig.Extensions.`
          );
        }
        for (const name of config?.rizzyExtensions ?? []) {
          if (!registered.has(name) && !rejected.has(name)) {
            warnings.push(
              `Rizzy: the "${name}" extension is enabled on the server but rizzy.js did not see it register. Render <RzScripts /> in the root layout (after <RzHeadOutlet />). If you load the scripts yourself, load the extension after htmx and after rizzy.js: an extension registered before rizzy.js is not tracked and is reported here even though it works.`
            );
          }
        }
        const serverMajor = majorOf(config?.rizzyVersion);
        if (serverMajor !== null && serverMajor !== CLIENT_MAJOR) {
          warnings.push(
            `Rizzy: the server runs Rizzy ${config.rizzyVersion} but rizzy.js is for Rizzy ${CLIENT_MAJOR}.x. Load the rizzy.js shipped with the Rizzy package (_content/Rizzy/js/rizzy.js) or the matching @jalexsocial/rizzy version.`
          );
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
  (function() {
    const root = window.Rizzy = window.Rizzy || {};
    const nonceApi = root.nonce = root.nonce || {};
    let cachedPageNonce;
    function escapeRegex(value) {
      return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    }
    nonceApi.getDocumentNonce = function getDocumentNonce() {
      if (cachedPageNonce) return cachedPageNonce;
      const fromScript = document.querySelector("script[nonce]")?.nonce || document.querySelector("script[nonce]")?.getAttribute("nonce") || "";
      const fromConfig = typeof htmx !== "undefined" && htmx.config?.documentNonce || "";
      cachedPageNonce = fromScript || fromConfig || "";
      return cachedPageNonce;
    };
    nonceApi.getResponseNonce = function getResponseNonce(responseOrHeaders) {
      const headers = responseOrHeaders?.headers?.get ? responseOrHeaders.headers : responseOrHeaders;
      const nonce = headers?.get?.("HX-Nonce") || "";
      if (nonce) return nonce;
      const csp = headers?.get?.("Content-Security-Policy") || "";
      return csp.match(/(?:script-src|style-src|default-src)[^;]*'nonce-([^']+)'/i)?.[1] || "";
    };
    nonceApi.rewriteNonces = function rewriteNonces(text, documentNonce, responseNonce) {
      if (typeof text !== "string" || !text || !documentNonce) return text ?? "";
      const attr = (value) => new RegExp(`(\\snonce\\s*=\\s*)(["']?)${escapeRegex(value)}\\2(?=[\\s/>])`, "gi");
      if (responseNonce !== documentNonce) {
        text = text.replace(attr(documentNonce), "");
      }
      if (responseNonce && responseNonce !== documentNonce) {
        text = text.replace(attr(responseNonce), (_, prefix, quote) => `${prefix}${quote}${documentNonce}${quote}`);
      }
      return text;
    };
    nonceApi.processResponseHtml = function processResponseHtml(text, responseOrHeaders) {
      return nonceApi.rewriteNonces(text, nonceApi.getDocumentNonce(), nonceApi.getResponseNonce(responseOrHeaders));
    };
    htmx.registerExtension("rizzy-nonce", {
      htmx_after_request(_elt, detail) {
        const ctx = detail?.ctx;
        if (typeof ctx?.text === "string") {
          ctx.text = nonceApi.processResponseHtml(ctx.text, ctx.response);
        }
      }
    });
  })();
  (function() {
    const report = (elt, name, detail) => window.Rizzy?.diagnostics?.report ? window.Rizzy.diagnostics.report(elt, name, detail) : htmx.trigger(elt, name, detail);
    const STREAM_SINK_ATTR = "data-rizzy-stream-sink";
    const HTML_CLOSE = "</html>";
    const SSR_OPEN = "<blazor-ssr";
    const SSR_CLOSE = "</blazor-ssr>";
    const NULL_BODY_STATUSES = /* @__PURE__ */ new Set([101, 103, 204, 205, 304]);
    const streams = /* @__PURE__ */ new WeakMap();
    ensureBlazorSsrEndElement();
    htmx.registerExtension("rizzy-streaming", {
      htmx_before_request(_elt, detail) {
        const ctx = detail?.ctx;
        if (!ctx || typeof ctx.fetch !== "function" || ctx.fetch._rizzyStreaming) return;
        const layers = [ctx.fetch];
        let depth = 0;
        const wrapped = async (url, init) => {
          if (depth > 0) {
            const layer = layers[Math.max(0, layers.length - 1 - depth)];
            depth++;
            return layer(url, init);
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
          }
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
      try {
        Object.defineProperty(response, "url", { value: original.url });
        Object.defineProperty(response, "redirected", { value: original.redirected });
      } catch {
      }
      return response;
    }
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
      ctx.extensionPromise = readRemainder(state, decoder).catch((error) => {
        if (error?.name !== "AbortError") {
          report(ctx.sourceElement, "htmx:error", { ctx, error });
        }
      }).finally(() => closeStream(state));
    }
    async function readRemainder(state, decoder) {
      collectBlocks(state, false);
      while (true) {
        const { done, value } = await state.reader.read();
        if (done) break;
        if (!state.opened) {
          state.opened = true;
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
        state.buffer = "";
      }
      if (state.swapped) flushPendingBlocks(state);
    }
    function flushPendingBlocks(state) {
      while (state.pending.length) {
        const blockHtml = state.pending.shift();
        const html = window.Rizzy?.nonce?.processResponseHtml ? window.Rizzy.nonce.processResponseHtml(blockHtml, state.response) : blockHtml;
        getOrCreateSink(state).insertAdjacentHTML("beforeend", html);
        htmx.trigger(state.ctx.sourceElement, "htmx:rizzy:stream:block", { ctx: state.ctx });
      }
    }
    function closeStream(state) {
      state.pending.length = 0;
      if (state.sink?.isConnected) state.sink.remove();
      streams.delete(state.ctx);
      if (state.opened) {
        htmx.trigger(state.ctx.sourceElement, "htmx:rizzy:stream:close", { ctx: state.ctx });
      }
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
      for (const el of insertedElements) htmx.process(el);
    }
    function findStreamingMarkers(componentIdAsString) {
      const expectedStartText = `bl:${componentIdAsString}`;
      const expectedEndText = `/bl:${componentIdAsString}`;
      const iterator = document.createNodeIterator(document, NodeFilter.SHOW_COMMENT);
      let startMarker = null;
      while (startMarker = iterator.nextNode()) {
        if (startMarker.textContent === expectedStartText) break;
      }
      if (!startMarker) return null;
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
          const error = new Error(`invalid htmx-config JSON in the boosted response: ${e.message}`);
          if (window.Rizzy?.diagnostics?.report) {
            window.Rizzy.diagnostics.report(elt, "htmx:rizzy:antiforgery:error", { error });
          } else {
            console.error("[rizzy-antiforgery]", error);
          }
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
    function requestDialog(props) {
      let resolveResult;
      const promise = new Promise((resolve) => {
        resolveResult = resolve;
      });
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
    function confirm(props = {}) {
      const { handled, promise } = requestDialog(props);
      if (handled) return promise;
      const confirmed = window.confirm(nativeMessage(props));
      return Promise.resolve({ isConfirmed: confirmed, isDenied: false, isDismissed: !confirmed, value: null });
    }
    function nativeMessage(props) {
      return [props.title, props.text].filter(Boolean).join("\n\n") || defaultEventData.title;
    }
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
}));
//# sourceMappingURL=rizzy.js.map
