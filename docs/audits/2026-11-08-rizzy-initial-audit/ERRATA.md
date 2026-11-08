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
