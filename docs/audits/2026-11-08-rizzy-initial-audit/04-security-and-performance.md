# 04 — Security and Performance

| Field | Value |
|---|---|
| Audit | Rizzy initial audit, 2026-11-08 |
| Repository / commit | https://github.com/JalexSocial/Rizzy @ `7e3f0f1126ee412d9db4f37d13d0370159edac15` |
| Scope | Security impact of Rizzy on consuming apps (XSS, CSP, antiforgery, headers, redirects, disclosure, dependencies) and performance characteristics |
| Methodology / limitations | See [00](00-executive-summary.md#methodology). There was no intrusive or external testing. Exploitability was shown with local harnesses only. Benchmarks are indicative Stopwatch runs (.NET SDK 10.0.403, Release, Windows 11), not BenchmarkDotNet |

## Part A — Security

### Summary

| Class | Result |
|---|---|
| Confirmed exploitable vulnerability | **RIZZY-002** (XSS in `SerializeAsAlpineData`) |
| Security design concerns | RIZZY-027 (CSP nonce design), RIZZY-029 (raw-markup swap APIs), RIZZY-031 (no redirect validation), RIZZY-018 (`Vary`) |
| Healthy areas | Meta config escaping, nonce generation, antiforgery header injection, same-origin fetch default, no sensitive data logged (the library does no logging at all) |

### RIZZY-002 — XSS in `ObjectExtensions.SerializeAsAlpineData`

| | |
|---|---|
| Category / Severity | Security (XSS) / **P0** |
| Subsystem | `src/Rizzy/Extensions/ObjectExtensions.cs` (line 41) → `src/Rizzy/Serialization/SingleQuoteStringConverter.cs` (line 17) |
| Evidence | Each string value is written with `writer.WriteRawValue($"'{value.Replace("'", "\\'")}'", skipInputValidation: true)`. Backslashes, newlines and other control characters are not escaped, and the default encoder is bypassed. **Reproduction:** the value `\', x: alert(1)}//` serializes to `{"name":'\\', x: alert(1)}//'}` (harness, byte-verified). In JavaScript, `'\\'` is a complete string, so `x: alert(1)` becomes an evaluated property and `//` comments out the rest. The method's documented use is `<div x-data="@obj.SerializeAsAlpineData()">`. Razor's attribute encoding is undone by the HTML parser before Alpine evaluates the expression, so the payload runs. A raw newline in a value also produces invalid JS (`'line1⏎line2'`). |
| Exploitability | Exploitable whenever **any string property of the serialized object can be influenced by a user** (names, comments, titles), which is the helper's intended purpose, and the page uses Alpine.js. |
| Impact | Stored or reflected XSS in consuming apps. Nonce-based CSP does not help, because Alpine evaluates attribute expressions. Apps using Alpine typically need `unsafe-eval` or the CSP build. |
| Remediation | Emit standard JSON with the default (HTML-safe) encoder: `JsonSerializer.Serialize(value, options)` with no custom string converter. Double-quoted JSON is valid JS, and Razor attribute-encodes the quotes correctly. If single quotes must be kept, escape `\` first, then `'`, plus `\n`, `\r`, ` `, ` ` and `<`. Add unit tests with hostile inputs. Publish a security advisory once fixed. |
| Status / Confidence | Confirmed (runtime) / High |

### RIZZY-027 — CSP nonce design concerns

| | |
|---|---|
| Category / Severity | Security design / **P2** |
| Subsystem | `HtmxConfigHeadOutlet` (line 56), `RizzyMiddleware`, `rizzy-nonce.js`, `rizzy-streaming.js` |
| Evidence | (1) **The nonce is exposed in a DOM attribute.** The page nonce is written into `<meta name="htmx-config" content='{"documentNonce":"…"}'>`. Browsers deliberately hide `nonce` attribute values from CSS selectors and the DOM to stop exfiltration through CSS or markup injection. Copying the nonce into an ordinary attribute defeats that hiding. htmx's own `inlineScriptNonce` meta config has the same property. (2) **Blanket nonce re-application.** When `GenerateScriptNonce` is true, `InlineScriptNonce = DocumentNonce`, and htmx 4 then stamps that nonce on **every** script in swapped content (`#processScripts`). The only remaining barrier against injected scripts is the `rizzy-nonce` filter, which is itself defective (RIZZY-004). (3) **Sanitizer bypass.** In `rizzy-streaming.js` (lines 202–212), non-SSR "prefix" text is swapped via `htmx.swap` without `sanitizeHtmlOrFailClosed`, unlike the primary and tail paths. (4) `replaceAll(responseNonce, documentNonce)` rewrites the response nonce anywhere in the text, not only in `nonce` attributes. The value is random, so this is low risk. |
| Impact | Defense in depth is weaker than the "secure nonce handling" the docs claim. Exploitation still requires an HTML injection bug in the app, so this is not a vulnerability on its own. |
| Remediation | Redesign around the official `hx-csp` extension and htmx's `inlineScriptNonce`. Don't publish the nonce in a meta attribute if it can be avoided (read it from an existing `script[nonce]` element via the `.nonce` property instead). Route every swap path through one sanitizer, or none. Document a recommended CSP for Rizzy apps. Follow up with a dedicated security review after RM-B2. |
| Status / Confidence | Architectural concern (static) / Medium–High |

### RIZZY-018 — `Vary: HX-Request` applied inconsistently

| | |
|---|---|
| Category / Severity | Correctness, cache safety / **P2** |
| Subsystem | `src/Rizzy/Components/Layout/HtmxLayout.cs` (line 20) |
| Evidence | `Vary: HX-Request` is added only **on htmx responses** and only via `TryAdd`, so it isn't appended if another component already set `Vary` (e.g. `Accept-Encoding`). The non-htmx (full page) variant of the same URL carries no `Vary`. |
| Impact | Browser or shared caches can serve a fragment for a full navigation, or the reverse, when caching is enabled (output caching, CDN, `Cache-Control: public`). This is a classic htmx caching bug. Antiforgery responses are no-cache by default, which limits exposure. |
| Remediation | Always append `Vary: HX-Request` (and `HX-Request-Type` once RIZZY-006 is fixed) to responses whose shape depends on these headers, merging with existing values. |
| Status / Confidence | Confirmed (static) / High |

### RIZZY-031 — No URL validation for redirect helpers

| | |
|---|---|
| Category / Severity | Security (application-dependent) / **P3** |
| Evidence | `HtmxResponse.Redirect`, `Location`, `PushUrl` and `ReplaceUrl` accept any string. There is no `LocalRedirect`-style helper. `rizzy-streaming` follows Blazor `redirection` templates with `location.replace` (server-controlled). |
| Impact | Open redirect only if an app passes user input, such as a `returnUrl`, straight through. This is the same as `Redirect()` in MVC. |
| Remediation | Add `LocalRedirect(string)` / `RedirectToLocal` helpers that validate with `IUrlHelper.IsLocalUrl` semantics, and document them. |
| Status / Confidence | Optional improvement / High |

### Security checklist

| Topic | Result |
|---|---|
| HTML escaping of generated markup | Razor components handle encoding. **Exceptions:** `AddSwappableContent`/`AddRawContent`/`AddChildContent(string markup)` insert raw markup by design (RIZZY-029). `SerializeAsAlpineData` (RIZZY-002). |
| Attribute injection | `HtmxConfigHeadOutlet` places JSON in a single-quoted attribute. The source-generated serializer's default encoder escapes `'`, `<`, `>`, `&` (verified: `"a'b<c>"`). **Safe.** `HtmxSwappable` attributes are emitted by the renderer, so they're encoded. |
| Header injection | Header values come from developer code. Kestrel rejects CR/LF. `HX-Trigger` JSON can be malformed (RIZZY-015) but cannot inject headers. Non-ASCII values throw (RIZZY-016), which is a reliability issue rather than a security one. |
| CSRF / antiforgery | The token is sent as a header for every non-GET/HEAD htmx request, including `DELETE`. With the default `GenerateTokensPerPage` it is refreshed on boosted navigation. Server validation needs `[ValidateAntiForgeryToken]`/`[AutoValidateAntiforgeryToken]` on MVC actions. **Rizzy adds no global filter**, so the developer must opt in, as usual in MVC. Document it. Setup pitfall: without `AddHtmx`, the token never gets injected (RIZZY-026). |
| Token leakage | `fetch` mode defaults to `same-origin`, so the token header is not sent cross-origin unless the app sets `Mode = "cors"`. If it does, the token goes to any cross-origin htmx URL. Recommend host-checking in `rizzy-antiforgery`. |
| Authentication / authorization | No Rizzy-specific boundaries. `HtmxRequestAttribute` is a routing constraint, **not** a security control, because the `HX-Request` header is client-controlled. Document this. |
| Sensitive information disclosure | No logging and no exception-detail rendering in the library. `handleControlTemplate("error")` writes Blazor's error text into `document.body` (server-controlled). |
| Unsafe serialization | Only RIZZY-002. Everything else uses System.Text.Json with default encoders. |
| Model binding | `InitializeBlazorFormData` feeds the whole form into Blazor's form-data provider, scoped by `_handler`. That is equivalent to the framework's own behavior. No over-posting beyond normal Blazor `[SupplyParameterFromForm]` semantics. |
| Nonce generation | 32 random bytes from `RandomNumberGenerator`, Base64Url, cached per request. **Good.** |
| Third-party scripts | The demo and READMEs load htmx and simple-notify from jsDelivr **without SRI** and pinned to a beta (RIZZY-024). |
| Dependency vulnerabilities | Runtime: none known (`Sqids` only). Test-only: `AngleSharp` 1.4.0 (via bUnit), moderate (GHSA-pgww-w46g-26qg). JS dev-only: `vite` ≤6.4.3, `postcss`, `nanoid`, `source-map-js` (high, build-time only, not shipped). `npm audit --omit=dev` reports 0. See RIZZY-024. |

**Do the defaults encourage secure development?** Partly. Antiforgery-by-header and the nonce provider are good defaults. However, the raw-markup APIs are named neutrally, `SerializeAsAlpineData` is unsafe, the CSP design has gaps, and none of it is documented. Hence the P0 and P2 findings above.

## Part B — Performance

### Measured (indicative)

| Operation | Time | Allocation | Frequency | Assessment |
|---|---|---|---|---|
| Reflection lookup in `InitializeBlazorFormData` (`Assembly.Load(...).GetTypes()` + 2× `First`) | **23.9 µs/op** | 2.5 KB/op | Every form POST with `_handler` through `RzController` | Small compared with rendering. Cache it anyway when fixing RIZZY-020 |
| `new HtmxRequest(ctx)` (5 headers incl. `Uri.TryCreate`) | 1.5 µs | 128 B | Several times per request (constraint, middleware, swap service, `HtmxContext`) | Negligible. Use the cached `Request.Htmx()` consistently |
| `ObjectExtensions.ToDictionary(anon)` (`TypeDescriptor`) | 2.4 µs | 408 B | Each `View<T>(object)` call | Negligible. The typed builder avoids it |
| `Trigger.OnEvent(...).Changed().Delay(...).ToString()` | 0.9 µs | 832 B | Per render where used | Negligible. The `Lazy` cache (RIZZY-011) is unnecessary |

Harness: `scratchpad/verify/bench` (outside the repo). Warm-up 200 iterations, 2k–200k measured iterations. Treat as order-of-magnitude only.

### Analysis (not measured)

| Area | Observation | Priority |
|---|---|---|
| Rendering path | `RazorComponentResult` + `DynamicComponent` + `LayoutView` + `CascadingValue`: standard Blazor SSR cost. `RzView` caches `[Layout]` lookups in a static `ConcurrentDictionary` | Fine |
| Streaming | `PreventStreamingRendering = false` for both pages **and partials**. Combined with RIZZY-003, the client always buffers the full response for non-streaming partials before swapping (no extra latency, but extra JS work) | Revisit with RIZZY-003 |
| Client nonce filter | `DOMParser` parse + full re-serialization of every response (RIZZY-004): O(n) extra work and memory per swap, significant for large tables. That's on top of the correctness problems | Fixed by RM-B2 |
| `DataAnnotationsProcessor` | Per field per render: a new `DefaultHttpContext` + `ActionContext` + `DefaultModelValidationContext`, a localizer and adapter lookup. Moderate allocations on large forms. The cache bug (RIZZY-008) is the bigger issue | P3, after the fix |
| `HtmxSwapService` | `RenderToString()` creates a new `HtmlRenderer` per call (`await using`). Only used explicitly | Fine |
| Source generator | Combines with `CompilationProvider` and collects all class declarations, so it re-runs on every edit, which affects IDE responsiveness on large solutions (RIZZY-022) | P3 |
| Thread-pool / async | `InitializeBlazorFormData` reads `Request.Form` synchronously (sync-over-async) when the form isn't already buffered. Under load with large multipart bodies this blocks thread-pool threads (RIZZY-020) | P2, with RIZZY-020 |
| Memory retention | `HtmxSwapContent` subscribes without unsubscribing. Bounded by the request scope. Static caches are keyed by `Type` (bounded) | Fine |
| Concurrency | `DataAnnotationsProcessor.AddAttributeHandler` mutates a non-concurrent `Dictionary` on a singleton. Safe only if called during startup. Document or lock | P3 |
| `IdGenerator` | `Interlocked` counter + Sqids. Thread-safe. The alphabet is shuffled once per process, so IDs differ across instances, which is fine | Fine |

### RIZZY-032 — Minor measured per-request overheads

| | |
|---|---|
| Category / Severity | Performance / **P3** |
| Evidence | The table above. |
| Recommendation | Fold the reflection caching into the RIZZY-020 work, use the cached `HtmxRequest`, and drop the trigger `Lazy`. No dedicated optimization effort is justified. If performance becomes a goal, add a BenchmarkDotNet project for the render path (`View<T>` end to end) rather than micro-paths. |
| Status / Confidence | Confirmed (measured, indicative) / Medium |

### Practical mitigation summary

| Priority | Action | Findings |
|---|---|---|
| Immediate | Fix the Alpine serializer, release, publish an advisory | RIZZY-002 |
| Phase B | Redesign nonce handling around `hx-csp`/`inlineScriptNonce`, unify sanitizer paths | RIZZY-004, RIZZY-027 |
| Phase B | Always-correct `Vary` | RIZZY-018 |
| Phase C | Safe-by-name markup APIs and `LocalRedirect` helpers | RIZZY-029, RIZZY-031 |
| Phase D | Cache the reflection and make the form read async | RIZZY-020, RIZZY-032 |
