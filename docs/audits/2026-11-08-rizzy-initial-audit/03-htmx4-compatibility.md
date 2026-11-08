# 03 — htmx 4 Compatibility Assessment

| Field | Value |
|---|---|
| Audit | Rizzy initial audit, 2026-11-08 |
| Repository / commit | https://github.com/JalexSocial/Rizzy @ `7e3f0f1126ee412d9db4f37d13d0370159edac15` |
| Scope | All Rizzy code that touches htmx: C# (`Rizzy.Htmx`, `Rizzy` layouts/components), JS extensions (`packages/rizzy/src/js`), demo markup, docs |
| htmx version assessed | **4.0.1** (npm `next`, 2026-11-01). npm `latest` is still **2.0.11**. Rizzy's lockfile pins **4.0.0-beta4**. The demo and READMEs load **4.0.0-beta2** from a CDN |
| Sources | [llms.txt](https://four.htmx.org/llms.txt), [llms-full.txt](https://four.htmx.org/llms-full.txt), [docs](https://four.htmx.org/docs/), [What's New in htmx 4](https://four.htmx.org/docs/whats-new-in-htmx-4/), per-header/attribute/config reference pages, and the source of `htmx.org@4.0.1/dist/htmx.js` |
| Limitations | No browser execution. Behavior was established from the official docs and the 4.0.1 source, plus jsdom for the nonce sanitizer. See [00](00-executive-summary.md#important-limitations) |

## 1. Summary

The June 2026 migration (PR #25, against htmx 4 betas) got the **server protocol surface mostly right**:

- request headers `HX-Source`, `HX-Request-Type`, `HX-Target` in `tag#id` format;
- the `HX-Trigger-After-*` headers removed;
- htmx 4 config keys;
- `<hx-partial>` out-of-band rendering;
- `:inherited` in demo markup;
- JS extensions rewritten to `htmx.registerExtension` and the new event names.

What remains:

1. **Client extensions conflict with htmx 4 core.** They are always active in every Rizzy app (RIZZY-003, -004, -005).
2. **Navigation and history semantics.** htmx 4 has no history cache and always re-fetches full pages (RIZZY-006).
3. **Server builders emit htmx 2 attribute syntax** (RIZZY-007, RIZZY-013).
4. **Option and config models** drifted from htmx 4 (RIZZY-014, RIZZY-028).
5. **No strategy for "error responses swap"** (RIZZY-019).

### Automated inspection

htmx 4.0.1 ships an official checker, `dist/scripts/upgrade-check.py` (also `npx upgrade-check`). It was run read-only against `.razor`, `.cs`, `.cshtml`, `.js`, `.mjs` and `.md` files in `src/`, `packages/rizzy/src`, `packages/rizzy/dist` and `src/Rizzy/wwwroot/js`.

Result: **0 issues in 123 tracked files.** Its only hits were stale untracked `obj/Debug/net8.0` generated files.

The checker parses static HTML-like templates. It cannot see markup generated at runtime by `TriggerBuilder`/`SwapStyleBuilder`, header values, or JS extension behavior. Every finding below came from manual review against the docs and the 4.0.1 source, and where possible was confirmed by executing Rizzy code.

## 2. Compatibility matrix

Status legend: ✅ compatible · ⚠️ partially compatible / risk · ❌ incompatible.

| # | Subsystem | Current implementation | htmx 4 change | Status | Sev | Remediation | Required tests | Reference |
|---|---|---|---|---|---|---|---|---|
| 1 | Request detection | `IsHtmx()` checks `HX-Request` | Unchanged | ✅ | – | – | Existing | [HX-Request](https://four.htmx.org/reference/headers/HX-Request.md) |
| 2 | Request headers | `HtmxRequestHeaderNames`: Boosted, Current-URL, History-Restore-Request, Request, Target, Source, Request-Type | `HX-Trigger`→`HX-Source`, `HX-Trigger-Name` removed, `HX-Request-Type` added, `tag#id` format | ✅ names / ⚠️ parsing | P2 | Fix `TargetId`/`SourceId` for id-less elements and add typed request-type helpers (**RIZZY-017**) | Unit: `div`, `div#x`, missing | [What's New › Request headers](https://four.htmx.org/docs/whats-new-in-htmx-4/), [HX-Source](https://four.htmx.org/reference/headers/HX-Source.md) |
| 3 | Full vs partial rendering | `HtmxLayout<T>` drops the layout for **any** `HX-Request` | `HX-Request-Type: full` when the target is `body` or `hx-select` is used; boosted links target body | ❌ | P1 | Render the full layout when `RequestType==full` or history restore (**RIZZY-006**) | Pipeline: partial / full / boosted / restore | [HX-Request-Type](https://four.htmx.org/reference/headers/HX-Request-Type.md) |
| 4 | History restore | Same as #3. The demo has no `hx-history-elt` | **No history cache.** Every back/forward re-fetches with `HX-History-Restore-Request: true`, target `body` (swap `outerSync`), or `select: [hx-history-elt]` from a full page | ❌ | P1 | Full page on restore. Document `hx-history-elt`. Expose `HtmxConfig.History = Reload` as a fallback (**RIZZY-006**) | Pipeline plus browser back/forward | [What's New › No history cache](https://four.htmx.org/docs/whats-new-in-htmx-4/), [htmx.config.history](https://four.htmx.org/reference/config/htmx-config-history.md), 4.0.1 `#restoreHistory` |
| 5 | Response headers | `HtmxResponseHeaderNames`: Location, Push-Url, Redirect, Refresh, Replace-Url, Reswap, Retarget, Reselect, Trigger | `HX-Trigger-After-Swap/Settle` removed. `HX-Trigger` fires after swap. Headers not processed on 3xx | ✅ | – | Document the 3xx rule | Existing | [What's New › Response headers](https://four.htmx.org/docs/whats-new-in-htmx-4/) |
| 6 | `HX-Trigger` | String-concatenated JSON | JSON or HCON, `target` key supported | ⚠️ | P2 | JSON writer, escaping, merge semantics (**RIZZY-015**). Optional `target` support | Unit: escaping, merge | [HX-Trigger](https://four.htmx.org/reference/headers/HX-Trigger.md) |
| 7 | `HX-Location` | `LocationTarget : AjaxContext` with `values`/`headers` as **strings** and `handler` | Calls `htmx.ajax('GET', path, opts)`. `values`/`headers` are objects. Defaults `push:true`. Options: source, event, target, swap, select, selectOOB, transition, values, headers (push/replace) | ❌ | P2 | Typed object model (**RIZZY-014**) | Unit: serialized JSON shape | [HX-Location](https://four.htmx.org/reference/headers/HX-Location.md), [htmx.ajax()](https://four.htmx.org/reference/methods/htmx-ajax.md) |
| 8 | Trigger builder | `from:closest form`, `from:{.a .b}`, `target:{.a, .b}`, `queue:all` | `queue` modifier **removed** (use `hx-sync`). Selectors with spaces or commas must be quoted (`'…'`, `"…"` or `<…/>`). New: `prevent`, `stop`, `halt`, `capture`, `passive`, `rootMargin`, `from:outside` | ❌ | P1 | **RIZZY-007** | Unit: emitted strings | [hx-trigger](https://four.htmx.org/reference/attributes/hx-trigger.md), [What's New › queue removed / selector quoting](https://four.htmx.org/docs/whats-new-in-htmx-4/) |
| 9 | Swap builder | `focus-scroll:`, unquoted `scrollTarget`/`showTarget`, culture-formatted times | Modifier key is `focusScroll`. Selectors with spaces must be quoted. New modifiers `strip`, `target`, `swapEmpty`. New style `outerSync` | ⚠️ | P2 | **RIZZY-013**, **RIZZY-012** | Unit | [hx-swap](https://four.htmx.org/reference/attributes/hx-swap.md) |
| 10 | Swap styles enum | Includes innerMorph, outerMorph, textContent, delete, before/after/prepend/append aliases | Matches 4.0.1, but `outerSync` and `upsert` (extension) are missing | ⚠️ | P3 | Add `outerSync` | Unit | [hx-swap](https://four.htmx.org/reference/attributes/hx-swap.md) |
| 11 | OOB / partials | `HtmxSwappable` emits `<hx-partial hx-target hx-swap>` after main content | `<hx-partial>` supported. OOB swaps after main content | ✅ | – | CSS-escape target ids (RIZZY-029) | Pipeline: partial markup | [&lt;hx-partial&gt;](https://four.htmx.org/reference/tags/hx-partial.md), [What's New › OOB order](https://four.htmx.org/docs/whats-new-in-htmx-4/) |
| 12 | Attribute inheritance | Library emits no inherited attributes. The demo uses `hx-boost:inherited`, `hx-target:inherited`, etc. Config exposes `ImplicitInheritance` | Explicit inheritance by default | ✅ | – | Document the `:inherited` pattern and the `ImplicitInheritance` escape hatch | – | [What's New › Explicit inheritance](https://four.htmx.org/docs/whats-new-in-htmx-4/) |
| 13 | Error responses (4xx/5xx) | No server strategy. The demo reverts with `NoSwap=[204,304,"4xx","5xx"]`. `rizzy-streaming` re-implements `noSwap` but **ignores `hx-status`** | **All responses swap** except 204/304. Per-code behavior via `hx-status:NNN` | ❌ | P2 | **RIZZY-019** (+ RIZZY-003) | Pipeline: 422 validation, 500 error page | [What's New › Error responses swap](https://four.htmx.org/docs/whats-new-in-htmx-4/), [hx-status](https://four.htmx.org/reference/attributes/hx-status.md) |
| 14 | Response pipeline (JS) | `rizzy-streaming` returns `false` from `htmx_before_response` for **all** `text/html` and swaps itself | Supported takeover contract: set `ctx.extensionPromise` (as `hx-sse` and `hx-download` do). Otherwise core's `finally` runs immediately | ❌ | P1 | **RIZZY-003** | JS integration against real htmx 4.0.1 | 4.0.1 `#issueRequest` (`extensionPromise`), `ext/hx-sse.js` |
| 15 | Response text handling (JS) | `rizzy-nonce` `DOMParser` round-trip in `htmx_after_request` | htmx parses fragments via `<template>` and strips `<head>` with a regex | ❌ | P1 | **RIZZY-004** | jsdom + browser | 4.0.1 `#makeFragment` |
| 16 | Confirm (JS) | `rizzy-confirm` cancels the native confirm and waits for an `rz:confirm-dialog` handler | `htmx:confirm` detail `{ctx, issueRequest, dropRequest}`. Cancelling skips `window.confirm` | ❌ (no listener) | P1 | **RIZZY-005** | JS unit | [htmx:confirm](https://four.htmx.org/reference/events/htmx-confirm.md), 4.0.1 `#issueRequest` |
| 17 | Antiforgery (JS) | `htmx_config_request` adds the token header for non-GET/HEAD. Body fallback only for POST/PUT/PATCH | `hx-delete` no longer sends form data. `htmx:config:request` is the supported hook | ✅ | – | Also refresh the token on `HX-Request-Type: full` responses, not only boosted ones | Existing JS test | [What's New › hx-delete](https://four.htmx.org/docs/whats-new-in-htmx-4/), [htmx:config:request](https://four.htmx.org/reference/events/htmx-config-request.md) |
| 18 | Extension loading | 4 extensions auto-register on script load | No `hx-ext`. `htmx.config.extensions` allow-list blocks unlisted names | ⚠️ | P3 | Document that the allow-list must include `rizzy-*`. Make Rizzy extensions individually configurable | – | [htmx.config.extensions](https://four.htmx.org/reference/config/htmx-config-extensions.md) |
| 19 | Reliance on internals (JS) | `elt._htmx.boosted`, `_htmx[...]` state, `ctx.hx.*` | `_htmx` is not a public API. `ctx` is documented | ⚠️ | P2 | Use documented `ctx` fields and `HX-Boosted`/`HX-Request-Type` headers only | – | [Extension authoring skill](https://four.htmx.org/docs/) (htmx 4.0.1 `dist/skills/htmx-extension-authoring.md`) |
| 20 | Config model | `HtmxConfig` (C#) → meta JSON | Several keys removed or renamed. Default `prefix` is `data-hx-`. `inlineStyleNonce` removed | ⚠️ | P2 | **RIZZY-028** | Snapshot test of the meta JSON | [htmx.config](https://four.htmx.org/reference/config/htmx-config.md), [What's New › Config](https://four.htmx.org/docs/whats-new-in-htmx-4/) |
| 21 | Timeout | Config exposes `DefaultTimeout` | Default 60 s (was none) | ⚠️ | P3 | Document. Streaming responses currently escape the timeout only because core's `finally` clears it early (RIZZY-003). After the fix, `extensionPromise` users must call `clearTimeout(ctx.requestTimeout)` as `hx-sse` does | Long-stream test | [What's New › 60-second timeout](https://four.htmx.org/docs/whats-new-in-htmx-4/) |
| 22 | Attribute constants | `Constants.Attributes` includes `hx-ignore`, `hx-disable` (new meaning), `hx-action`, `hx-method`, `hx-config`, `hx-validate`, `hx-status:` | Matches 4.0.1 | ✅ | – | – | – | [What's New › Attributes](https://four.htmx.org/docs/whats-new-in-htmx-4/) |
| 23 | Event names (JS) | `htmx:after:swap`, `htmx:before:response`, `htmx:finally:request`, etc. | `htmx:phase:action` naming | ✅ | – | – | `validate-htmx4.js` already blocks legacy names | [What's New › Events](https://four.htmx.org/docs/whats-new-in-htmx-4/) |
| 24 | Pinned htmx version | Lockfile 4.0.0-beta4. CDN 4.0.0-beta2 in demo and docs, without SRI | 4.0.1 is the first stable patch | ⚠️ | P2 | Update pins, add SRI, make it a peer dependency (**RIZZY-024**) | CI against the pinned version | npm registry |
| 25 | SSE | SSE trigger APIs removed from C#. `HxSseConnect`/`HxSseClose` constants | SSE rewritten as the `hx-sse` extension | ✅ | – | Docs | – | [What's New › Core extensions](https://four.htmx.org/docs/whats-new-in-htmx-4/) |

## 3. Detailed findings

### RIZZY-003 — `rizzy-streaming` takes over all HTML responses and bypasses htmx 4 core

| | |
|---|---|
| Category / Severity | Reproducible incompatibility, architecture / **P1** |
| Subsystem | `packages/rizzy/src/js/rizzy-streaming.js` (bundled into `src/Rizzy/wwwroot/js/rizzy.js`) |
| Evidence | `htmx_before_response` returns `false` for every response with `Content-Type: text/html` (lines 37–66). In htmx 4.0.1 `#issueRequest`, a cancelled `htmx:before:response` returns early, and the `finally` block runs **immediately** because the extension never sets `ctx.extensionPromise`. That `finally` fires `HX-Trigger`, `htmx:finally:request`, hides indicators, re-enables `hx-disable` elements and releases the `hx-sync` queue. Rizzy then: fires `HX-Trigger` **again** and **before** the swap (`applySimpleResponseOverrides` → `fireSimpleTriggerHeader`, line 397); fires a second synthetic `htmx:finally:request` on no-swap (line 51); re-implements `noSwap` but skips `#handleStatusCodes`, so **`hx-status:*` attributes never apply**; never fires `htmx:after:request` or `htmx:response:error`; handles `HX-Location` as a bare GET with no `push`/`select`/`swap` options (line 436); and processes the removed `HX-Trigger-After-*` headers (line 444). The "prefix" path (lines 202–212) swaps text **without** the nonce sanitizer (see RIZZY-027). |
| Impact | In every Rizzy app, the headline htmx 4 features (`hx-status`, error events, the trigger timing contract, request queueing, indicator lifetime) silently misbehave. This affects all responses, not just streaming ones. |
| Recommendation | (1) Only take over when streaming is actually present: a server header such as `Rizzy-Streaming: 1`, emitted when Blazor streaming is active, or detection of `<blazor-ssr` in the first chunk with fallback to core. (2) When taking over, use the supported contract: set `ctx.extensionPromise`, call `clearTimeout(ctx.requestTimeout)`, and **don't** re-fire headers that core fires in `finally`. (3) Delegate swapping to `htmx.swap(ctx)` with core's status handling, or call into `hx-status` semantics. (4) Delete the `HX-Trigger-After-*` handling. (5) Add JS integration tests that load the real htmx 4.0.1. |
| Status / Confidence | Reproducible incompatibility (static, against 4.0.1 source) / High |

### RIZZY-004 — `rizzy-nonce` sanitizer corrupts responses

| | |
|---|---|
| Category / Severity | Reproducible incompatibility, correctness / **P1** |
| Subsystem | `packages/rizzy/src/js/rizzy-nonce.js` (`processUnsafeHtml`, lines 25–45). Used by `htmx_after_request` and by `rizzy-streaming` |
| Evidence (jsdom harness) | `<tr id="r1"><td>Cell</td></tr>` becomes `<html><head></head><body>Cell</body></html>`, so **table rows and cells are destroyed** (the HTML parser drops them outside a table). `<div>x</div><style>…</style><link rel=stylesheet>` returns only `<div>x</div>`. **Every un-nonced `<style>`, `<link>` and `<script>` is removed, even when no CSP nonce is configured**: `getDocumentNonce()` returns `""`, and `null !== ""` is true. A leading `<script nonce=…>` is hoisted into `<head>`. htmx 4.0.1 `#makeFragment` then deletes `<head>…</head>` with a regex, so **nonced scripts at the start of a fragment never run**. Every response is re-serialized as a full document. |
| Impact | Common htmx patterns break in every Rizzy app: table row swaps (edit-in-place, infinite scroll in tables, delete-in-place), inline styles in partials, and scripts. The extension is always on. |
| Recommendation | Stop round-tripping through `DOMParser`. Prefer htmx 4's native `inlineScriptNonce` together with the official `hx-csp` extension. If filtering is still needed, parse via `<template>` (as htmx does), filter only `script` elements, only when a CSP nonce is configured, and serialize the template content, not `documentElement`. Add regression fixtures for `<tr>`, `<td>`, `<option>`, `<hx-partial>`, and leading scripts. |
| Status / Confidence | Confirmed (runtime, jsdom) plus static analysis of the htmx 4.0.1 parser / High |

### RIZZY-005 — `rizzy-confirm` blocks `hx-confirm` without a dialog listener

| | |
|---|---|
| Category / Severity | Correctness / **P1** |
| Subsystem | `packages/rizzy/src/js/rizzy-confirmation.js` |
| Evidence | `htmx_confirm` sets `detail.cancelled = true` and returns `false` (line 120). htmx 4.0.1 then skips `window.confirm`. The request only proceeds when something handles the `rz:confirm-dialog` window event and calls `confirm()`/`deny()`/`dismiss()` (line 63). Rizzy ships no such handler. |
| Impact | In any Rizzy app that hasn't written a custom dialog listener, **every `hx-confirm` element silently does nothing**: no dialog, no request. |
| Recommendation | If there are no listeners, fall back to `window.confirm`. For example, check `detail.defaultPrevented` on a cancellable `rz:confirm-dialog`, or make the custom dialog opt-in via config (`htmx.config.rizzy.confirm = "custom"`). Add a test. |
| Status / Confidence | Confirmed (static, deterministic code path against 4.0.1) / High |

### RIZZY-006 — Layout stripping breaks htmx 4 history restore and full-page requests

| | |
|---|---|
| Category / Severity | Reproducible incompatibility / **P1** |
| Subsystem | `src/Rizzy/Components/Layout/HtmxLayout.cs` (line 18), `HtmxApp<T>`, `MinimalLayout` |
| Evidence | `HtmxLayout` renders `MinimalLayout` (`<html><body>`) or `EmptyLayout` instead of the real layout whenever `HX-Request` is present. htmx 4.0.1 `#restoreHistory` issues `htmx.ajax('GET', path, { target: historyElt, swap: 'outerSync', select: '[hx-history-elt]' if present, headers: { 'HX-History-Restore-Request': 'true' } })`, and `HX-Request-Type` is `full` because the target is body or a select is used. This happens on **every** back/forward navigation, because htmx 4 has no localStorage cache. With no `hx-history-elt`, the body is replaced with layout-less content, so the sidebar and navigation disappear. With `hx-history-elt`, the selector finds nothing because the layout containing it was stripped. Boosted links or forms without an explicit `hx-target` (default target `body`) hit the same problem. |
| Impact | Browser back/forward is broken in typical Rizzy apps on htmx 4, including the demo. In htmx 2 this was limited to cache misses. |
| Recommendation | Use the fragment layout only when `HX-Request-Type == partial` and **not** a history restore. Otherwise render the full layout (consider `Vary: HX-Request-Type`). Document `hx-history-elt` usage, and the `HtmxConfig.History = HtmxHistoryMode.Reload` fallback. Add pipeline tests covering all four request kinds. |
| Status / Confidence | Reproducible incompatibility (static, 4.0.1 source) / High. Browser confirmation pending |

### RIZZY-007 — Trigger builder emits htmx 2 syntax

| | |
|---|---|
| Category / Severity | Reproducible incompatibility / **P1** |
| Subsystem | `src/Rizzy.Htmx/HtmxTriggerSpecification.cs` (lines 166–170, 199–238), `TriggerModifierBuilder.Queue`, `TriggerQueueOption` |
| Evidence (harness output) | `From("closest form")` produces `keyup from:closest form`. htmx 4 HCON reads `from:closest`, then `form` as a separate token, so it **must** be `from:'closest form'` (this is the exact example in the htmx 4 upgrade notes). `From(".a .b")` produces `from:{.a .b}` and `Target(".a, .b")` produces `target:{.a, .b}`. Braces were never htmx syntax, and htmx 4 requires quotes. `Queue(all)` produces `click queue:all`, a modifier that htmx 4 removed and silently ignores. |
| Impact | Triggers built with `From`/`Target` selectors that contain spaces or extended keywords listen on the wrong element or not at all. Queue settings silently do nothing. |
| Recommendation | Quote selectors containing whitespace or commas with `'…'`, escaping embedded quotes. Remove `Queue` (mark `[Obsolete]` with a pointer to `hx-sync`) or translate it into an `HxSync` helper. Add `Prevent/Stop/Halt/Capture/Passive/RootMargin` and `FromOutside/FromSelf`. Fix RIZZY-011 and RIZZY-012 at the same time. |
| Status / Confidence | Confirmed (runtime output) against official docs / High |

### RIZZY-013 — `SwapStyleBuilder` modifier drift

| | |
|---|---|
| Category / Severity | Reproducible incompatibility / **P2** |
| Subsystem | `src/Rizzy.Htmx/SwapStyleBuilder.cs` (line 356), `HtmxSwapSpecification` |
| Evidence | `ScrollFocus()` produces `focus-scroll:true`. htmx 4.0.1 parses swap modifiers with HCON and reads `swapSpec.focusScroll` (source line 1480), so `focus-scroll` is **ignored**. `ScrollTop("closest .container")` produces `scrollTarget:closest .container`, unquoted and therefore misparsed. `HtmxSwapSpecification.Strip`/`Target` exist but are never emitted. There is no `swapEmpty` or `outerSync`. Time formatting is culture-sensitive (RIZZY-012). |
| Recommendation | Emit `focusScroll:`. Quote selectors. Add `Strip()`, `Target()`, `SwapEmpty()`, and `SwapStyle.outerSync`. Snapshot-test all outputs. |
| Status / Confidence | Confirmed (runtime output plus 4.0.1 source) / High |

### RIZZY-014 — `AjaxContext` / `LocationTarget` don't match htmx 4 ajax options

| | |
|---|---|
| Category / Severity | Reproducible incompatibility / **P2** |
| Subsystem | `src/Rizzy.Htmx/AjaxContext.cs` (lines 26, 44, 50), `LocationTarget.cs` |
| Evidence | `Values` and `Headers` are `string?`, so the serialized `HX-Location` is `{"path":"/x","values":"{"a":1}","headers":"…"}` (harness), with JSON **strings** where htmx 4 expects objects. `Handler` is not an htmx 4 option. `push`, `replace`, `selectOOB` and `transition` are missing, so `HX-Location` always pushes history (the htmx 4 default) and that can't be turned off. |
| Recommendation | Make `Values`/`Headers` dictionaries. Add the missing options and remove `Handler`, which is a breaking change for 6.0. |
| Status / Confidence | Confirmed (runtime serialization) / High |

### RIZZY-019 — No strategy for htmx 4 swapping 4xx/5xx responses

| | |
|---|---|
| Category / Severity | Compatibility, developer experience / **P2** |
| Subsystem | Error handling, validation, auth (cross-cutting) |
| Evidence | htmx 4 swaps every response except 204/304. Rizzy has no `hx-status` helpers, no guidance for rendering validation errors with 422, and no partial-aware error pages. `UseExceptionHandler` and login redirects render full pages, which then get swapped into fragment targets. The demo sidesteps this by reverting to htmx 2 behavior (`NoSwap = [204, 304, "4xx", "5xx"]`, `src/RizzyDemo/Program.cs`). `rizzy-streaming` ignores `hx-status` (RIZZY-003). |
| Recommendation | Pick a documented default: either keep htmx 4 semantics and provide `HtmxStatus(...)` attribute helpers plus `HtmxResponse.ValidationFailed()` (422 + retarget), or set `NoSwap` 4xx/5xx by default in `AddHtmx` with a clear opt-out. Provide an htmx-aware error-page pattern (`HX-Retarget`/`HX-Reswap` to an error region, or `HX-Refresh` for 5xx) and an `HX-Redirect`-on-401 option. |
| Status / Confidence | Architectural concern / High |

### RIZZY-028 — `HtmxConfig` and API drift from htmx 4

| | |
|---|---|
| Category / Severity | Compatibility, maintainability / **P2** |
| Subsystem | `src/Rizzy.Htmx/HtmxConfig.cs`, `Constants.cs`, `TriggerQueueOption`, `ScrollBehavior` |
| Evidence | `InlineStyleNonce` (line 74) and `GenerateStyleNonce` (line 163) map to `inlineStyleNonce`, which htmx 4 removed (indicator CSS uses constructable stylesheets). The `Prefix` doc comment (line 177) says htmx 4 "no longer supports `data-hx-`", but the 4.0.1 default is `prefix: "data-hx-"`. `metaCharacter` and `allowEmptySwapAfterOOB` are missing. `TriggerQueueOption` is obsolete. `ScrollBehavior` is unused (it served the removed `scrollBehavior` config). The `HtmxConfigHeadOutletTest` still sets `InlineStyleNonce`. |
| Recommendation | Remove or obsolete the dead keys in 6.0. Correct the docs. Add the missing keys. Snapshot the meta JSON in tests. |
| Status / Confidence | Confirmed (static, against 4.0.1 config defaults) / High |

## 4. Required migration changes (in Rizzy)

1. Rework `rizzy-streaming` (opt-in, `extensionPromise`, no duplicate events, respect `hx-status`). RIZZY-003
2. Replace `rizzy-nonce` with htmx-native nonce handling and template-based filtering. RIZZY-004, RIZZY-027
3. Make `rizzy-confirm` fall back to native confirm. RIZZY-005
4. Choose layouts from `HX-Request-Type` and history-restore. RIZZY-006
5. Fix trigger and swap builder syntax, culture and caching. RIZZY-007, -011, -012, -013
6. Align `HX-Location`/ajax options and `HX-Trigger` serialization. RIZZY-014, RIZZY-015
7. Clean up config drift and update htmx pins (4.0.1, SRI, peer dependency). RIZZY-028, RIZZY-024
8. Ship an error/validation response strategy. RIZZY-019

## 5. Optional modernization opportunities

| Opportunity | Benefit | Recommend? |
|---|---|---|
| Use `HX-Request-Type` for automatic full/partial rendering in `RizzyService.View` | Removes a whole class of layout bugs, and controllers no longer choose between `View`/`PartialView` for boosted navigation | **Yes** (part of RIZZY-006) |
| `hx-status` helpers and 422 validation pattern | Native validation UX without client JS. Lets the demo drop its `NoSwap` revert | **Yes** (RIZZY-019) |
| Replace custom nonce rewriting with `inlineScriptNonce` plus the official `hx-csp` extension | Less custom security code to maintain | **Yes** (needs a security review) |
| Use `ctx.extensionPromise` for streaming | Supported contract and correct indicator and queue lifetime | **Yes** |
| `<hx-partial>`-based OOB with explicit swap per partial | Already used | Keep |
| `HX-Location` HCON format | Shorter headers | No. JSON is clearer and fine |
| Typed `hx-config` builder | Per-element request config | Optional, later |
| `htmx-2-compat` extension for migrating users | Eases app migration | Document it as an app-level option. Don't depend on it in Rizzy |
| Morph swaps (`innerMorph`/`outerMorph`) in `HtmxSwappable` | Preserves state on OOB updates | Optional, already representable via `SwapStyle` |

## 6. Compatibility strategy comparison

| Criterion | 1. htmx 2 primary | 2. Dual htmx 2 + 4 | 3. htmx 4 primary |
|---|---|---|---|
| Implementation changes | Revert the June migration on the main line, re-publish 5.3.x fixes | Two JS bundles (the extension APIs are incompatible: `defineExtension` vs `registerExtension`, different events and `ctx`). Version-switched config serializer, header names and trigger/swap syntax. A config flag for request-header parsing | Finish the migration (Section 4) |
| Public API consequences | Removes the htmx 4 config members just shipped in 5.4.0 | Doubles builder and config surfaces, or adds version parameters everywhere | Breaking cleanup in 6.0 (removed `Queue`, `Handler`, `InlineStyleNonce`, string `Values`) |
| Compatibility risk | Low short-term. Grows as htmx 2 enters maintenance | High: two code paths in every subsystem | Medium. Users must migrate apps (explicit inheritance, error swaps, history) |
| Testing | Existing | Full matrix × 2, including browser tests for both runtimes | One matrix against htmx 4.x |
| Maintenance | Medium now, rising | **High** for a small project | Low to medium |
| Developer experience | Familiar, but no htmx 4 features | Confusing (which builder emits what?) | Modern, aligned with upstream docs |
| Extensibility | Poor (no `hx-status`, `HX-Request-Type`, `<hx-partial>`) | Constrained by the lowest common denominator | Best |
| Sustainability | Declining (htmx 4 is the future, even though npm `latest` is still 2.x today) | Poor | **Best** |

### Recommendation: Strategy 3, htmx 4 as the sole supported version for Rizzy 6.0

- The repository and the published Rizzy 5.4.0 are already htmx 4 oriented. Reverting would throw away correct work (header names, config keys, `<hx-partial>`).
- Rizzy's value is in **opinionated client extensions**, which are tied to one htmx extension API. Supporting two would mean maintaining two client libraries.
- htmx 2 users have a clean path: **Rizzy 5.3.x (and Rizzy.Htmx 5.3.0) remain the htmx 2 line.** Document this in a compatibility table (Rizzy 5.3.x ↔ htmx 2.x; Rizzy 6.x ↔ htmx 4.x). Mark 5.4.0 deprecated, since it is broken (RIZZY-001).

## 7. Expected impact on consumers

**Changes Rizzy makes (library):** everything in Section 4. Consumers get these by upgrading to 6.0.

**Changes application developers must make when moving to Rizzy 6 / htmx 4:**

| Change | Why |
|---|---|
| Load htmx **4.0.1+** (or the pinned version) before `_content/Rizzy/js/rizzy.js` | Runtime |
| Add `:inherited` to attributes meant to cascade (`hx-boost`, `hx-target`, `hx-confirm`, `hx-headers`, …), or set `HtmxConfig.ImplicitInheritance = true` | Explicit inheritance |
| Rename `hx-disable` → `hx-ignore`, then `hx-disabled-elt` → `hx-disable`. Replace `hx-vars`/`hx-params`/`hx-ext`/`hx-request` | Attribute renames |
| Replace `queue:` in hand-written `hx-trigger` with `hx-sync`. Quote `from:`/`target:` selectors | Trigger syntax |
| Design 4xx/5xx responses as swappable content, use `hx-status`, or opt into `NoSwap` | Error responses swap |
| Add `hx-include="closest form"` to `hx-delete` elements that relied on form data | `hx-delete` change |
| Add `hx-history-elt` (or `History = Reload`) where full-page restore is undesirable | No history cache |
| Update JS listeners to the new event names (`htmx:after:swap`, `htmx:response:error`, …) | Event renames |
| If using `htmx.config.extensions`, include `rizzy-nonce, rizzy-streaming, rizzy-antiforgery, rizzy-confirm` | Extension allow-list |
| Run `npx -p htmx.org@4 upgrade-check` on Razor/CSHTML templates (`--ext .razor --ext .cshtml`) | Automated assistance |
