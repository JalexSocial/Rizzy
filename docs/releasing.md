# Releasing Rizzy

Rizzy, Rizzy.Htmx and `@jalexsocial/rizzy` are released together with one version number
(RIZZY-001). `.github/workflows/release.yml` publishes all three from a `vX.Y.Z` tag. It refuses
to reuse a version, and it runs the full test, pack and smoke-test pipeline before publishing.
See `CHANGELOG.md` for the SemVer policy.

## Checklist

1. **Changelog.** Rename `## [Unreleased]` in `CHANGELOG.md` to `## [X.Y.Z] - YYYY-MM-DD`, add a new
   empty `## [Unreleased]` above it, and update the compare links at the bottom. Every breaking
   change must be listed with its migration note (and finding ID, where there is one).
2. **Migration guide.** For a major version, check that `docs/migration/` covers every breaking
   change in the changelog.
3. **Version.** `Directory.Build.props` holds the development version. Bump it to the next
   version after a release. For a new major version, also update `CLIENT_MAJOR` in
   `packages/rizzy/src/js/rizzy-diagnostics.js` (a test fails until they match) and rebuild the bundle.
4. **htmx version.** If htmx was upgraded, check that `npm run generate:htmx-assets` was run and the drift tests pass (see CONTRIBUTING).
5. **Merge** the release PR to `main` and wait for CI to pass.
6. **Tag** the merge commit and push the tag (approve the run in the `release` environment):
   ```bash
   git tag -a vX.Y.Z -m "Rizzy X.Y.Z" && git push origin vX.Y.Z
   ```
   Versions with a pre-release suffix (`6.0.0-preview.1`) are published to npm under the `next`
   tag and marked as pre-releases on GitHub.
7. **Check** the published packages:
   - the GitHub release, whose notes are the changelog section followed by the merged PRs;
   - nuget.org: create a fresh app and `dotnet add package Rizzy --version X.Y.Z` (it can take a
     few minutes to be listed);
   - npm: `npm view @jalexsocial/rizzy dist-tags` (`next` for pre-releases, `latest` otherwise).
8. **Bump** `Directory.Build.props` to the next development version (for example
   `6.0.0-preview.2-dev`).

The workflow refuses to publish when `CHANGELOG.md` has no section for the version
(`eng/changelog-section.sh`), before anything is pushed.

## One-time setup (maintainers)

- **Environment.** Create a `release` environment (Settings → Environments) with yourself as a
  required reviewer, so a pushed tag waits for your approval before anything is published. Move the
  publishing secrets from the repository into it.
- **NuGet.** `NUGET_API_KEY` must be unexpired and allowed to push `Rizzy` and `Rizzy.Htmx`.
- **npm**, one of:
  - *Trusted publishing (recommended):* on npmjs.com, add a trusted publisher for
    `@jalexsocial/rizzy` (GitHub Actions, repository `JalexSocial/Rizzy`, workflow `release.yml`,
    environment `release`) and remove the `NPM_TOKEN` secret. No long-lived token is stored.
  - *Token:* an `NPM_TOKEN` secret with publish rights to `@jalexsocial/rizzy`. Check its expiry.
- **After the first 6.x release:** deprecate Rizzy 5.4.0 on nuget.org (it depends on a pre-htmx-4
  Rizzy.Htmx, RIZZY-001) and point users to 6.0.
