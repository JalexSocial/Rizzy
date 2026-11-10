# Errata and verification updates

Additions made after the audit was written, during roadmap Phase A (2026-11-08, branch `phase-a-stabilization`). The original reports are left unchanged, as required by [the audit conventions](../README.md#preserving-historical-reports).

## Verification status upgrades

| Finding | Original status | New status | Evidence |
|---|---|---|---|
| RIZZY-001 | Confirmed (static) | **Confirmed (runtime)** | `eng/consumer-smoke-test.sh`, run against the packages published on nuget.org (Rizzy 5.4.0, which resolves Rizzy.Htmx 5.3.0). The first `HtmxSwappable` render throws `System.MissingMethodException: Method not found: 'System.String Rizzy.Htmx.HtmxConfig.get_Prefix()'`. A consumer using the htmx 4 config API (`HtmxConfig.History`) cannot even compile: `CS0117`. |
| RIZZY-003 | Reproducible incompatibility (static) | **Confirmed (runtime)** | `packages/rizzy/tests/htmx4-integration.test.mjs` runs the built Rizzy bundle against the real htmx 4.0.1 in jsdom. `HX-Trigger` fired **twice, both before the swap** (`[false, false]`). `hx-status:422` was ignored (content landed in the original target). `htmx:response:error` never fired. |
| RIZZY-004 | Confirmed (runtime, isolated sanitizer) | **Confirmed (runtime, real htmx 4.0.1)** | Same suite. `<tr id="row1"><td>Cell</td></tr>` swapped as the bare text `Cell`. Un-nonced `<style>` was removed when no CSP was configured. The identical swap without the Rizzy bundle keeps the row. |
| RIZZY-005 | Confirmed (static) | **Confirmed (runtime)** | Same suite. With no `rz:confirm-dialog` listener, `window.confirm` is never called and the request is never issued. |

These tests are marked `todo` (they run but don't fail the build) until roadmap Phase B fixes the defects.

## New finding

### RIZZY-034 — npm `exports`/`module` point to a file the build never produces

| | |
|---|---|
| Category / Severity | Packaging / **P2** |
| Evidence | `packages/rizzy/package.json` declared `"module": "./dist/js/rizzy.es.mjs"` and `exports["."].import: "./dist/js/rizzy.es.mjs"`. The Vite build produces only `rizzy.js` and `rizzy.min.js`, and `files: ["dist/"]` ships no `rizzy.es.mjs`. ESM consumers and bundlers (`import "@jalexsocial/rizzy"`) would hit a module-not-found error. |
| Resolution | Fixed in Phase A: `exports["."]` now points at the UMD bundle (`./dist/js/rizzy.js`), which works as a side-effect import, and `module` was removed. |
| Status / Confidence | Confirmed (static) / High |

## Clarification

- **RIZZY-026**: `AddRizzy()` also requires `AddRazorComponents()`, because `AddSupplyValueFromFormProvider` needs `IFormValueMapper`. Without it, the first render throws `InvalidOperationException: No service for type 'IFormValueMapper'`. The consumer smoke test surfaced this. Add it to the setup documentation (RM-E1) and consider registering it from `AddRizzy` (RM-C3).

## Corrections found during roadmap Phase B

Verified against the htmx 4.0.1 source (`node_modules/htmx.org/dist/htmx.js`) while implementing Phase B.

- **History restore requests (RIZZY-006):** htmx 4.0.1 restores history with `htmx.ajax('GET', path, { request: { headers: { 'HX-History-Restore-Request': 'true' } } })`. `htmx.ajax` copies its options onto the request context with `Object.assign`, which replaces the whole request object. The restore request therefore carries `HX-History-Restore-Request: true` and `HX-Request-Type: full` but **not** `HX-Request`. Layout selection must not depend on `HX-Request` alone. Rizzy 6 uses `HX-Request-Type` and the restore header.
- **`HX-Request-Type: full`** is sent when the target is `body` **or** the request uses `hx-select`, not only for boosted navigation.
- **Config keys (RIZZY-028):** `inlineScriptNonce` and `metaCharacter` are absent from the 4.0.1 default config object, but both are still honored (`#processScripts`, `#maybeAdjustMetaCharacter`). The roadmap's B10 item to add `MetaCharacter` stands. `inlineStyleNonce` is gone.
- **`inlineScriptNonce` (RIZZY-004, RM-B2):** core stamps this nonce on *every* script it recreates during a swap, including scripts injected into a response. The audit's suggested "rely on `htmx.config.inlineScriptNonce`" would trust injected scripts, so Rizzy 6 does not set it by default. It rewrites the response nonce to the page nonce in the raw text instead, and lets the browser's CSP block un-nonced scripts.
- **Deprecation policy (RM-B5, RM-B10):** the roadmap suggested obsoleting htmx 2 APIs. The repository's `PublicApiTests` (added with the htmx 4 migration) forbids `[Obsolete]` members in Rizzy.Htmx, so Phase B removes them outright. Every removal is listed in [docs/migration/htmx2-to-htmx4.md](../../migration/htmx2-to-htmx4.md).

## Corrections and findings from roadmap Phase C

- **Correction to the Phase B notes:** `src/RizzyDemo` still exists. It is part of `src/Rizzy.sln`, and CI builds it. The Phase B summary and PR #27 wrongly said it had been removed. Roadmap items that target the demo (RM-E3, the Playwright scenarios in RM-A7) still apply.
- **RIZZY-020 clarification:** with `AddControllers()`, the endpoint-level `[RequireAntiforgeryToken]` plus `UseAntiforgery()` does **not** reject a forged POST to an MVC action. The middleware only records the result, and MVC does not act on it. `[ValidateAntiForgeryToken]` (which needs `AddControllersWithViews()`) remains the mechanism for MVC actions.
- **RIZZY-020, no supported API:** in ASP.NET Core 10, `HttpContextFormDataProvider`, `EndpointHtmlRenderer+FormCollectionReadOnlyDictionary` and `RazorComponentResultExecutor` are all internal, and `RazorComponentResult` never passes form data. A reflective hook is still required. Phase C confines it to one cached, guarded lookup (`BlazorFormData`) with an upgrade canary test.

### RIZZY-035 — `[SupplyParameterFromForm]` was never populated for minimal-API Rizzy views

| | |
|---|---|
| Category / Severity | Functional defect / **P2** |
| Evidence | Form data reached Blazor form mapping only through `RzController.OnActionExecuting`. `app.MapPost("/x", (IRizzyService r) => r.View<T>())` rendered `[SupplyParameterFromForm]` properties as null. Reproduced by `FormPostTests.Minimal_api_view_binds_named_form_values` before the fix. |
| Resolution | Fixed in Phase C. `RizzyViewResult` supplies the form when any Rizzy view executes. |
| Status / Confidence | Confirmed (runtime) / High |

## Notes from roadmap Phase D

- **RM-D1** was finished in Phase C (RM-C1): the form is read with `ReadFormAsync`, at most once per request, and the reflection lookup runs once per process. `FormPostTests.Multipart_form_is_read_asynchronously` posts a 200 KB multipart body to a TestServer, which disallows synchronous IO.
- **RIZZY-028 addition:** htmx 4 applies its `detail.error` logging convention only to events that htmx core triggers itself. The public `htmx.trigger()` dispatches without logging, so the streaming extension's `htmx:error` for a broken stream was never logged. Phase D routes extension errors through `Rizzy.diagnostics.report()`, which logs per the convention and then dispatches.
- **RIZZY-032 follow-up:** `benchmarks/Rizzy.Benchmarks` records end-to-end baselines (see its README). A full-page `View<T>` costs about 2× a plain `RazorComponentResult`; htmx requests cost about the same as the baseline.

### RIZZY-036: pages with their own `<HeadContent>` lost the htmx-config meta tag and antiforgery token

| | |
|---|---|
| Category / Severity | Functional and security-control defect / **P1** |
| Evidence | `RzView` added `<meta name="htmx-config">` through Blazor's `<HeadContent>`. Blazor shows only the most recently rendered `<HeadContent>`, and a page's own block renders after Rizzy's, so it replaced it. In the demo, `/home/` (which declares a description meta in `<HeadContent>`) had no htmx-config tag and no antiforgery configuration. `/home/weather` and `/home/counter` had both. Found in the browser by the Phase D client diagnostics. With the tag missing, unsafe htmx requests carry no antiforgery token and fail validation, and all `HtmxConfig` settings are ignored. |
| Resolution | Fixed. The new `RzHeadOutlet` renders `HeadOutlet` and then the config tag directly. It replaces `<HeadOutlet />` in the root layout, before the htmx script, because htmx reads the tag once at script load. `RzView` no longer uses `<HeadContent>`. Covered by `RenderingPipelineTests.Page_with_its_own_HeadContent_keeps_the_htmx_config_meta`. The client warning and the startup warning point to `RzHeadOutlet`. |
| Status / Confidence | Confirmed (browser) / High |

### Streaming verification

The streaming extension (RIZZY-003) is now checked end to end:

- **Server tests (`StreamingTests`):** for both partial htmx and full-page requests, the server sends the initial document (ending in `</html>`) while the component is still loading, then `<blazor-ssr>` blocks that match the component markers. This is the same shape as the client test fixture.
- **Demo in a browser:** streaming worked for boosted navigation, Back navigation and a direct page load.

## Findings from roadmap Phase E

### RIZZY-037: MVC model-state errors never reached the field they belong to

| | |
|---|---|
| Category / Severity | Functional defect / **P2** |
| Evidence | `RzModelStateProvider` passed each `ModelStateDictionary` key straight to `EditContext.Field(key)`. MVC model binding names keys with the binding prefix and property path (`Person.Name` for `[FromForm] Person person`). Blazor identifies a field by its owning object and property name (`(person, "Name")`), so no field matched. Errors appeared only in the validation summary: the field message stayed empty and the input stayed `valid`. Found while modernizing the demo's validation page. `FormRenderingTests.Model_state_keys_with_the_binding_prefix_reach_the_field` fails without the fix. |
| Resolution | Fixed. `RzModelStateProvider.ResolveField` treats a leading segment that is not a property of the form model as the binding prefix and walks nested paths to the owning object (`Person.Address.City` → `(address, "City")`). Keys it cannot follow (indexers, null objects) fall back to the raw key. Unit tests are in `ModelStateKeyTests`. Also fixed: since commit `c5e5342`, `RzController`/`RzControllerWithViews` passed `null` instead of the controller's `ModelState` unless one was given explicitly, although the documentation still described automatic cascading. They now default to the controller's `ModelState` again (`FormRenderingTests.Controller_views_receive_the_controller_ModelState_by_default`). |
| Status / Confidence | Confirmed (runtime, browser) / High |

### Phase E notes

- **RIZZY-033:** the remaining items are done. `llmquery.txt` and `stats.html` are removed and ignored, `.serena/` stays tracked (its own `.gitignore` excludes the local cache), `global.json` and `.editorconfig` are added, and CONTRIBUTING describes the real branch model and build steps. The stale solution entry was already gone.
- **RIZZY-024:** `npm audit` is clean (Vite 6.4.6). `aspnet-client-validation` (unused) and the unused test packages `Alba` and `System.IdentityModel.Tokens.Jwt` are removed. bUnit 2.14.0 removes the transitive AngleSharp advisory (NU1902). FluentAssertions stays on 7.x.
- **Demo (RM-E3):**
  - The demo sends a strict `script-src 'nonce-…'` CSP and uses `app.UseHtmxErrorHandling()`, the htmx 4 default error swapping and `hx-history-elt`.
  - A new Examples page covers table rows with an out-of-band button, `hx-confirm` with antiforgery, CSP nonces and server errors. The Validation page returns 422 with field errors.
  - The BL0008 warning is fixed.

## RM-A7 completed

- **Browser tests:** `test/e2e` holds a Playwright suite (15 tests, Chromium) against the demo, and CI runs it in an `e2e` job. It covers:
  - boosted navigation and back/forward history with `hx-history-elt`
  - native `hx-confirm` (dismiss and accept)
  - table rows with an out-of-band swap
  - progressive streaming
  - 422 validation
  - CSP nonce promotion vs. an injected script
  - server errors and antiforgery

  Each test also fails on unexpected console errors or Rizzy configuration warnings.
- **Coverage (2026-11-09):** line coverage is Rizzy 60.1%, Rizzy.Htmx 72.6% and Rizzy.SourceGenerators 83.6% (branch: 45.5%, 64.9% and 61.9%). This meets the RM-A7 target of at least 50% for the Rizzy package.

## htmx 4 surface coverage

An audit of htmx 4.0.1's source against Rizzy found complete typed coverage of headers, config, swap and trigger syntax, but typed builders for only 3 of 34 attributes, no event constants and no extension loading support. Rizzy now has:

- `Hx`/`HxAttributes` for every core and extension attribute
- `HtmxEvents` for every event
- the `HtmxExtension` catalog with allow-list sync
- `<RzScripts />`

Drift tests in `packages/rizzy/tests/htmx4-coverage.test.mjs` compare all of these with the installed htmx package. They found one attribute the first pass missed: `hx-history`, read by the history-cache extension.

### RIZZY-038: streaming switched off when another extension replaced `ctx.fetch`

| | |
|---|---|
| Category / Severity | Functional defect / **P2** |
| Evidence | `rizzy-streaming` wrapped `ctx.fetch` in its `htmx:before:request` hook. hx-preload's hook runs later and assigns `ctx.fetch = () => prefetchedResponse`, which dropped the wrapper. With Preload enabled, every boosted click (preloaded on `mousedown`) lost streaming: no placeholder first, and no stream events. Found by the Playwright suite after the demo enabled Preload. |
| Resolution | Fixed. `ctx.fetch` becomes an accessor: reads return the streaming wrapper, and each assignment adds a layer. Extensions that *wrap* the fetch (`const prev = ctx.fetch; ctx.fetch = ...prev(...)`) capture the wrapper, so it is re-entered from the layer above and calls the next layer down, reproducing the plain-assignment chain (stepping back up after each call, so a layer that retries reaches the same layer below); streaming is applied once to the final response. A first version that only stored the latest assignment sent wrapping extensions into infinite recursion; it was caught in review before commit. Covered by JS integration tests for both patterns (replace, and two wrapping extensions) and the browser streaming tests with Preload on. |
| Status / Confidence | Confirmed (browser) / High |
