# Changelog

All notable changes to Rizzy, Rizzy.Htmx and the `@jalexsocial/rizzy` npm package are recorded
here. The three are released together with one version number.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and Rizzy uses
[Semantic Versioning](https://semver.org/spec/v2.0.0.html):

- **Major** releases may contain breaking changes to public .NET APIs, the client script's
  behavior or required setup. Each one is listed under "Breaking" with a migration note.
- **Minor** releases add features without breaking existing code.
- **Patch** releases contain only fixes.
- Pre-releases (`6.0.0-preview.1`) may change between previews.

Finding IDs (RIZZY-0xx) refer to the
[2026-11-08 audit](docs/audits/2026-11-08-rizzy-initial-audit/00-executive-summary.md) and its
[errata](docs/audits/2026-11-08-rizzy-initial-audit/ERRATA.md).

## [Unreleased]

The 6.0.0 release: htmx 4 support. Upgrading from 5.x is covered step by step in
[docs/migration/htmx2-to-htmx4.md](docs/migration/htmx2-to-htmx4.md).

### Breaking

- **htmx 4 is required.** `htmx.org` is a peer dependency (4.0.1 or later), so your app loads htmx and
  chooses its version. Attribute inheritance is explicit (`:inherited`), and error responses swap by default.
- **Setup.**
  - Render `<RzHeadOutlet />` instead of `<HeadOutlet />` in the root layout, before the htmx script
    (RIZZY-036).
  - `AddRizzy()` registers htmx and Razor components itself, and repeated calls are safe (RIZZY-026).
  - `[ValidateAntiForgeryToken]` needs `AddControllersWithViews()`; Rizzy no longer injects MVC's
    filters through reflection (RIZZY-020).
- **Views.** `View`/`PartialView` return `RizzyViewResult`. The empty-body (204) check runs when
  the result executes (RIZZY-030).
- **Raw markup is explicit.** `AddSwappableContent`, `AddRawContent` and `AddChildContent` take
  `MarkupString`. Use the new `AddSwappableText`/`AddChildText` for text (RIZZY-029).
- **Builders emit htmx 4 syntax.**
  - Selectors are quoted when needed, and the removed `queue` modifier is gone (RIZZY-007, RIZZY-013).
  - Times use the invariant culture (RIZZY-012).
  - `ScrollFocus()` renders `focusScroll` (RIZZY-013).
  - `HtmxTriggerSpecification` no longer caches its string (RIZZY-011).
- **Ajax and request APIs.**
  - `AjaxContext.Values`/`Headers` are dictionaries, and `Handler` is removed (RIZZY-014).
  - `HtmxRequest.TargetId`/`SourceId` return `null` when the element has no id (RIZZY-017).
  - `HtmxRequestAttribute` is an action constraint (RIZZY-017).
- **Response headers.**
  - The last `HX-Trigger` detail for an event wins, and names are case-sensitive (RIZZY-015).
  - Non-ASCII header values are percent-encoded (RIZZY-016).
  - Every `HX-*` setter requires an htmx request, and `StatusCode` works for any request (RIZZY-030).
- **Removed.**
  - `HtmxConfig.InlineStyleNonce`/`GenerateStyleNonce`, `ScrollBehavior` and `TriggerSpecificationCache` (RIZZY-028).
  - `ConfigureHtmxSettings`, `HtmxAntiforgeryOptions.CookieName` and `EnumMemberJsonConverter` (RIZZY-030).
- **Meta tag.** The `htmx-config` meta tag no longer contains `documentNonce` or `cookieName`. It now
  carries `rizzyVersion` (RIZZY-027, RIZZY-028).
- **Confirm.** `hx-confirm` falls back to the native `confirm()` when no custom dialog takes over (RIZZY-005).

### Added

- **htmx 4 request and response helpers.**
  - `HtmxRequest.Type`, `IsPartial` and `IsFullPage`.
  - `HtmxResponse.ValidationFailed()` and `TriggerOn()`.
  - `HtmxStatus.Attribute()`, `UseHtmxErrorHandling()` and `CookieAuthenticationEvents.UseHtmxRedirects()` (RIZZY-019).
- **Trigger and swap modifiers.**
  - Trigger: `Prevent`, `Stop`, `Halt`, `Capture`, `Passive`, `FromSelf`, `FromOutside` and `RootMargin`.
  - Swap: `Strip`, `Target`, `SwapEmpty` and `SwapStyle.outerSync`.
  - Config: `HtmxConfig.AllowEmptySwapAfterOOB` and `MetaCharacter`.
- **Safe helpers.**
  - `HtmxResponse.LocalRedirect`/`LocalLocation`/`IsLocalUrl` (RIZZY-031).
  - `CssSelector.Escape`/`Id`; swap target ids are CSS-escaped (RIZZY-029).
- **`[RizzyParameterize]`.** The source generator ships in the Rizzy package (RIZZY-022).
- **Startup checks.** Validation fails startup for a missing `RootComponent`, and a warning is logged
  for a root without `<head>` (RIZZY-026).
- **Client diagnostics.**
  - One console warning for a missing or invalid htmx-config meta tag, or for a server/client
    version mismatch (`HtmxConfig.RizzyDiagnostics` turns it off).
  - Extension errors are logged the way htmx logs its own (RIZZY-026, RIZZY-028).
- **Client API.** `window.Rizzy.confirm()`, `htmx.config.rizzyConfirmDialog` and `window.Rizzy.diagnostics`.

### Fixed

- **Security.**
  - XSS in `SerializeAsAlpineData` (RIZZY-002).
  - The CSP nonce extension no longer re-parses responses, so table rows and other fragments survive.
    Injected scripts stay blocked (RIZZY-004, RIZZY-027).
  - Pages with their own `<HeadContent>` lost the antiforgery token (RIZZY-036).
- **Streaming rendering cooperates with htmx 4.** Swaps, `hx-status`, `HX-*` headers, triggers and
  `hx-sync` all work while blocks stream in. A broken stream is now logged (RIZZY-003).
- **Layouts and history.** Back/forward, boosted navigation and full-page htmx requests get the
  real layout. Responses vary on the htmx headers (RIZZY-006, RIZZY-018).
- **Forms.**
  - `DataAnnotationsProcessor` caches validation metadata per field (RIZZY-008).
  - `[SupplyParameterFromForm]` works for minimal-API views (RIZZY-035).
  - The form is read asynchronously (RIZZY-020).
  - MVC model-state errors show next to their field, and `RzController`/`RzControllerWithViews`
    pass the controller's `ModelState` to views again by default (RIZZY-037).
- **Response attribute.** `HtmxResponseAttribute` works on non-htmx requests (RIZZY-009).
- **Packaging.**
  - Rizzy depends on the matching Rizzy.Htmx version (RIZZY-001).
  - Stale bundles and stray content files are gone from the packages (RIZZY-021).

### Changed

- **Builds and tests.** CI builds, tests and packs every pull request, runs a consumer smoke test
  against the packed packages, and checks that the committed client bundle is current. Releases
  are published only from a version tag (RIZZY-010, RIZZY-023).
- **Dependencies.** Unused dependencies are removed, and the JS toolchain is updated (RIZZY-024).

## [5.4.0]

Published to nuget.org with a dependency on a pre-htmx-4 Rizzy.Htmx 5.3.0 (RIZZY-001). Do not use
it; upgrade to 6.0.0.

## 5.2.0 and earlier

See the [GitHub releases](https://github.com/JalexSocial/Rizzy/releases) and tags.

[Unreleased]: https://github.com/JalexSocial/Rizzy/compare/v5.2.0...HEAD
[5.4.0]: https://www.nuget.org/packages/Rizzy/5.4.0
