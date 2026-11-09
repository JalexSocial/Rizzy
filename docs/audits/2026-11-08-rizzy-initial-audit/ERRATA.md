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
