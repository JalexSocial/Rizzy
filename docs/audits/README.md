# Rizzy Technical Audits

This directory holds dated, point-in-time technical audits of the Rizzy repository. An audit gives a baseline built from evidence: architecture, code quality, security, performance, htmx compatibility, testing and release readiness. It also gives a prioritized roadmap. Audits are planning documents. They do not change production code.

## Completed audits

| Date | Audit | Reviewed commit | Summary |
|------|-------|-----------------|---------|
| 2026-11-08 | [Rizzy initial audit](2026-11-08-rizzy-initial-audit/00-executive-summary.md) ([errata](2026-11-08-rizzy-initial-audit/ERRATA.md)) | `7e3f0f1` (main) | First comprehensive baseline and htmx 4.0.1 compatibility assessment |

## Directory and naming conventions

- Each audit lives in its own directory: `docs/audits/YYYY-MM-DD-descriptive-audit-name/`
  - The date is the day the audit was performed, not the day it was merged.
  - Use lowercase kebab-case for the name, e.g. `2027-02-01-htmx4-regression-audit`.
- A full audit uses these files:

  | File | Purpose |
  |------|---------|
  | `00-executive-summary.md` | Decisions and impact for the project owner |
  | `01-repository-and-architecture.md` | Inventory, structure, request lifecycle, architecture |
  | `02-code-quality-and-apis.md` | Defects, maintainability, public API, ASP.NET Core integration |
  | `03-htmx4-compatibility.md` | htmx compatibility matrix and strategy (or the relevant client-library topic) |
  | `04-security-and-performance.md` | Security findings, performance evidence |
  | `05-testing-and-release.md` | Build/test baseline, CI/CD, packaging, dependencies, docs |
  | `06-prioritized-roadmap.md` | Actionable tasks grouped into phases |

  A focused audit, such as a security-only review, may omit reports that don't apply. It must say which ones it omits and why in its executive summary.

## Required report metadata

Every report starts with a short metadata block:

```markdown
| Field | Value |
|---|---|
| Audit | <name> |
| Date | YYYY-MM-DD |
| Repository | <url> |
| Branch / commit | <branch> @ <full SHA> |
| Scope | <what this report covers> |
| Methodology | <how evidence was gathered>, or a cross-reference to the summary |
| Limitations | <what could not be verified> |
```

Put the full methodology and limitations in `00-executive-summary.md`. Other reports can cross-reference it instead of repeating it.

## Finding IDs

- Each substantive finding gets a unique ID: `RIZZY-NNN` (zero-padded, e.g. `RIZZY-007`).
- IDs are **never reused**. A later audit that re-confirms an earlier finding must reference the original ID, e.g. "RIZZY-007 (from 2026-11-08) still open". New findings continue numbering from the highest ID used in any previous audit or errata file (currently RIZZY-034).
- Use the same ID everywhere the finding is mentioned: reports, roadmap tasks, commits, PRs and issues.
- Roadmap tasks use their own IDs, `RM-<phase><n>` (e.g. `RM-A1`). These are scoped to the audit that created them. Prefix them with the audit date when cross-referencing from elsewhere.

Each finding records: title, category, severity, affected subsystem, evidence (repo-relative path and line), impact, recommendation, verification status, and confidence.

## Severity definitions

| Severity | Meaning |
|----------|---------|
| **P0 – Critical** | Confirmed security vulnerability, data-loss risk, or fundamental failure (e.g. a broken published release) needing immediate attention. |
| **P1 – High** | Significant functional defect, compatibility failure, or architectural problem affecting typical users. |
| **P2 – Medium** | Important maintainability, testing, performance, or developer-experience issue. |
| **P3 – Low** | Minor improvement, documentation correction, or optional cleanup. |

## Verification status vocabulary

| Status | Meaning |
|--------|---------|
| **Confirmed (runtime)** | Reproduced by executing code (test, script, or harness). |
| **Confirmed (static)** | Proven by reading source or binaries, with a deterministic code path. Not executed end-to-end. |
| **Reproducible incompatibility** | Shown against the official upstream documentation or source of a dependency. |
| **Architectural concern** | A design issue with credible impact, not a single defect. |
| **Needs investigation** | Plausible risk that needs more evidence before acting. |
| **Optional improvement** | Beneficial but not required. |

Confidence is **High**, **Medium**, or **Low**.

## Preserving historical reports

- Don't edit a completed audit's findings after it's merged. Corrections go in a short `ERRATA.md` inside that audit's directory, or in the next audit.
- Fixing a finding does not change the audit. Track resolution in issues/PRs that reference the finding ID, and in the next audit's status table.
- Don't delete or move audit directories. Links from issues and PRs depend on stable paths.

## Recommended follow-up audits

1. **Post-stabilization release audit**: after roadmap Phase A. Verify the published NuGet and npm artifacts match the repository and resolve correctly. Re-check RIZZY-001, RIZZY-010 and RIZZY-021.
2. **htmx 4 regression audit**: after Phase B. Re-run the compatibility matrix against the current htmx 4.x release, using browser-level tests.
3. **Security review**: after the nonce and streaming rework (RIZZY-003, RIZZY-004, RIZZY-027). Focus on CSP, XSS and antiforgery.
4. **Recurring light audit**: once per minor release, or every 6 months. Cover dependency advisories, documentation accuracy and coverage trends.
