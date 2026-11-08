# 05 — Testing, Dependencies, Release and Documentation

| Field | Value |
|---|---|
| Audit | Rizzy initial audit, 2026-11-08 |
| Repository / commit | https://github.com/JalexSocial/Rizzy @ `7e3f0f1126ee412d9db4f37d13d0370159edac15` |
| Scope | Build and test baseline, coverage, regression gaps, dependencies, CI/CD, packaging, versioning, documentation |
| Methodology / limitations | See [00](00-executive-summary.md#methodology). Packages were packed to a scratch directory. Published packages were downloaded from nuget.org and npm for comparison |

## 1. Build baseline

| Item | Result |
|---|---|
| Environment | Windows 11 Pro 10.0.26200. .NET SDK **10.0.403** (multiple SDKs installed. No `global.json`, so the newest SDK is used). Node **26.13.0**, npm 9.7.1 |
| Command | `dotnet build src/Rizzy.sln -c Release -v:n` |
| Result | **Succeeded**, 28.8 s, **0 errors, 5 warnings** (3 unique) |
| Warnings | `NU1902` AngleSharp 1.4.0 moderate advisory (test project, transitive via bUnit). `RS2008` ×2: analyzer release tracking not enabled for RZ0001/RZ0002 (source generator). `BL0008`: `Information.Person` has `[SupplyParameterFromForm]` and an initializer (demo) |
| Client assets | The MSBuild targets `EnsureClientAssets`/`LLM` **did not run** (no npm output), see RIZZY-023 |
| Working tree after build | Unchanged (only the untracked `.serena/`) |
| Pack | `dotnet pack -c Release --no-build` for both libraries **succeeded** (contents analysis in §6) |

## 2. Test baseline

| Suite | Command | Result | Duration |
|---|---|---|---|
| .NET | `dotnet test src/Rizzy.sln -c Release --no-build --collect:"XPlat Code Coverage"` | **55 passed**, 0 failed, 0 skipped | 0.9 s (9.8 s wall) |
| JS (in place) | `npm test` in `packages/rizzy` | **Fails to start**: `ERR_MODULE_NOT_FOUND` jsdom. Environmental: local `node_modules` is stale (htmx 2.0.4 installed vs lockfile 4.0.0-beta4, jsdom missing) | – |
| JS (clean copy) | `npm ci --ignore-scripts && npm test` | **3 passed** | 1.1 s |
| JS build | `npm run build && npm run validate:htmx4` | **Succeeded**. `validate-htmx4 passed`. Output is byte-identical to the committed `dist/` and `wwwroot/js/rizzy*.js` apart from CRLF/LF (`core.autocrlf=true`) | – |

No failures existed before the audit. The JS in-place failure is an environment issue, not a code defect.

### Coverage (Coverlet, line / branch)

| Package | Line | Branch | Notes |
|---|---|---|---|
| Overall | 35.4% | 14.5% | Inflated by STJ-generated code |
| `Rizzy.Htmx` | 60.3% | 37.9% | `HtmxRequest` 91%, `HtmxResponse` 73%, `SwapStyleBuilder` 87%, `HtmxTriggerSpecification` 39%, `TriggerModifierBuilder` 17%, `ServiceCollectionExtensions`/`HtmxResponseAttribute`/`HtmxRequestAttribute` **0%** |
| `Rizzy` | **1.4%** | 0.6% | Only `HtmxConfigHeadOutlet` (55%) and `RizzyConfig` are touched. `RizzyService`, `RzView`, layouts, swap service, all form components, `DataAnnotationsProcessor`, middleware and nonce provider are at **0%** |

**Coverage versus quality:** the existing tests are reasonable unit tests of header strings. However, several tests **lock in incorrect output**. For example, `SwapStyleBuilderTests` assert the `focus-scroll` key, and the trigger tests never exercise `from:`/`target:` quoting. The defects found in this audit (RIZZY-007/008/009/011/012/013/015/016/017) all sit in either uncovered code or assertions that encode htmx 2 syntax.

## 3. Missing regression coverage — RIZZY-025

| | |
|---|---|
| Category / Severity | Testing / **P2** |
| Evidence | The coverage table above. There are no `WebApplicationFactory`/`TestServer`/Alba pipeline tests, even though `Alba` 8.4.0 is referenced. JS tests use a hand-mocked `htmx` object and never load the real runtime. There is no browser test. CI runs none of these tests (RIZZY-010). |
| Impact | The htmx 4 migration shipped with three always-on client defects and a broken history flow, and nothing caught them. |

**Proposed tests (priority order):**

1. **Pipeline tests** (xUnit + `WebApplicationFactory<Program>` on a minimal test host, or Alba):
   - `View<T>` with no htmx headers renders the full layout and includes `<meta name="htmx-config">`.
   - `HX-Request` + `HX-Request-Type: partial` renders the fragment with no layout.
   - `HX-Request-Type: full` and `HX-History-Restore-Request: true` render the full layout (RIZZY-006).
   - `Vary` contains `HX-Request` on both variants (RIZZY-018).
   - `EmptyBody()`/`Redirect()` returns 204 plus the header.
   - `IHtmxSwapService` output: `<hx-partial>` only for htmx requests.
   - Antiforgery: POST without the header returns 400, with the header from the meta config returns 200.
   - `[HtmxResponse]` on a non-htmx request returns 200 (RIZZY-009).
   - A Blazor form POST through `RzController` binds `[SupplyParameterFromForm]` (guards RIZZY-020 across framework updates).
2. **Unit tests**:
   - `DataAnnotationsProcessor` multi-field model (RIZZY-008).
   - Builder outputs for every modifier, including quoting, culture `de-DE`, and `with`-copies (RIZZY-007/011/012/013).
   - `HX-Trigger` escaping and merge (RIZZY-015).
   - `Uri` overloads (RIZZY-016).
   - `TargetId` without an id (RIZZY-017).
   - `SerializeAsAlpineData` hostile input (RIZZY-002).
   - Config meta JSON snapshot (RIZZY-028).
3. **JS integration tests** that load `htmx.org@4.0.1` in jsdom or happy-dom, or better, Playwright against the demo:
   - table-row swap survives (RIZZY-004);
   - `hx-status:422` is honored and `htmx:response:error` fires (RIZZY-003);
   - `HX-Trigger` fires exactly once, after the swap (RIZZY-003);
   - `hx-confirm` without a listener falls back to `window.confirm` (RIZZY-005);
   - back/forward restores the layout (RIZZY-006);
   - streaming `<blazor-ssr>` blocks patch markers.
4. **API contract**: `PublicApiAnalyzers` or package validation with a baseline (see [02](02-code-quality-and-apis.md#backward-compatibility-analysis)).
5. **Package content test**: a CI step that lists the `.nupkg` and fails on unexpected entries (RIZZY-021) or a missing analyzer (RIZZY-022).
6. **Concurrency and performance**: not needed now. Revisit after Phase B (RIZZY-032).

## 4. Dependency assessment — RIZZY-024

| | |
|---|---|
| Category / Severity | Dependencies / **P2** |

| Dependency | Finding | Recommendation |
|---|---|---|
| `htmx.org` (npm) | Declared `^4.0.0-beta4` as a **dependency**, but it is never bundled. Lockfile pins the beta. The demo and READMEs load **4.0.0-beta2** from jsDelivr **without SRI** | Make it a `peerDependency` `^4.0.1`. Pin docs and demo to 4.0.1 with `integrity`, or serve a local copy. Add a CI test against the pinned version |
| `aspnet-client-validation` (npm) | Runtime dependency that is **not imported** since validation was removed from the bundle. The README still claims it is bundled | Remove, or restore as an optional entry point |
| `vite` 6.4.3, `postcss`, `nanoid`, `source-map-js` (npm dev) | 4 **high** advisories, all build-time only. `npm audit --omit=dev` reports 0 | `npm audit fix` (vite 6.4.5+). Low risk. Not shipped |
| `jsdom` 26 → 31, `rollup-plugin-visualizer` 6 → 7, `vite` 6 → 8 | Outdated majors | Upgrade opportunistically. No functional need |
| Node 18 in CI | End of life since April 2025 | Use Node 22 LTS (or 24) |
| `AngleSharp` 1.4.0 (NuGet, transitive via bUnit 2.3.4) | Moderate advisory, test-only | Update bUnit, or pin AngleSharp ≥ the patched version in the test project |
| `Alba` 8.4.0, `System.IdentityModel.Tokens.Jwt` 8.15.0 (tests) | Unused | Remove, or use Alba for the pipeline tests |
| `DefaultDocumentation` 1.2.2 | Effectively unused (`GenerateDocumentationFile=false`) | Remove, or enable XML docs (recommended: enable XML docs for IntelliSense) |
| `FluentAssertions` 7.0.0 | v7 remains Apache-2.0 (v8+ needs a commercial license) | Keep it pinned below 8, or move to Shouldly/xUnit asserts. Note this in CONTRIBUTING |
| `Sqids` 3.2.0 (runtime) | MIT, current | Fine. Could be replaced by a trivial counter encoder to drop the dependency (optional) |
| `Microsoft.CodeAnalysis.CSharp` 5.0.0 (generator) | Requires a Roslyn 5 host (VS 2026 / .NET 10 SDK). Fine for a net10.0-only library | Document the minimum SDK. Add `global.json` |
| Target framework | **net10.0 only.** Package dependency floors (`Microsoft.AspNetCore.App`) are implicit | Acceptable (.NET 10 is LTS). State it in the README |
| Licensing | All direct dependencies are MIT/Apache-2.0. Repository is MIT | Fine |

No dependency conflicts were detected. Upgrades should be driven by RIZZY-024's concrete items, not by a blanket "update everything".

## 5. CI/CD assessment — RIZZY-010

| | |
|---|---|
| Category / Severity | Release engineering / **P1** |
| Subsystem | `.github/workflows/dotnet.yml` |
| Evidence | (1) Triggers only on `push` to `main` (line 5). PR validation was removed in commit `8e88aaa`. (2) The "Test Rizzy" step runs `dotnet test --no-build` **in `src/Rizzy`**, a non-test project, so **zero tests run** (reproduced locally: no output, exit 0). JS tests and `validate:htmx4` are not run. (3) Every push to `main` packs and pushes to NuGet and npm, with no tag or manual gate. `--skip-duplicate` (lines 67, 75) silently skipped the htmx 4 build of Rizzy.Htmx 5.3.0, which **caused RIZZY-001**. (4) `dotnet publish -o ./publish/net10.0` (lines 50, 54) writes **inside the project directory**. The Razor SDK's default globs then pack those files as `content/` (confirmed in the published Rizzy 5.4.0: `content/publish/net10.0/Rizzy.deps.json`, …). (5) Node 18 (line 23). (6) npm version is copied from `Rizzy.csproj` at publish time, but the Rizzy.Htmx version is independent. (7) No package content validation, no SBOM, no provenance (`npm publish --provenance`), no GitHub release or tag. |
| Impact | Untested code reaches nuget.org and npm on every merge. Version mistakes are silent. |
| Recommendation | Split into **`ci.yml`** (PRs and pushes: restore, build, `dotnet test test/Rizzy.Tests`, `npm ci && npm test && npm run build:check`, a git-diff check that `wwwroot` matches the fresh build, pack and content check) and **`release.yml`** (on `v*` tags or manual dispatch: lockstep version from the tag, pack, validate, push **without** `--skip-duplicate` so collisions fail loudly, then npm publish `--provenance`, then a GitHub Release with notes). Drop `dotnet publish` from CI. Use Node 22. |
| Status / Confidence | Confirmed (static + local reproduction + published artifact) / High |

## 6. Packaging and release

### RIZZY-001 — Published Rizzy 5.4.0 depends on a pre-htmx-4 Rizzy.Htmx

| | |
|---|---|
| Category / Severity | Release integrity / **P0** |
| Subsystem | NuGet packages, versioning, CI |
| Evidence | nuget.org: **Rizzy 5.4.0** (published 2026-06-09) declares `Rizzy.Htmx >= 5.3.0`. The only **Rizzy.Htmx 5.3.0** was published **2025-12-19**, six months before the htmx 4 migration. `src/Rizzy.Htmx/Rizzy.Htmx.csproj` still says `<Version>5.3.0</Version>` (line 30), so CI's push of the migrated build was skipped as a duplicate. Binary comparison: published Rizzy.Htmx 5.3.0 **lacks** `HtmxAttributeName`, `HtmxHistoryMode`, `HtmxConfig.Prefix`/`Mode`/`NoSwap`, `HX-Source`, `HX-Request-Type`, `hx-partial`, and **contains** htmx 2 members (`historyEnabled`, `defaultSwapStyle`, `TriggerAfterSwap`, `HX-Trigger-Name`). The published Rizzy 5.4.0 `Rizzy.dll` **references** `HtmxAttributeName` and `get_Prefix`. |
| Impact | Every NuGet consumer of Rizzy 5.4.0 resolves the old Rizzy.Htmx. Expect `TypeLoadException`/`MissingMethodException` when `HtmxSwappable` renders. The `htmx-config` meta is serialized with htmx 2 keys against an htmx 4 client. The released htmx 4 support is effectively unusable from NuGet. |
| Recommendation | (Owner actions.) Deprecate Rizzy 5.4.0 on nuget.org with a message pointing to 5.3.x (htmx 2) or the upcoming fixed release. Release the corrected pair in **lockstep** (same version, exact dependency range `[x.y.z]`) through the new release workflow (RIZZY-010). Add a release check that every packed dependency version exists on the feed with matching content, or simply always bump both. |
| Status / Confidence | Confirmed (static: nuget.org metadata + binary inspection). The runtime failure was not executed / High |

### RIZZY-021 — Package contents: stale bundles, stray content files, publish artifacts

| | |
|---|---|
| Category / Severity | Packaging / **P2** |
| Evidence (local pack and published 5.4.0) | (1) `staticwebassets/js/rizzy.es.js`, `.es.min.js`, `.es.mjs`, `.es.min.mjs` + maps (about 330 KB) are htmx 2/webpack-era bundles from March 2026. They are still in `src/Rizzy/wwwroot/js` because the Vite copy step never deletes old files, and they are served at `_content/Rizzy/js/rizzy.es.*`. (2) `content/package.json`, `content/package-lock.json` and `contentFiles/any/net10.0/…` (`buildAction="Content"`) are the repomix tooling manifest from `src/Rizzy/`, injected into every consumer project. (3) Published 5.4.0 also ships `content/publish/net10.0/*` (from RIZZY-010 §4). |
| Impact | Consumers get confusing obsolete scripts (anyone referencing `rizzy.es.mjs` from older docs gets htmx 2 code) and junk files in their projects. |
| Recommendation | Delete the stale bundles. Make the Vite copy clean its destination. Exclude `package*.json`, `llmquery.txt` and `publish/**` from `Content`/`None` packing (`<Content Remove=…/>`). Add a CI content check. |
| Status / Confidence | Confirmed (pack listing) / High |

### RIZZY-022 — Source generator not shipped; non-incremental pipeline

| | |
|---|---|
| Category / Severity | Packaging, developer experience / **P2** |
| Evidence | `Rizzy.SourceGenerators.csproj` has `IsPackable=false` (and contradictory `GeneratePackageOnBuild=true`). `Rizzy.csproj` sets `IncludeAnalyzerInPackage` (line 76), which is not an MSBuild or NuGet property that does anything. The packed `Rizzy.5.4.0.nupkg` contains **no `analyzers/` folder**. Consumers therefore can't use `[RizzyParameterize]` on their own components. The attribute is generated as `internal` per assembly. The pipeline `Combine`s with `CompilationProvider` and `Collect()`s all classes, so it re-runs on every keystroke. `RS2008` warnings show release tracking isn't set up. |
| Recommendation | Decide whether this is a supported feature. If yes: pack the generator DLL into `analyzers/dotnet/cs/` of the Rizzy package (`<None Include="…Rizzy.SourceGenerators.dll" Pack="true" PackagePath="analyzers/dotnet/cs" />`), use `ForAttributeWithMetadataName`, add `AnalyzerReleases.*.md`, and add generator tests. If no: remove it from docs and the demo. |
| Status / Confidence | Confirmed (pack listing + static) / High |

### RIZZY-023 — MSBuild client-asset targets never execute

| | |
|---|---|
| Category / Severity | Build / **P2** |
| Evidence | `Rizzy.csproj` line 80: `BuildDependsOn` is extended only when `'$(TargetFramework)' == ''`. With a single `<TargetFramework>`, that property is always set, so `EnsureClientAssets` and `LLM` never run (no npm output in the baseline build). `wwwroot/js` is therefore whatever was last committed by hand. A contributor who edits `packages/rizzy/src/js/*` and runs `dotnet build` ships stale JS. |
| Recommendation | Either remove the targets and document `npm run build` (plus the CI drift check, RIZZY-010), or wire them correctly (`BeforeTargets="AssignTargetPaths"` with Inputs/Outputs, skipped when `RizzySkipClientBuild=true`). Prefer the former, which keeps the .NET build free of Node. |
| Status / Confidence | Confirmed (runtime: build log) / High |

### Versioning and release notes

- Rizzy is at 5.4.0 and Rizzy.Htmx at 5.3.0. The two are coupled through `InternalsVisibleTo`, so versioning them independently is unsafe (RIZZY-001). **Adopt lockstep versioning.**
- Breaking htmx 4 changes shipped as a minor version, which violates SemVer. **Ship the corrected release as 6.0.0.**
- There is no `CHANGELOG.md` and no GitHub releases. The last tag is `v5.2.0`. **Add a Keep-a-Changelog file and tag every release.** Optional: let conventional commits (already used) drive release notes.
- Symbols: `DebugType=embedded` plus `DotNet.ReproducibleBuilds` (SourceLink) is good. `.snupkg` is not needed with embedded PDBs.
- Package metadata: titles, description, license file, icon, readme and repository commit are present. The nuspec includes the deprecated `licenseUrl` (auto-added). Fine. Rizzy's package README is the marketing-oriented root README with no install steps.

### When can a locally successful build produce a bad package?

1. Editing JS without running `npm run build` (RIZZY-023).
2. Bumping one package version but not the other (RIZZY-001).
3. A local `publish/` folder or tool manifests inside `src/Rizzy` get packed as content (RIZZY-021).
4. Stale files left in `wwwroot/js` by the non-cleaning Vite copy (RIZZY-021).
5. `--skip-duplicate` turning a forgotten version bump into a silent no-op (RIZZY-010).
6. The analyzer silently missing from the package (RIZZY-022).

## 7. Documentation quality

### RIZZY-026 — Onboarding and documentation accuracy (docs aspects)

| | |
|---|---|
| Category / Severity | Documentation / **P2** (code aspects in [02](02-code-quality-and-apis.md#rizzy-026--onboarding-defaults-and-setup-pitfalls-code-aspects)) |

| Document | Problem |
|---|---|
| Root `README.md` (also the Rizzy NuGet readme) | Marketing only. No install, `AddRizzy`/`AddHtmx`/`UseRizzy` setup, root layout with `HeadOutlet`, or script tags. Links to an external site where e.g. `/docs/getting-started/` returns **404** |
| `packages/rizzy/README.md` | `npm install rizzy` is the **wrong package** (should be `@jalexsocial/rizzy`). `<script type="module">` is wrong for a UMD bundle. Refers to `<HtmxAntiforgeryScript />`, which **does not exist**. Claims `aspnet-client-validation` is bundled and bootstrapped (removed). Says to "activate" extensions, but they auto-register. Pins htmx `4.0.0-beta2` |
| `src/Rizzy.Htmx/README.md` | Pins htmx `4.0.0-beta2`. Mentions "caching" config that doesn't exist. Example returns `Results.Ok("Update succeeded.")` as an htmx response (plain text swap). Otherwise reasonably accurate |
| `CONTRIBUTING.md` | Says to target PRs at the **`dev`** branch, but CI and recent merges use `main`. Doesn't explain how to build JS assets or run tests |
| XML docs | Extensive `///` comments, but no XML file is generated, so there is no IntelliSense for consumers |
| Missing high-value docs | Quick start. htmx 2 → 4 migration guide for Rizzy users (see [03 §7](03-htmx4-compatibility.md#7-expected-impact-on-consumers)). Compatibility table (Rizzy ↔ htmx ↔ .NET). Layout and history model (`HX-Request-Type`, `hx-history-elt`). Error and validation response patterns. CSP/nonce guide. Antiforgery setup for MVC vs minimal APIs. Description of the client extensions and how to disable them. Troubleshooting (no meta config, 400 antiforgery, layout missing on back) |

**Can a developer build a working app from the published instructions alone?** No. The READMEs never mention the root component with `<HeadOutlet/>`, and they give the wrong npm package name and script tag. Combined with RIZZY-001, a NuGet-based setup fails at runtime.

## 8. Repository hygiene — RIZZY-033

| | |
|---|---|
| Category / Severity | Maintainability / **P3** |
| Evidence | `src/Rizzy/llmquery.txt` (3,183-line repomix dump), committed and regenerated by a dead target. `packages/rizzy/stats.html` (4,949-line bundle analyzer output), committed. `Rizzy.sln` has a configuration block for the non-existent project `{483780E3-…}`. No `global.json`. No `.editorconfig`. `.serena/` is not in `.gitignore`. Release tags stop at `v5.2.0`. |
| Recommendation | Remove generated artifacts and ignore them. Clean the solution file. Add `global.json` (SDK 10.0.x, `rollForward: latestFeature`) and `.editorconfig`. Tag releases. |
| Status / Confidence | Confirmed / High |

## 9. Healthy areas

- Clean Release build with only 5 warnings and nullable enabled.
- The JS build is reproducible and matches committed artifacts.
- Embedded PDBs, SourceLink, reproducible builds, and package validation (once a baseline is configured).
- Fast, deterministic unit tests (under 1 s) that are a good foundation to extend.
- The `validate-htmx4.js` guard script is a good idea and should run in CI.
