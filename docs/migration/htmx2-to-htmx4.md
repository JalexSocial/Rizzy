# Upgrading to Rizzy 6 and htmx 4

Rizzy 6 targets **htmx 4**. This guide covers what changes for an application moving from Rizzy 5.x (htmx 2) to Rizzy 6 (htmx 4.0.1 or later).

1. [Compatibility](#compatibility)
2. [Upgrade steps](#upgrade-steps)
3. [Changes in your markup (htmx 4)](#changes-in-your-markup-htmx-4)
4. [Changes in Rizzy's behavior](#changes-in-rizzys-behavior)
5. [Breaking API changes in Rizzy 6](#breaking-api-changes-in-rizzy-6)
6. [New APIs](#new-apis)

Finding IDs such as RIZZY-006 refer to the [2026-11-08 audit](../audits/2026-11-08-rizzy-initial-audit/00-executive-summary.md).

## Compatibility

| Rizzy (NuGet + npm) | htmx | .NET | Notes |
|---|---|---|---|
| **6.x** | **4.0.1+** (4.x) | 10 | Rizzy, Rizzy.Htmx and `@jalexsocial/rizzy` always ship with the same version |
| 5.4.0 | 4.0 beta | 10 | **Broken**: depends on an incompatible Rizzy.Htmx 5.3.0 (RIZZY-001). Do not use |
| 5.3.x and earlier | 2.x | 8+ | htmx 2 line |

Always install Rizzy and Rizzy.Htmx with the same version. Rizzy 6 releases publish both packages, and the npm package, from one tag.

## Upgrade steps

1. **Update the packages.** Reference `Rizzy` 6.x; it brings the matching `Rizzy.Htmx`. If you use the npm package, install `@jalexsocial/rizzy` 6.x and `htmx.org` 4.0.1 or later. `htmx.org` is now a peer dependency, so your app chooses the exact htmx version.
2. **Replace `<HeadOutlet />` with `<RzHeadOutlet />` in your root layout, then load htmx 4 and Rizzy after it.** `RzHeadOutlet` renders Blazor's `HeadOutlet` plus Rizzy's `<meta name="htmx-config">` tag (htmx settings and the antiforgery token). htmx reads that tag once, when its script loads, so the outlet must come first. Rizzy 5 added the tag through `<HeadContent>`, but a page with its own `<HeadContent>` silently replaced it (RIZZY-036). Load htmx from a CDN with subresource integrity, for example:
   ```html
   <RzHeadOutlet />
   <script src="https://cdn.jsdelivr.net/npm/htmx.org@4.0.1/dist/htmx.min.js"
           integrity="sha384-Qp9rdC8H7EqOoX+nR0P3RPG2Noji+qdwJlGpfzwQJsfOpp3sGwIdSP9xO3+UlEhn"
           crossorigin="anonymous" nonce="@nonce"></script>
   <script src="_content/Rizzy/js/rizzy.js" nonce="@nonce"></script>
   ```
3. **Register Rizzy.** `AddRizzy()` now also registers htmx support and Razor components, so it is all you need. Call `AddHtmx(...)` as well only to configure `HtmxConfig`; the order does not matter and repeated calls are safe:
   ```csharp
   builder.Services.AddRizzy(o =>
   {
       o.RootComponent = typeof(HtmxApp<AppLayout>);   // AppLayout renders <RzHeadOutlet /> in <head>
       o.DefaultLayout = typeof(HtmxLayout<MainLayout>);
   });
   builder.Services.AddHtmx(o => o.DefaultTimeout = TimeSpan.FromSeconds(30)); // optional
   ```
   If your MVC controllers use `[ValidateAntiForgeryToken]` or `[AutoValidateAntiforgeryToken]`, register MVC with `AddControllersWithViews()` (or `AddMvcCore().AddViews()`). `AddControllers()` alone does not provide the filters those attributes need, and Rizzy no longer injects them through reflection.
4. **Run the official htmx upgrade checker** on your templates:
   ```bash
   npx -p htmx.org@4 upgrade-check --ext .razor --ext .cshtml ./
   ```
   It cannot see attribute values built at runtime (`Trigger`, `SwapStyleBuilder`, response headers). Rizzy 6's builders already produce htmx 4 syntax for those.
5. **Work through the sections below.** The fastest check is clicking through the app with the browser console open. htmx 4 logs every error.

## Changes in your markup (htmx 4)

These follow from htmx 4 itself; see [What's new in htmx 4](https://four.htmx.org/docs/whats-new-in-htmx-4/).

| Change | What to do |
|---|---|
| Inheritance is explicit | Add `:inherited` to attributes meant to cascade to children (`hx-boost:inherited`, `hx-target:inherited`, `hx-confirm:inherited`, `hx-headers:inherited`, …), or set `HtmxConfig.ImplicitInheritance = true` |
| Attribute renames | `hx-disable` → `hx-ignore`, then `hx-disabled-elt` → `hx-disable`. `hx-vars`, `hx-params`, `hx-ext` and `hx-request` are gone (use `hx-vals`, `hx-config`, `htmx.config.extensions`) |
| Trigger syntax | Values containing spaces or commas must be quoted: `from:'closest form'`, `target:'.a, .b'`. The htmx 2 `{...}` form is not recognized. The `queue:` modifier is gone; use `hx-sync="this:queue last"` |
| Swap syntax | `focus-scroll` → `focusScroll`. New modifiers: `strip`, `target`, `swapEmpty`; new style `outerSync` |
| Error responses swap | 4xx and 5xx responses are swapped like 200 (only 204/304 are not). See [Errors and validation](#errors-and-validation) |
| `hx-delete` sends no form data | Add `hx-include="closest form"` if the endpoint needs the form fields |
| No history cache | Back/forward re-fetches the page (`HX-History-Restore-Request`). Rizzy renders the full layout for these requests. Add `hx-history-elt` to the element that should be restored, or set `HtmxConfig.History = HtmxHistoryMode.Reload` |
| Event names | `htmx:afterSwap` → `htmx:after:swap`, `htmx:responseError` → `htmx:response:error`, etc. Update your JS listeners |
| Extension allow-list | If you set `HtmxConfig.Extensions`, include `rizzy-nonce, rizzy-streaming, rizzy-antiforgery, rizzy-confirm` |
| 60-second default timeout | Set `HtmxConfig.DefaultTimeout` if requests legitimately take longer. Rizzy clears the timeout while a Blazor streaming response is in progress |
| Attribute prefix | htmx 4 accepts `data-hx-*` as well as `hx-*` by default. `HtmxConfig.Prefix` changes or disables the extra prefix |

## Changes in Rizzy's behavior

### Full page or fragment (RIZZY-006, RIZZY-017, RIZZY-018)

`HtmxLayout`/`HtmxApp` (and `RzPageTitle`) now render the fragment layout **only** for partial htmx requests. That means `HX-Request-Type: partial` (or an htmx 2-style request without that header), excluding history restores. These get the full layout:

- normal browser requests;
- `HX-Request-Type: full`, which htmx sends for boosted navigation that targets `body` and for requests using `hx-select`;
- history restores (`HX-History-Restore-Request: true`). htmx 4.0.1 sends these **without** `HX-Request`.

Every Rizzy page response now sends `Vary: HX-Request, HX-Request-Type, HX-History-Restore-Request`, so browsers and proxies cannot serve a fragment for a normal navigation. Check your own action filters if they use `Request.IsHtmx()` to choose between a page and a fragment. Use `Request.Htmx().IsPartial` instead.

### Streaming rendering (RIZZY-003)

The `rizzy-streaming` extension no longer replaces htmx's response handling. htmx core processes the initial HTML: `hx-status`, `HX-*` headers, `htmx:after:request`, `htmx:response:error`, the swap and `HX-Trigger`. Rizzy then patches `<blazor-ssr>` updates in as they stream. Visible differences:

- `HX-Trigger` fires **once**, after the swap. On a streaming response it fires when the stream ends.
- The request indicator stays on until a streaming response ends, and `hx-sync` queues wait for it.
- `HX-Trigger-After-Swap`/`HX-Trigger-After-Settle` are no longer special. htmx 4 does not have them; use `HX-Trigger`.

### CSP nonces (RIZZY-004, RIZZY-027)

The `rizzy-nonce` extension no longer parses and re-serializes responses. That parsing dropped `<tr>`/`<td>` fragments and stripped every `<style>`/`<link>` without a nonce. Instead it rewrites the response nonce (the `HX-Nonce` header) to the page nonce in the raw text, and the browser's CSP decides what runs. Without a CSP nonce on the page, responses swap exactly as htmx would swap them.

- Put the nonce on your `<script>` tags (see the upgrade steps). The client reads the page nonce from there.
- The page nonce is **no longer written into the `htmx-config` meta tag**, where CSS attribute selectors could read it.
- Leave `HtmxConfig.GenerateScriptNonce`/`InlineScriptNonce` unset. They make htmx stamp the page nonce on every script in every response, injected ones included.

### Confirm dialogs (RIZZY-005)

`hx-confirm` shows the browser's native `confirm()` unless a custom dialog takes over. A dialog takes over by listening for `rz:confirm-dialog` and calling `event.preventDefault()` or setting `event.detail.handled = true`, then calling `event.detail.confirm()`, `deny()` or `dismiss()`. You can also opt in globally with `htmx.config.rizzyConfirmDialog = true`. Previously, without a listener, the request never ran.

### Errors and validation (RIZZY-019)

Rizzy keeps htmx 4's default (error responses swap) and adds helpers instead of reverting to htmx 2's `NoSwap`:

- **Validation:** re-render the form and call `Response.Htmx().ValidationFailed()` (status 422), optionally with a retarget selector or swap style. The page can also route it: `hx-status:422="target:#errors"` (`HtmxStatus.Attribute(422)` builds the attribute name).
- **Server errors:** `app.UseHtmxErrorHandling()` (before `UseExceptionHandler`) keeps a 5xx page from being swapped into a fragment target. It sends `HX-Reswap: none` plus an `rz:server-error` event, or retargets into `options.ErrorTarget`.
- **Login redirects:** `AddCookie(o => o.Events.UseHtmxRedirects())` answers htmx requests with `HX-Redirect` instead of a 302 that would swap the login page into the target.

If you prefer htmx 2 behavior, set `HtmxConfig.NoSwap = [204, 304, "4xx", "5xx"]`.

### Setup, forms and raw markup (RIZZY-020, RIZZY-026, RIZZY-029, RIZZY-031)

- **Startup checks.** Rizzy validates its configuration when the app starts. A null `RootComponent` fails startup. Leaving the default `HtmxApp<EmptyLayout>` logs a warning, because that root renders no `<head>`, so the htmx config and antiforgery token are dropped.
- **`[SupplyParameterFromForm]` in views.** Rizzy now supplies the posted form when a Rizzy view *executes*, reading it asynchronously. This makes form binding work for minimal-API endpoints too, not only `RzController` actions. It still relies on an internal ASP.NET Core API, now resolved once and failing with a clear message if a future framework version changes it.
- **Empty bodies.** `EmptyBody()`, `Redirect()` and `Refresh()` turn any Rizzy view into a 204, even when called after `View()` returned.
- **Raw HTML is explicit.** APIs that insert HTML as-is now take `MarkupString`. Write `AddRawContent((MarkupString)html)`, which marks the spot for review. Use the new `AddSwappableText`/`AddChildText` for text, which is HTML-encoded. Swap target ids are CSS-escaped, so ids like `item:1` work.

### Client diagnostics (RIZZY-026, RIZZY-028)

- When rizzy.js loads, it logs one console warning if the page has no `<meta name="htmx-config">` (the root layout does not render `<RzHeadOutlet />`), if that tag is not valid JSON, or if the server's Rizzy major version differs from the script's (for example a stale CDN copy). Set `HtmxConfig.RizzyDiagnostics = false` to silence these warnings.
- Errors in Rizzy's extensions, such as a streaming connection that breaks mid-response, are now logged the way htmx 4 logs its own errors (`htmx: htmx:error: ...`) and then dispatched as events. Previously a broken stream raised `htmx:error` without logging anything.

## Breaking API changes in Rizzy 6

| Before (5.x) | Rizzy 6 | Finding |
|---|---|---|
| `Trigger...Queue(TriggerQueueOption)`, `HtmxTriggerSpecification.Queue`, `TriggerQueueOption` | Removed: htmx 4 has no `queue` modifier. Use `hx-sync` | RIZZY-007 |
| Trigger/swap selectors rendered bare or as `{...}` | Quoted when they contain spaces or commas | RIZZY-007, RIZZY-013 |
| Times formatted with the current culture (`delay:1,5s` in `de-DE`) | Invariant: whole seconds as `2s`, otherwise milliseconds (`1500ms`) | RIZZY-012 |
| `HtmxTriggerSpecification` cached its string on first `ToString()` | Rendered on every call; `with`-copies render their own values | RIZZY-011 |
| `SwapStyleBuilder.ScrollFocus()` rendered `focus-scroll` | `focusScroll` | RIZZY-013 |
| `AjaxContext.Values`/`Headers` were `string` | `IDictionary<string, string>` (JSON objects, as `htmx.ajax` expects); new `SelectOob`, `Push`, `Replace`, `Transition` | RIZZY-014 |
| `AjaxContext.Handler` | Removed: `htmx.ajax` has no handler option | RIZZY-014 |
| `HX-Trigger`: the first detail for an event name won (case-insensitive) | The last call replaces the detail; names are case-sensitive; header is valid, ASCII-only JSON; details use the app's `JsonOptions` (camelCase by default) | RIZZY-015 |
| `Uri` overloads sent unescaped, non-ASCII header values | Percent-encoded (IDN hosts as punycode); string overloads too | RIZZY-016 |
| `HtmxRequest.TargetId`/`SourceId` returned the tag name when the element had no id | `null`; ids are URI-decoded | RIZZY-017 |
| `HtmxRequestAttribute` derived from `ActionFilterAttribute` | Plain action constraint (`Attribute, IActionConstraint`) | RIZZY-017 |
| `HtmxConfig.InlineStyleNonce`, `GenerateStyleNonce` | Removed: htmx 4 has no style nonce setting | RIZZY-028 |
| `ScrollBehavior`, `TriggerSpecificationCache` | Removed (unused in htmx 4) | RIZZY-028 |
| `htmx-config` meta tag contained `documentNonce` | No longer emitted; contains `rizzyVersion` | RIZZY-027, RIZZY-028 |
| `rizzy-confirm` always waited for a custom dialog | Native `confirm()` unless a dialog takes over | RIZZY-005 |
| The htmx-config meta tag was added through `<HeadContent>` (and lost on pages with their own `<HeadContent>`) | Render `<RzHeadOutlet />` instead of `<HeadOutlet />` in the root layout | RIZZY-036 |
| `AddHtmx()` registered MVC's internal antiforgery filters through reflection | Not registered; use `AddControllersWithViews()` for `[ValidateAntiForgeryToken]` | RIZZY-020 |
| `AddRizzy()` needed `AddHtmx()` and `AddRazorComponents()` | Registers both (idempotently); registrations use `TryAdd*` | RIZZY-026 |
| `RizzyConfig` setters threw `System.Exception` | `ArgumentException` (with the parameter name) | RIZZY-026 |
| `IRizzyService`/controller `View`/`PartialView` returned `RazorComponentResult` or `NoContent` | Return `RizzyViewResult` (still `IResult`); the 204 check runs at execution | RIZZY-030 |
| `IHtmxSwapService.AddSwappableContent(string, string)`, `AddRawContent(string)` | Take `MarkupString`; new `AddSwappableText(...)` encodes | RIZZY-029 |
| `RizzyComponentParameterBuilder.AddChildContent(string markup)` | `AddChildContent(MarkupString)`; new `AddChildText(string)` encodes | RIZZY-029 |
| `HtmxResponse.StatusCode(...)` required an htmx request; `Reswap(string)`/`Reswap(SwapStyleBuilder)` did not | `StatusCode` works for any request; every `HX-*` setter requires an htmx request; `Reswap(SwapStyle.Default)` no longer throws | RIZZY-030 |
| `ServiceCollectionExtensions.ConfigureHtmxSettings`, `HtmxAntiforgeryOptions.CookieName`, `antiforgery.cookieName` in the meta tag | Removed (unused) | RIZZY-030 |

## New APIs

- **Full htmx 4 coverage:** `Hx`/`HxAttributes` (every core and extension attribute), `HtmxEvents` (every event), `HtmxExtension` + `HtmxConfig.UseExtensions(...)`/`RestrictExtensions`/`Set(...)`, and `<RzScripts />` (htmx, rizzy.js and enabled extensions, with SRI and the CSP nonce). Replace hand-written htmx and rizzy.js script tags with `<RzScripts />` after `<RzHeadOutlet />`.
- `HtmxRequest.Type` (`HtmxRequestType`), `IsPartial`, `IsFullPage`.
- Trigger modifiers: `Prevent()`, `Stop()`, `Halt()`, `Capture()`, `Passive()`, `FromSelf()`, `FromOutside()`, `RootMargin()`.
- Swap modifiers: `Strip()`, `Target()`, `SwapEmpty()`; `SwapStyle.outerSync`.
- `HtmxResponse.TriggerOn(event, selector[, detail])`, `ValidationFailed(...)`.
- `HtmxStatus.Attribute(...)`, `UseHtmxErrorHandling(...)`, `CookieAuthenticationEvents.UseHtmxRedirects()`.
- `HtmxConfig.AllowEmptySwapAfterOOB`, `MetaCharacter`.
- Client: `window.Rizzy.confirm(props)`, `htmx.config.rizzyConfirmDialog`.
- Client: `window.Rizzy.diagnostics.report(elt, eventName, detail)`, `window.Rizzy.diagnostics.checkPage()`, the `htmx:rizzy:config:warn` event; server: `HtmxConfig.RizzyDiagnostics`.
- `RzHeadOutlet` (HeadOutlet plus the htmx-config meta tag).
- `HtmxResponse.LocalRedirect(...)`, `LocalLocation(...)`, `HtmxResponse.IsLocalUrl(...)` (open-redirect safe).
- `CssSelector.Escape(...)`, `CssSelector.Id(...)`.
- `IHtmxSwapService.AddSwappableText(...)`, `RizzyComponentParameterBuilder.AddChildText(...)`.
- **`[RizzyParameterize]` for your own components.** The source generator now ships in the Rizzy package. Mark a `partial` component with `[Rizzy.RizzyParameterize]` to get a typed `Params(...)` helper. It reports `RZ0001` for non-partial classes and `RZ0002` for nested ones. It requires the .NET 10 SDK compiler.
