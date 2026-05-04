(function() {
  function x(p, f, c) {
    f && c && (p = p.replaceAll(c, f));
    const o = new DOMParser();
    try {
      const a = o.parseFromString(p, "text/html");
      if (a)
        return a.querySelectorAll("script, style, link").forEach((u) => {
          u.getAttribute("nonce") !== f && u.remove();
        }), a.documentElement.outerHTML;
    } catch {
    }
    return "";
  }
  htmx.registerExtension("rizzy-nonce", {
    htmx_after_request: function(p, f) {
      let c = htmx.config.documentNonce ?? htmx.config.inlineScriptNonce;
      c || (console.warn("rizzy-nonce extension loaded but no nonce found for document. Inline scripts may be blocked."), c = "");
      let o = f?.ctx?.response?.headers?.get("HX-Nonce");
      if (!o) {
        const a = f?.ctx?.response?.headers?.get("content-security-policy");
        if (a) {
          const l = a.match(/(style|script)-src[^;]*'nonce-([^']*)'/i);
          l && (o = l[2]);
        }
      }
      return o ?? (o = ""), typeof f?.ctx?.text == "string" && (f.ctx.text = x(f.ctx.text, c, o)), !0;
    }
  });
})();
(function() {
  const x = "_rizzyStreaming", p = "data-rizzy-stream-sink", f = "</html>", c = "<blazor-ssr", o = "</blazor-ssr>";
  a(), htmx.registerExtension("rizzy-streaming", {
    htmx_before_request(e, t) {
      const r = t?.ctx?.sourceElement || e;
      return y(r, "superseded"), !0;
    },
    htmx_before_response(e, t) {
      const r = t?.ctx, n = r?.response?.raw;
      if (!r || !n?.body) return !0;
      const i = n.headers.get("Content-Type") || "";
      return /text\/html/i.test(i) ? (k(r), N(r) || l(r).catch((s) => {
        y(r.sourceElement, "error"), htmx.trigger(r.sourceElement, "htmx:error", { ctx: r, error: s });
      }), !1) : !0;
    },
    htmx_before_cleanup(e) {
      return y(e, "cleanup"), !0;
    }
  });
  function a() {
    customElements.get("blazor-ssr-end") || customElements.define("blazor-ssr-end", class extends HTMLElement {
      connectedCallback() {
        const t = this.parentNode;
        if (!t || t.nodeType !== Node.ELEMENT_NODE) return;
        t.parentNode?.removeChild(t);
        const r = Array.from(t.childNodes);
        for (const n of r) {
          if (!(n instanceof HTMLTemplateElement)) continue;
          const i = n.getAttribute("blazor-component-id");
          i ? T(i, n.content) : z(n);
        }
      }
    });
  }
  async function l(e) {
    const t = e.sourceElement, n = e.response.raw.body.getReader(), i = new TextDecoder(), s = {
      sourceElement: t,
      reader: n,
      cancelled: !1,
      cleanedUp: !1,
      abortHandler: null,
      sink: null,
      buffer: "",
      didPrimarySwap: !1
    };
    s.abortHandler = () => y(t, "abort"), t.addEventListener("htmx:abort", s.abortHandler), O(t, s), htmx.trigger(t, "htmx:rizzy:stream:open", { ctx: e });
    try {
      for (; !s.cancelled; ) {
        const { done: g, value: v } = await n.read();
        if (g) break;
        s.buffer += i.decode(v, { stream: !0 }), await u(e, s, !1);
      }
      s.buffer += i.decode(), await u(e, s, !0);
    } finally {
      w(s, "ended");
    }
  }
  async function u(e, t, r) {
    for (; !t.cancelled; ) {
      if (!t.didPrimarySwap) {
        const s = d(t.buffer);
        if (s === -1) {
          r && t.buffer && (await h(e, t, t.buffer), t.buffer = "");
          return;
        }
        const g = t.buffer.slice(0, s);
        await h(e, t, g), t.buffer = t.buffer.slice(s);
        continue;
      }
      const n = m(t.buffer);
      if (!n) {
        r && (t.buffer.trim() && await htmx.swap({
          sourceElement: e.sourceElement,
          target: e.target,
          text: t.buffer,
          swap: "beforeend",
          transition: !1,
          response: e.response,
          hx: e.hx,
          push: e.push,
          replace: e.replace
        }), t.buffer = "");
        return;
      }
      if (n.start > 0) {
        const s = t.buffer.slice(0, n.start);
        /\S/.test(s) && await htmx.swap({
          sourceElement: e.sourceElement,
          target: e.target,
          text: s,
          swap: "beforeend",
          transition: !1,
          response: e.response,
          hx: e.hx,
          push: e.push,
          replace: e.replace
        }), t.buffer = t.buffer.slice(n.start);
        continue;
      }
      const i = t.buffer.slice(n.start, n.end);
      t.buffer = t.buffer.slice(n.end), b(t, i), await Promise.resolve(), htmx.trigger(e.sourceElement, "htmx:rizzy:stream:block", {});
    }
  }
  function d(e) {
    const t = e.toLowerCase(), r = t.indexOf(f);
    if (r >= 0) return r + f.length;
    const n = t.indexOf(c);
    return n >= 0 ? n : -1;
  }
  function m(e) {
    const t = e.toLowerCase(), r = t.indexOf(c);
    if (r < 0) return null;
    const n = t.indexOf(o, r);
    if (n < 0) return null;
    const i = n + o.length;
    return { start: r, end: i };
  }
  async function h(e, t, r) {
    if (!r) {
      t.didPrimarySwap = !0;
      return;
    }
    e.text = r, await htmx.swap(e), t.didPrimarySwap = !0, A(e);
  }
  function b(e, t) {
    E(e).insertAdjacentHTML("beforeend", t);
  }
  function E(e) {
    if (e.sink && e.sink.isConnected) return e.sink;
    const t = document.createElement("div");
    return t.hidden = !0, t.style.display = "none", t.setAttribute("aria-hidden", "true"), t.setAttribute(p, "true"), document.body.appendChild(t), e.sink = t, t;
  }
  function T(e, t) {
    const r = C(e);
    if (!r)
      return;
    const { startMarker: n, endMarker: i } = r;
    _(n, i, t);
  }
  function _(e, t, r) {
    const n = t.parentNode;
    if (!n) return;
    const i = new Range();
    i.setStart(e, e.textContent?.length || 0), i.setEnd(t, 0), i.deleteContents();
    const s = [];
    for (; r.firstChild; ) {
      const g = r.firstChild;
      n.insertBefore(g, t), g.nodeType === Node.ELEMENT_NODE && s.push(g);
    }
    for (const g of s)
      htmx.process(g);
  }
  function C(e) {
    const t = `bl:${e}`, r = document.createNodeIterator(document, NodeFilter.SHOW_COMMENT);
    let n = null;
    for (; (n = r.nextNode()) && n.textContent !== t; )
      ;
    if (!n) return null;
    const i = `/bl:${e}`;
    let s = null;
    for (; (s = r.nextNode()) && s.textContent !== i; )
      ;
    return s ? { startMarker: n, endMarker: s } : null;
  }
  function z(e) {
    const t = e.getAttribute("type"), r = (e.content.textContent || "").trim();
    switch (t) {
      case "redirection":
      case "not-found":
        r && location.replace(r);
        break;
      case "error":
        document.body.textContent = r || "Error";
        break;
    }
  }
  function k(e) {
    e?.hx && (e.hx.retarget && (e.target = e.hx.retarget), e.hx.reswap && (e.swap = e.hx.reswap), e.hx.reselect && (e.select = e.hx.reselect), S(e.hx.trigger, e.sourceElement));
  }
  function N(e) {
    if (!e?.hx) return !1;
    if (e.hx.refresh === "true")
      return location.reload(), !0;
    if (e.hx.redirect)
      return location.href = e.hx.redirect, !0;
    if (e.hx.location) {
      let t = e.hx.location;
      try {
        t.trim().startsWith("{") && (t = JSON.parse(t).path || t);
      } catch {
      }
      return htmx.ajax("GET", t, { source: e.sourceElement, target: e.target }), !0;
    }
    return !1;
  }
  function A(e) {
    S(e?.hx?.triggerafterswap, e?.sourceElement), S(e?.hx?.triggeraftersettle, e?.sourceElement);
  }
  function S(e, t) {
    if (e) {
      try {
        if (e.trim().startsWith("{")) {
          const r = JSON.parse(e);
          for (const [n, i] of Object.entries(r)) {
            let s = t;
            i && typeof i == "object" && i.target && (s = htmx.find(i.target) || t), htmx.trigger(s, n, typeof i == "object" ? i : { value: i });
          }
          return;
        }
      } catch {
      }
      e.split(",").forEach((r) => {
        const n = r.trim();
        n && htmx.trigger(t, n, {});
      });
    }
  }
  function O(e, t) {
    e && (e._htmx || (e._htmx = {}), e._htmx[x] = t);
  }
  function H(e) {
    return e?._htmx?.[x] || null;
  }
  function M(e) {
    e?._htmx?.[x] && delete e._htmx[x];
  }
  function y(e, t) {
    const r = H(e);
    if (r) {
      r.cancelled = !0;
      try {
        r.reader?.cancel?.();
      } catch {
      }
      w(r, t || "cancelled");
    }
  }
  function w(e, t) {
    if (!(!e || e.cleanedUp)) {
      e.cleanedUp = !0, e.cancelled = !0;
      try {
        e.reader?.cancel?.();
      } catch {
      }
      e.abortHandler && e.sourceElement?.removeEventListener("htmx:abort", e.abortHandler), e.sink?.isConnected && e.sink.remove(), M(e.sourceElement), htmx.trigger(e.sourceElement, "htmx:rizzy:stream:close", { reason: t });
    }
  }
})();
htmx.registerExtension("rizzy-antiforgery", {
  /**
   * htmx:config:request
   * Runs after htmx has built ctx.request (method/action/headers/body), but before it finalizes
   * query-param conversions (GET/DELETE) and before the request is issued.
   *
   * Goal: Add antiforgery token to request headers or body.
   */
  htmx_config_request: (p, f) => {
    const o = f?.ctx?.request;
    if (!o) return;
    const a = (o.method || "").toUpperCase();
    if (a === "GET" || a === "HEAD") return;
    const l = htmx.config?.antiforgery;
    if (!l) return;
    const { headerName: u, requestToken: d, formFieldName: m } = l;
    if (!d || !u && !m) return;
    if (u) {
      o.headers || (o.headers = {}), typeof o.headers.set == "function" ? o.headers.set(u, d) : o.headers[u] = d;
      return;
    }
    if (a !== "POST" && a !== "PUT" && a !== "PATCH") return;
    const h = o.body;
    h && (typeof h.has == "function" && h.has(m) || (typeof h.append == "function" ? h.append(m, d) : typeof h.set == "function" && h.set(m, d)));
  },
  /**
   * htmx:after:swap
   * Runs after htmx has performed the swap. This is a safer point than after:request
   * because swaps can be cancelled or bypassed by headers (redirect/refresh) or status rules.
   *
   * Goal: If this was a boosted (full-page-ish) navigation, refresh antiforgery config
   * from the server-provided meta config in the response text.
   */
  htmx_after_swap: (p, f) => {
    const c = f?.ctx;
    if (!c?.text) return;
    const o = !!c.sourceElement?._htmx?.boosted, a = c.request?.headers?.["HX-Boosted"] === "true";
    if (!(o || a)) return;
    let u;
    try {
      u = new DOMParser().parseFromString(c.text, "text/html");
    } catch {
      return;
    }
    const m = u.querySelector('meta[name="htmx-config"]')?.getAttribute("content");
    if (!m) return;
    try {
      const E = JSON.parse(m)?.antiforgery;
      if (!E) return;
      htmx.config.antiforgery = E;
    } catch (b) {
      console.error("[rizzy-antiforgery] Error parsing htmx-config JSON:", b);
      return;
    }
    const h = document.querySelector('meta[name="htmx-config"]');
    if (h)
      h.setAttribute("content", m);
    else {
      const b = document.createElement("meta");
      b.setAttribute("name", "htmx-config"), b.setAttribute("content", m), document.head.appendChild(b);
    }
  }
});
(function() {
  const x = "rizzy-confirm";
  function p(c = {}) {
    return new Promise((o) => {
      const a = {
        isConfirmed: !1,
        isDenied: !1,
        isDismissed: !1,
        value: null
      }, d = { ...{
        title: "Proceed?",
        text: "",
        showConfirmButton: !0,
        showDenyButton: !1,
        showCancelButton: !1,
        confirmButtonText: "OK",
        denyButtonText: "No",
        dismissButtonText: "Cancel"
      }, ...c, ...{
        confirm: function(m) {
          a.isConfirmed = !0, a.value = m ?? null, o(a);
        },
        deny: function() {
          a.isDenied = !0, o(a);
        },
        dismiss: function() {
          a.isDismissed = !0, o(a);
        }
      } };
      window.dispatchEvent(
        new CustomEvent("rz:confirm-dialog", {
          detail: d
        })
      );
    });
  }
  function f(c) {
    if (!c) return null;
    if (typeof c == "object") return c;
    if (typeof c != "string")
      return { title: "Proceed?", text: String(c) };
    const o = c.trim();
    if (!o) return null;
    if (o.startsWith("{") || o.startsWith("["))
      try {
        return JSON.parse(o);
      } catch {
      }
    return { title: "Proceed?", text: c };
  }
  htmx.registerExtension(x, {
    /**
     * htmx_confirm maps to the "htmx:confirm" hook in htmx 4
     * (colons become underscores in extension method names).
     *
     * In the htmx 4 source you pasted, the confirm hook detail is:
     *   { ctx, issueRequest: () => resolve(true), dropRequest: () => resolve(false) }
     *
     * We must call issueRequest() to continue or dropRequest() to cancel.
     */
    htmx_confirm: (c, o) => {
      const l = o?.ctx?.confirm;
      if (!l) return;
      o.cancelled = !0;
      const u = f(l);
      return u ? (p(u).then((d) => {
        d.isConfirmed ? o.issueRequest() : o.dropRequest();
      }), !1) : (o.issueRequest(), !1);
    }
  });
})();
