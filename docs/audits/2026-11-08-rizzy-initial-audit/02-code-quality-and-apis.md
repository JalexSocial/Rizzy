# 02 — Code Quality and Public APIs

| Field | Value |
|---|---|
| Audit | Rizzy initial audit, 2026-11-08 |
| Repository / commit | https://github.com/JalexSocial/Rizzy @ `7e3f0f1126ee412d9db4f37d13d0370159edac15` |
| Scope | Confirmed defects, maintainability, public API design, ASP.NET Core integration, compatibility risks |
| Methodology / limitations | See [00-executive-summary.md](00-executive-summary.md#methodology). Runtime confirmations used a throwaway console harness that referenced `src/Rizzy/bin/Release/net10.0/*.dll` |

htmx-specific API issues (trigger and swap syntax, ajax options, config drift) are covered in [03-htmx4-compatibility.md](03-htmx4-compatibility.md). Security-specific issues are in [04-security-and-performance.md](04-security-and-performance.md).

---

## Confirmed defects

### RIZZY-008 — `DataAnnotationsProcessor` property cache is keyed by model type only

| | |
|---|---|
| Category / Severity | Correctness / **P1** |
| Subsystem | Forms and validation (`src/Rizzy/Components/Form/DataAnnotationsProcessor.cs`) |
| Evidence | `_propertyCache` is `ConcurrentDictionary<Type, PropertyCacheEntry>` (line 32). `GetCachedProperty` calls `_propertyCache.GetOrAdd(fieldIdentifier.Model.GetType(), …)` (line 168) and caches **the first field's** `PropertyInfo` and `ValidationAttribute[]` for the whole model type, process-wide (static). |
| Reproduction | Model `Person { [Required] Name; [EmailAddress] Email }`. `MergeAttributes(() => m.Name)` returns `data-val-required="The Name field is required."`. `MergeAttributes(() => m.Email)` then returns **`data-val-required="The Email field is required."`**, with no email rule (harness output). |
| Impact | Every Rizzy form input after the first field of a model renders the wrong `data-val-*` attributes. Client-side validation is wrong (spurious "required", missing range/regex/email). Server-side validation is unaffected. The result depends on which field renders first after app start, so it varies between processes. |
| Recommendation | Key the cache by `(Type, string FieldName)`. Add unit tests for multi-field models and inherited/shadowed properties (`GetProperty` can throw `AmbiguousMatchException`). |
| Status / Confidence | Confirmed (runtime) / High |

### RIZZY-009 — `HtmxResponseAttribute` is unusable for its boolean/enum options and throws for non-htmx requests

| | |
|---|---|
| Category / Severity | Correctness, API design / **P1** |
| Subsystem | `src/Rizzy.Htmx/HtmxResponseAttribute.cs` |
| Evidence | (a) `PreventBrowserHistoryUpdate`, `PreventBrowserCurrentUrlUpdate`, `Refresh` are `bool?` and `Reswap` is `SwapStyle?` (lines 25, 30, 40, 50). Nullable value types are not valid attribute argument types: `[HtmxResponse(Refresh = true)]` fails with **CS0655** (harness compile). (b) `OnResultExecuting` (line 69) calls `HtmxResponse` methods that call `AssertIsHtmxRequest()`. For a normal browser request, such as direct navigation or a crawler, it throws `InvalidOperationException` and the request returns 500 (harness: `THREW InvalidOperationException`). |
| Impact | Four of the ten options can never be used. Any action decorated with `[HtmxResponse(PushUrl = …)]` breaks progressive enhancement and fails for non-htmx clients. |
| Recommendation | Use non-nullable `bool` and a sentinel `SwapStyle.Default`. Make the filter a no-op when `!Request.IsHtmx()`. More broadly, reconsider throwing from `HtmxResponse` setters on non-htmx requests. Setting `HX-*` headers on a normal response is harmless, while throwing turns benign cases into 500s. Make it configurable or log instead. Add tests. |
| Status / Confidence | Confirmed (runtime + compile) / High |

### RIZZY-011 — `HtmxTriggerSpecification` combines a mutable record with a cached `Lazy<string>`

| | |
|---|---|
| Category / Severity | Correctness / **P2** |
| Subsystem | `src/Rizzy.Htmx/HtmxTriggerSpecification.cs` (lines 24, 85, 89) |
| Evidence | `ToString()` returns `_lazyToString.Value`. The `Lazy` is created in the constructor and bound to the original instance. Harness results: (1) `spec with { Once = true }` copies the `Lazy` field, so `copy.ToString()` returns the **original** string (`"click"`, expected `"click once"`). (2) Two specs with identical properties are not equal (`==` is `False`), because record equality compares the `Lazy` by reference. (3) Setting a property after the first `ToString()` returns a stale string (`"keyup"` instead of `"keyup delay:500ms"`). `[DebuggerDisplay("{ToString(),nq}")]` on the builders means just inspecting a builder in a debugger mid-chain can freeze a stale value. |
| Impact | Silent wrong `hx-trigger` values for anyone who reuses or derives specs. `TriggerBuilder.AddTrigger`'s `Contains` de-duplication never matches. |
| Recommendation | Remove the `Lazy` cache (building the string costs about 1 µs, see RIZZY-032), or make the record immutable (`init`) and exclude the cache from equality. |
| Status / Confidence | Confirmed (runtime) / High |

### RIZZY-012 — Culture-sensitive number formatting in htmx time values

| | |
|---|---|
| Category / Severity | Correctness, globalization / **P2** |
| Subsystem | `src/Rizzy.Htmx/SwapStyleBuilder.cs` (lines 38, 59) and `HtmxTriggerSpecification.FormatTimeSpan` (lines 193–196) |
| Evidence | Interpolated `{time.TotalSeconds}` uses `CurrentCulture`. Under `de-DE`, `AfterSwapDelay(1500ms)` produces `innerHTML swap:1,5s` and `Delay(1500ms)` produces `input delay:1,5s` (harness). |
| Impact | On servers or requests with comma-decimal cultures (most of Europe, or `UseRequestLocalization`), timing modifiers are invalid. |
| Recommendation | Format with `CultureInfo.InvariantCulture`. Prefer whole milliseconds (`1500ms`). Add a culture-switching unit test. |
| Status / Confidence | Confirmed (runtime) / High |

### RIZZY-015 — `HX-Trigger` JSON built by string concatenation

| | |
|---|---|
| Category / Severity | Correctness, robustness / **P2** |
| Subsystem | `src/Rizzy.Htmx/HtmxResponse.cs` `MergeTrigger` / `TriggerHeaderEventSet.ToString` (lines 301–334) |
| Evidence | The event name is interpolated into JSON without escaping: `Trigger("evt\"x", new { a = 1 })` produces `{"evt"x":{"a":1}}`, which is malformed (harness). Calling `Trigger("a", {v=1})` then `Trigger("a", {v=2})` keeps only the first detail. The second is silently dropped. Mixed calls emit `"name":null` for events without detail. |
| Impact | Malformed header means htmx's parse fails and no events fire. Duplicate names silently lose data. |
| Recommendation | Build the header with `Utf8JsonWriter`. Define and document merge semantics (last-wins or error). Emit `{}` rather than `null`. |
| Status / Confidence | Confirmed (runtime) / High |

### RIZZY-016 — `Uri` overloads produce unescaped (non-ASCII) header values

| | |
|---|---|
| Category / Severity | Correctness / **P2** |
| Subsystem | `HtmxResponse.PushUrl(Uri)`, `Redirect(Uri)`, `ReplaceUrl(Uri)` (lines 100–104, 148–152, 183–187) |
| Evidence | They use `url.ToString()`, which **unescapes**. `PushUrl(new Uri("https://example.com/caf%C3%A9?q=a%20b"))` writes `https://example.com/café?q=a b` (harness: `nonAscii=True`). By default Kestrel rejects non-ASCII response header values with `InvalidOperationException`. |
| Impact | Likely 500 errors for URLs with encoded non-ASCII characters. An unencoded space corrupts the URL even where it is accepted. |
| Recommendation | Use `uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString`. Add tests with encoded segments. |
| Status / Confidence | Confirmed (runtime) for the header value. The Kestrel rejection is documented platform behavior and was not executed / High |

### RIZZY-017 — `HtmxRequest` target/source parsing and request-type API

| | |
|---|---|
| Category / Severity | Correctness, API design / **P2** |
| Subsystem | `src/Rizzy.Htmx/HtmxRequest.cs` (lines 53, 58, 63), `HtmxRequestAttribute` |
| Evidence | `TargetId => Target?.Split('#').LastOrDefault()`. For an element without an id, htmx 4 sends `HX-Target: div`, and `TargetId` returns **`"div"`** (harness), a tag name. `[HtmxRequest(Target = "div")]` would therefore match any id-less div target. `RequestType` is an untyped string with no `IsPartial`/`IsFullPage` helpers. `HtmxContext` constructs a new, uncached `HtmxRequest`. `HtmxRequestAttribute` derives from `ActionFilterAttribute` but only implements `IActionConstraint`. |
| Impact | Wrong routing or logic decisions in apps that branch on target id. Developers must know htmx 4's header formats for ordinary request classification. |
| Recommendation | Return `null` when there is no `#`. Add an `HtmxRequestType { Partial, Full }` enum with `IsPartial`/`IsFullPage` helpers that also account for history restore (needed for RIZZY-006). Make `HtmxContext` use the cached request. |
| Status / Confidence | Confirmed (runtime) / High |

---

## Maintainability and architectural concerns

### RIZZY-020 — Reflection over ASP.NET Core internal types

| | |
|---|---|
| Category / Severity | Architectural concern, upgrade risk / **P2** |
| Subsystem | `src/Rizzy.Htmx/ServiceCollectionExtensions.cs` `AddAntiforgeryValidation` (line 64). `src/Rizzy/Extensions/HttpContextExtensions.cs` `InitializeBlazorFormData` (line 54), which `RzController.OnActionExecuting` calls on every form request with a `_handler` field |
| Evidence | `Assembly.Load("Microsoft.AspNetCore.Mvc.ViewFeatures").GetTypes().First(t => t.Name == "AutoValidateAntiforgeryTokenAuthorizationFilter")`, and `Assembly.Load("Microsoft.AspNetCore.Components.Endpoints")` looking up `HttpContextFormDataProvider`, `FormCollectionReadOnlyDictionary` and `SetFormData`. The same reflection helper is duplicated in `src/RizzyDemo/Helpers/ServiceCollectionExtensions.cs`. `InitializeBlazorFormData` also reads `Request.Form` synchronously inside a filter (sync-over-async when the form isn't already buffered). |
| Impact | An ASP.NET Core update that renames these internals breaks app startup (`First` throws) or every Blazor form POST, with no compile-time signal and no tests covering it. Per-request cost is small (about 24 µs and 2.5 KB per POST, see RIZZY-032). |
| Recommendation | (1) Antiforgery: document `AddControllersWithViews()` or `AddMvcCore().AddViews()`, which register these filters, and drop the reflection. (2) Form data: investigate a supported registration path. Failing that, cache the `MethodInfo` statically, fail with a clear diagnostic, use `ReadFormAsync` in `OnActionExecutionAsync`, and add an integration test that fails loudly on framework upgrades. |
| Status / Confidence | Architectural concern (static) / High |

### RIZZY-026 — Onboarding defaults and setup pitfalls (code aspects)

| | |
|---|---|
| Category / Severity | Developer experience / **P2** |
| Subsystem | `AddRizzy`, `AddHtmx`, `RizzyConfig`, `EmptyRootComponent` |
| Evidence | (1) `AddRizzy` does not call `AddHtmx`. Without `AddHtmx`, `HtmxAntiforgeryOptions` stays unconfigured (`FormFieldName`/`HeaderName` null), so `HtmxConfigHeadOutlet` emits an antiforgery block with null names, the JS injects no token, and unsafe requests fail antiforgery validation. (2) The default `RootComponent = HtmxApp<EmptyLayout>` renders no `<head>`/`HeadOutlet`, so the `htmx-config` meta is silently dropped unless the app supplies a root layout with `<HeadOutlet/>`. (3) `RizzyConfig` setters throw bare `System.Exception` with the message `"value is not…"`. (4) The demo comment says "This must be added after AddControllers", but the code calls it before. |
| Impact | A first-time adopter following the READMEs gets no antiforgery and no config, with no error. |
| Recommendation | Have `AddRizzy` call `AddHtmx` idempotently. Add options validation (`ValidateOnStart`) and a logged warning when the config meta cannot render. Throw `ArgumentException`. Documentation aspects are in [05](05-testing-and-release.md). |
| Status / Confidence | Confirmed (static) / High |

### RIZZY-029 — `HtmxSwapService` raw-markup API and selector construction

| | |
|---|---|
| Category / Severity | API safety / **P3** |
| Subsystem | `src/Rizzy/Components/Swap/Services/HtmxSwapService.cs`, `HtmxSwappable.cs` |
| Evidence | `AddSwappableContent(targetId, string content)` and `AddRawContent(string)` insert **unencoded HTML** via `AddMarkupContent`, and the names don't signal it. `HtmxSwappable` builds `#{TargetId}` without CSS escaping, so ids with `:`, `.`, or leading digits break the selector. The constructor captures `HttpContext!` eagerly. `HtmxSwapContent` subscribes to `ContentItemsUpdated` and never unsubscribes (request scope only). |
| Impact | Easy XSS if developers pass user data to the "content" overloads. Swaps silently target nothing for unusual ids. |
| Recommendation | Rename to `…Markup`, or accept `MarkupString`. Add an encoded-text overload. Escape ids. Resolve `HttpContext` lazily. |
| Status / Confidence | Confirmed (static) / High |

### RIZZY-030 — Duplication and small API inconsistencies

| | |
|---|---|
| Category / Severity | Maintainability / **P3** |
| Evidence | `RzController` and `RzControllerWithViews` duplicate every `View`/`PartialView` overload. `RizzyService` repeats the `EmptyResponseBodyRequested` check in eight methods. `HtmxResponse.Reswap(string)` and `Reswap(SwapStyleBuilder)` skip `AssertIsHtmxRequest()` while the others call it. `Reswap(SwapStyle.Default, null)` calls `Reswap(null!)` and throws. `StatusCode()` requires an htmx request for no clear reason. `ConfigureHtmxSettings.Configure` is empty. `HtmxAntiforgeryOptions.CookieName` is unused. `EnumMemberJsonConverter`, `TriggerSpecificationCache`, `ScrollBehavior` and `HtmxContext` have no internal usage. `AddRizzy` uses `AddScoped` rather than `TryAdd*`, so calling it twice registers duplicates. |
| Recommendation | Share one implementation between the controllers. Centralize the 204 short-circuit. Make the assertion policy consistent. Remove or obsolete dead types in the next major. |
| Status / Confidence | Confirmed (static) / High |

---

## ASP.NET Core integration assessment

| Area | Assessment |
|---|---|
| Service registration | Idiomatic options pattern. Issues: two entry points with an implicit dependency (RIZZY-026), reflection-based filter registration (RIZZY-020), and non-`TryAdd` registrations (RIZZY-030). |
| Middleware ordering | `UseRizzy()` only adds an `HX-Nonce` header, so its ordering is harmless. |
| Endpoint routing | Works with MVC and minimal APIs (`IRizzyService` returns `IResult`). `HtmxRequestAttribute` integrates correctly with action selection. |
| Request/response | `HttpRequest.Htmx()` is cached per request. Trigger state merges via `HttpContext.Items`, which is correct. |
| DI scopes | Lifetimes are appropriate. Scoped services capture `HttpContext` eagerly (`HtmxSwapService`). |
| Logging | **No `ILogger` usage anywhere in the library.** Misconfiguration and reflection failures are silent. |
| Configuration | `IOptionsSnapshot<HtmxConfig>` per render. `HtmxConfig` is a record with per-request `with`-copies, which is fine. |
| Hosting environments | No environment-specific behavior. Consider defaulting `LogAll` from `IsDevelopment()`. |
| Exception handling | No integration. htmx 4 now swaps error responses, so `UseExceptionHandler` pages get swapped into fragments (RIZZY-019). |
| Authentication/authorization | No integration. `fetch` follows 302 login challenges, and the login page HTML gets swapped into the target. There is no guidance or `HX-Redirect`-on-401 helper. |
| Antiforgery | Good client injection via header. MVC validation relies on reflection-registered filters (RIZZY-020). Minimal-API endpoints rely on `UseAntiforgery`, which only validates form-bound endpoints. This is undocumented. |
| Razor/Blazor | Good use of `RazorComponentResult`, streaming SSR, `HeadContent`, and `SupplyParameterFromForm` (`AddSupplyValueFromFormProvider`). |
| Testing infrastructure | Hand-written mocks (about 300 lines) duplicate `DefaultHttpContext`. `Alba` is referenced but unused. No `WebApplicationFactory`/`TestServer` tests (RIZZY-025). |

**Duplication of platform features** is minor: `ObjectExtensions.ToDictionary` duplicates `RouteValueDictionary`/`HtmlHelper.AnonymousObjectToHtmlAttributes`, and the test mocks duplicate `DefaultHttpContext`.

---

## Public API inventory and review

| Area | Main public types | Observations |
|---|---|---|
| Setup | `AddHtmx`, `AddRizzy`, `UseRizzy`, `HtmxConfig`, `RizzyConfig` | Discoverable, but config has stale keys (RIZZY-028). **No XML documentation file is generated** (`GenerateDocumentationFile` is false or unset), so consumers get no IntelliSense docs despite extensive `///` comments. |
| Request | `HtmxRequest`, `IsHtmx()`, `Htmx()` | See RIZZY-017. |
| Response | `HtmxResponse`, `HttpResponse.Htmx()`, `HtmxResponseAttribute` | Fluent and readable. Issues: RIZZY-009, -015, -016, and the throwing policy. |
| Builders | `SwapStyleBuilder`, `Trigger`/`TriggerBuilder`/`TriggerModifierBuilder`, `SwapStyle` | Lower-case enum members are deliberate for markup but break .NET naming conventions. The emitted syntax is htmx 2 era (RIZZY-007, RIZZY-013). |
| Rendering | `IRizzyService`, `RzController`, `RzControllerWithViews`, `RizzyComponentParameterBuilder<T>` | Strong typing with good error messages. `View<T>(object)` reflects over anonymous types. |
| OOB | `IHtmxSwapService`, `HtmxSwappable`, `HtmxSwapContent` | See RIZZY-029. |
| Forms | `RzInput*Base`, `RzValidationMessageBase<T>`, `RzValidationSummaryBase`, `RzModelStateProvider`, `RzInitialValidator`, `DataAnnotationsProcessor` | Components overwrite their own `[Parameter] Id` in `OnParametersSet` (a Blazor anti-pattern). See also RIZZY-008. |
| Utilities | `IdGenerator`, `ObjectExtensions`, `RenderFragmentExtensions` | `SerializeAsAlpineData` is unsafe (RIZZY-002). `AsMarkupString` only captures markup frames and silently drops text, elements and components. |

**Exceptions:** `Rizzy.Htmx` uses `ArgumentNullException` and `InvalidOperationException` consistently. `RizzyConfig` throws bare `Exception`. Messages are generally clear, especially in `RizzyComponentParameterBuilder`.

**Do ordinary tasks need internals knowledge?** Yes, in three places:

- choosing full vs partial rendering (htmx 4 header semantics);
- getting the config meta to render (needs a custom root layout with `HeadOutlet`);
- knowing that the client extensions are always active.

## Backward-compatibility analysis

| Kind | Observation |
|---|---|
| Binary | Rizzy 5.4.0 is binary-incompatible with the published Rizzy.Htmx 5.3.0 (RIZZY-001). `EnablePackageValidation` is on, but with no `PackageValidationBaselineVersion` it cannot detect breaks between releases. |
| Source | The htmx 4 migration removed public members (SSE trigger APIs, htmx 2 config properties, `HX-Trigger-After-*` helpers) in a **minor** bump (5.3 → 5.4), which violates SemVer. `PublicApiTests` only asserts there are no `[Obsolete]` members. It doesn't snapshot the API. |
| Behavioral | The bundled JS extensions change request and response semantics globally (RIZZY-003/004/005). History-restore behavior changed in effect, because htmx 4 always re-fetches (RIZZY-006). |
| Configuration | `htmx-config` keys changed to htmx 4 names. The Rizzy-specific `antiforgery`/`documentNonce` keys are an undocumented, unversioned contract between the C# and JS packages. |

**Recommendation:**

- Ship the corrective release as **6.0.0**.
- Adopt `Microsoft.CodeAnalysis.PublicApiAnalyzers`, or package validation with a baseline version.
- Version the C# and JS packages in lockstep.
- Emit a `rizzyVersion` in the meta config, and have the JS warn when it doesn't match.

## Positive observations

- `RizzyComponentParameterBuilder<T>` gives type-safe component parameters, validates `[EditorRequired]`, and has excellent error messages.
- `HtmxConfigHeadOutlet` output is safely escaped: `'` becomes `'` and `<` becomes `<` (harness).
- The `HttpContext.Items` request cache, the component metadata cache, and the layout attribute cache are all appropriate.
- Nullable reference types are enabled throughout, and the build has no nullable warnings.
