# 00 — Executive Summary

| Field | Value |
|---|---|
| Audit | Rizzy initial comprehensive audit and htmx 4 compatibility assessment |
| Date | 2026-11-08 |
| Repository | https://github.com/JalexSocial/Rizzy |
| Branch / commit | `main` @ `7e3f0f1126ee412d9db4f37d13d0370159edac15` ("chore: release htmx v4 compatible library") |
| Working tree | Clean, except an untracked `.serena/` tool directory (not part of the product) |
| Scope | Entire repository: `src/Rizzy`, `src/Rizzy.Htmx`, `src/Rizzy.SourceGenerators`, `src/RizzyDemo`, `test/Rizzy.Tests`, `packages/rizzy` (JS client), CI workflow, docs, and the **published** NuGet and npm artifacts |
| Reference target | htmx **4.0.1** (npm `next` tag, published 2026-11-01). npm `latest` is still htmx 2.0.11 |

## Methodology

1. Read all repository instructions, READMEs, project files, CI configuration and every C#, Razor and JS source file. The C# library code is about 8k lines.
2. Ran a non-destructive baseline: Release build of the solution, `dotnet test` with Coverlet coverage, and a clean `npm ci` / `npm test` / `npm run build` of the JS package in an isolated scratch copy. Also packed both NuGet packages into a scratch directory and inspected their contents.
3. Retrieved the official htmx 4 documentation (`llms.txt`, `llms-full.txt`/`docs.md`, *What's New in htmx 4*, header/attribute/config references) and the **htmx 4.0.1 npm package source**. Reviewed Rizzy's integration against that source, not against remembered htmx 2 behavior.
4. Ran the official **htmx upgrade checker** (`dist/scripts/upgrade-check.py` in htmx 4.0.1) against Razor, C#, JS and docs files.
5. Wrote throwaway verification harnesses outside the repository. They reference the locally built DLLs and reproduce suspected defects at runtime. A jsdom harness exercised the JS nonce sanitizer. An indicative Stopwatch microbenchmark covered hot paths.
6. Downloaded the **published** `Rizzy 5.4.0` and `Rizzy.Htmx 5.3.0` packages from nuget.org and compared their binaries with the repository build.

No production source, dependency or configuration was modified. The only files written to the repository are these reports and `docs/audits/README.md`.

## Important limitations

- **No browser-level testing.** htmx 4 runtime behavior was established from the official docs and the 4.0.1 source code. The demo app was not run in a real browser. Findings in that category are marked *Confirmed (static)* or *Reproducible incompatibility*.
- The external documentation site (`jalexsocial.github.io/rizzy.docs`) is client-rendered. Its content could not be audited in depth beyond confirming that some pages return 404.
- The local `packages/rizzy/node_modules` is stale (it has htmx 2.0.4 and is missing jsdom), so JS tests fail in place. A clean install was used for the JS baseline.
- Microbenchmarks are indicative Stopwatch measurements, not BenchmarkDotNet runs.
- No runtime test installed the *published* NuGet packages into an app. The dependency mismatch (RIZZY-001) is proven from package metadata and binary contents.

## Overall technical health

**Rating: fair core, with critical release and htmx 4 integration problems.**

Rizzy's idea is sound and its server-side core is small and understandable. It renders Razor components from MVC or minimal APIs via `RazorComponentResult`, swaps layouts for htmx requests, and offers a fluent API over htmx headers. The C# builds cleanly with only 5 warnings, uses nullable reference types and System.Text.Json source generation, and its JS bundle is reproducible.

The June 2026 htmx 4 migration (commit `270f7ec`) was done against htmx 4 betas. It left three kinds of problems:

1. A **broken published release**.
2. Several **client-side extensions that override or corrupt htmx 4 core behavior** in every Rizzy app.
3. **Server-side builders that still emit htmx 2 syntax.**

Test protection is weak where it matters: the core library has about 1.4% line coverage, and CI never runs the tests.

## Architectural strengths

- Clear package split: `Rizzy.Htmx` (protocol helpers, usable standalone) and `Rizzy` (Razor/MVC integration).
- It builds on ASP.NET Core primitives rather than replacing them (`RazorComponentResult`, `DynamicComponent`, `LayoutView`, options, DI).
- Typed, fluent htmx response API (`HtmxResponse`, `SwapStyleBuilder`, `TriggerBuilder`). Config serialization is JSON source-generated and correctly HTML-escaped.
- CSP-nonce awareness and automatic antiforgery-token injection are built in.
- The JS bundle builds reproducibly and is small (about 9.4 kB minified).

## Principal weaknesses and risks

| # | Finding | Severity |
|---|---------|----------|
| RIZZY-001 | The published **Rizzy 5.4.0 depends on Rizzy.Htmx ≥ 5.3.0**, but the only Rizzy.Htmx 5.3.0 on nuget.org predates the htmx 4 migration. It lacks types that Rizzy 5.4.0 calls (`HtmxAttributeName`, `HtmxConfig.Prefix`). NuGet consumers get a mismatched pair. | P0 |
| RIZZY-002 | `SerializeAsAlpineData` writes raw single-quoted strings without escaping backslashes. A crafted value breaks out of the string and runs JavaScript in Alpine `x-data` (XSS reproduced). | P0 |
| RIZZY-003 | The bundled `rizzy-streaming` extension intercepts **every** `text/html` htmx response and re-implements htmx core. `hx-status` is ignored, `htmx:after:request`/`htmx:response:error` never fire, `HX-Trigger` fires twice, and indicators and `hx-sync` queues are released before the swap. | P1 |
| RIZZY-004 | The bundled `rizzy-nonce` extension round-trips every response through `DOMParser`. Table-row fragments are destroyed, every un-nonced `<style>`/`<link>` is stripped even without CSP, and leading scripts are dropped. | P1 |
| RIZZY-005 | The bundled `rizzy-confirm` extension silently suppresses `hx-confirm` when the app has no custom dialog listener. The request never fires. | P1 |
| RIZZY-006 | Layouts are stripped for every `HX-Request`. htmx 4 re-fetches **every** back/forward navigation and expects a full page, so history restore loses the layout. | P1 |
| RIZZY-007 | `TriggerBuilder` emits htmx 2 syntax: unquoted `from:closest form`, brace-wrapped selectors, and the removed `queue:` modifier. | P1 |
| RIZZY-008 | `DataAnnotationsProcessor` caches validation metadata **per model type, not per field**. Every field after the first gets the first field's client validation rules. | P1 |
| RIZZY-010 | CI runs only on push to `main`, executes zero tests, publishes on every push with `--skip-duplicate` (which hid RIZZY-001), and ships `dotnet publish` output inside the package. | P1 |

## htmx 4 readiness

**Partially migrated, not production-ready on htmx 4.**

What's correct:

- Header names: `HX-Source`, `HX-Request-Type`, and the `HX-Trigger-After-*` headers removed.
- Config keys: `defaultSwap`, `history`, `noSwap`, `implicitInheritance`, `mode`.
- `<hx-partial>` out-of-band rendering.
- `:inherited` in the demo.
- `htmx.registerExtension` and the new event names in the JS extensions.

What's broken:

- The JS extensions bypass or corrupt htmx 4 core processing (RIZZY-003, -004, -005).
- History restoration (RIZZY-006).
- Trigger and swap modifier syntax (RIZZY-007, RIZZY-013).
- `HX-Location` option shapes (RIZZY-014).
- There is no strategy for htmx 4's default of swapping 4xx/5xx responses (RIZZY-019).

The official upgrade checker reports **0 issues**. That is expected, because it scans static markup and cannot see markup that C# builders or JS extensions generate at runtime. Details: [03-htmx4-compatibility.md](03-htmx4-compatibility.md).

**Recommended strategy: adopt htmx 4 as the sole supported version for the next Rizzy major (6.0).** Point htmx 2 users to the 5.3.x line. Dual support is not recommended: Rizzy's JS extensions are inherently version-specific, and its C# config surface differs between versions.

## Security assessment

- **One confirmed exploitable XSS sink** in a public helper (RIZZY-002).
- **CSP design concerns** (RIZZY-027): the document nonce is exposed in a `<meta>` attribute, defeating browser nonce-hiding. One streaming code path bypasses the sanitizer. Enabling `GenerateScriptNonce` makes htmx add a valid nonce to every swapped script.
- **Healthy**:
  - Config JSON is HTML-escaped in the meta tag (verified).
  - Nonces use 32 bytes from a CSPRNG.
  - Antiforgery tokens are sent via header for all unsafe methods, including `DELETE`, which htmx 4 no longer sends a body for.
  - `fetch` mode defaults to `same-origin`, which limits token leakage.
- No URL validation helpers exist for redirect headers (application-dependent, RIZZY-031).

## Testing confidence

**Low.**

- 55 .NET tests pass in 0.9 s and 3 JS tests pass.
- Coverage: `Rizzy.Htmx` is about 60% (line), the `Rizzy` core about **1.4%**.
- There are no pipeline or integration tests, no layout, history or streaming tests, and no tests against a real htmx 4 runtime.
- CI does not run any of these tests (RIZZY-010, RIZZY-025).

## Release readiness

**Not ready.**

- The published package graph is broken (RIZZY-001).
- Packages ship stale htmx 2-era bundles, a stray `package.json`/`package-lock.json`, and publish artifacts as content in consumer projects (RIZZY-021).
- The source generator isn't shipped (RIZZY-022).
- The MSBuild client-asset targets never run (RIZZY-023).
- The documentation is stale in several places: wrong npm package name, `type="module"` on a UMD bundle, components that don't exist, htmx `4.0.0-beta2` pins (RIZZY-026).

## Top five priorities

1. **Repair the release graph.** Version Rizzy and Rizzy.Htmx in lockstep, deprecate/unlist Rizzy 5.4.0, and publish a corrected pair through a CI pipeline that tests and validates package contents (RM-A1, RM-A2, RM-A6).
2. **Fix the XSS** in `SerializeAsAlpineData` and the **validation-attribute cache** bug (RM-A3, RM-A4).
3. **Make the JS extensions cooperate with htmx 4 core.** Streaming should be opt-in and use `ctx.extensionPromise`. The nonce handling should not re-parse responses. Confirm should fall back to `window.confirm` (RM-B1–B3).
4. **Fix layout selection** using `HX-Request-Type` and `HX-History-Restore-Request` so back/forward and boosted navigation work (RM-B4).
5. **Build a regression net**: ASP.NET pipeline tests, plus JS tests that load the real htmx 4.0.1 runtime, wired into PR CI (RM-A7).

## Recommended next development decision

**Approve a "6.0 stabilization" release line.** Freeze feature work, then execute roadmap Phases A and B as a single breaking release:

- lockstep versions,
- corrected htmx 4 behavior,
- test-gated CI,
- an htmx 2 → 4 migration guide.

Meanwhile, deprecate Rizzy 5.4.0 on nuget.org and point users to 5.3.x (htmx 2) until 6.0 ships. This decision is the owner's to make. Deprecating packages is an outward-facing action that this audit did not take.

## Report index

- [01 — Repository and architecture](01-repository-and-architecture.md)
- [02 — Code quality and APIs](02-code-quality-and-apis.md)
- [03 — htmx 4 compatibility](03-htmx4-compatibility.md)
- [04 — Security and performance](04-security-and-performance.md)
- [05 — Testing and release](05-testing-and-release.md)
- [06 — Prioritized roadmap](06-prioritized-roadmap.md)

## Findings index

| ID | Title | Sev | Report |
|----|-------|-----|--------|
| RIZZY-001 | Published Rizzy 5.4.0 depends on pre-htmx-4 Rizzy.Htmx 5.3.0 | P0 | 05 |
| RIZZY-002 | XSS in `SerializeAsAlpineData` | P0 | 04 |
| RIZZY-003 | `rizzy-streaming` takes over all HTML responses, bypassing htmx 4 core | P1 | 03 |
| RIZZY-004 | `rizzy-nonce` sanitizer corrupts responses | P1 | 03 |
| RIZZY-005 | `rizzy-confirm` blocks `hx-confirm` without a dialog listener | P1 | 03 |
| RIZZY-006 | Layout stripping breaks htmx 4 history restore and full-page requests | P1 | 03 |
| RIZZY-007 | Trigger builder emits htmx 2 syntax | P1 | 03 |
| RIZZY-008 | `DataAnnotationsProcessor` cache keyed by type only | P1 | 02 |
| RIZZY-009 | `HtmxResponseAttribute` unusable / throws on non-htmx requests | P1 | 02 |
| RIZZY-010 | CI/CD does not test, publishes unvalidated packages | P1 | 05 |
| RIZZY-011 | `HtmxTriggerSpecification` record + `Lazy` caching defects | P2 | 02 |
| RIZZY-012 | Culture-sensitive time formatting in builders | P2 | 02 |
| RIZZY-013 | `SwapStyleBuilder` modifier drift from htmx 4 | P2 | 03 |
| RIZZY-014 | `AjaxContext`/`LocationTarget` don't match htmx 4 ajax options | P2 | 03 |
| RIZZY-015 | `HX-Trigger` header built by string concatenation | P2 | 02 |
| RIZZY-016 | `Uri` overloads emit unescaped non-ASCII header values | P2 | 02 |
| RIZZY-017 | `HtmxRequest` target/source parsing and request-type API | P2 | 02 |
| RIZZY-018 | `Vary: HX-Request` applied inconsistently | P2 | 04 |
| RIZZY-019 | No strategy for htmx 4 swapping 4xx/5xx responses | P2 | 03 |
| RIZZY-020 | Reflection over ASP.NET Core internals | P2 | 02 |
| RIZZY-021 | Package contents: stale bundles, stray content files, publish artifacts | P2 | 05 |
| RIZZY-022 | Source generator not shipped; non-incremental pipeline | P2 | 05 |
| RIZZY-023 | MSBuild client-asset targets never execute | P2 | 05 |
| RIZZY-024 | Dependency hygiene (htmx pin, unused deps, dev advisories, Node 18) | P2 | 05 |
| RIZZY-025 | Test coverage and regression gaps | P2 | 05 |
| RIZZY-026 | Onboarding defaults and stale documentation | P2 | 02 / 05 |
| RIZZY-027 | CSP nonce design concerns | P2 | 04 |
| RIZZY-028 | `HtmxConfig` / API drift from htmx 4 | P2 | 03 |
| RIZZY-029 | `HtmxSwapService` raw-markup API and selector escaping | P3 | 02 |
| RIZZY-030 | Duplication and small API inconsistencies | P3 | 02 |
| RIZZY-031 | No URL validation for redirect helpers | P3 | 04 |
| RIZZY-032 | Minor measured per-request overheads | P3 | 04 |
| RIZZY-033 | Repository hygiene | P3 | 05 |
