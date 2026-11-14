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

## When a release fails partway

The workflow publishes NuGet first, then npm, then the GitHub release.

- **Failed before NuGet** (a check, the build or the tests): fix the cause and re-run the failed
  run. Nothing was published. A re-run uses the tag's copy of `release.yml`, so a fix to the
  workflow itself needs a new tag instead.
- **NuGet published, npm or the GitHub release failed:** don't re-run; the NuGet push would fail on
  the duplicate version. Fix the cause, then run the workflow manually from `main` (Actions →
  Release → Run workflow) with the version and **npm only** ticked:
  ```bash
  gh workflow run release.yml --ref main -f version=X.Y.Z -f npm_only=true
  ```
  It checks out the `vX.Y.Z` tag, skips NuGet, and publishes npm and the GitHub release. If npm
  fails with `ENEEDAUTH`, its trusted publishing exchange failed: the step's verbose log says why
  (search for `oidc`), and the usual cause is a field in the npm trusted publisher that doesn't
  exactly match (`JalexSocial`, `Rizzy`, `release.yml`, `release`).

## One-time setup (maintainers)

- **Environment.** Create a `release` environment (Settings → Environments) with yourself as a
  required reviewer, so a pushed tag waits for your approval before anything is published. Both
  trusted publishing policies (NuGet and npm) name this environment, so only approved runs can
  publish.
- **NuGet (trusted publishing).** On nuget.org, open your username menu → *Trusted Publishing* and
  add a policy: owner = the account that owns `Rizzy` and `Rizzy.Htmx`, repository owner
  `JalexSocial`, repository `Rizzy`, workflow file `release.yml`, environment `release`. On the
  `release` environment, add a **variable** (not a secret) `NUGET_USER` set to that account's
  nuget.org profile name (not its email). The workflow's `NuGet/login` step exchanges the run's
  OIDC token for an API key that lasts about an hour, so no NuGet key is stored. After the first
  successful publish, delete any old `NUGET_API_KEY` secret and revoke that key on nuget.org.
- **npm (trusted publishing).** On npmjs.com, open `@jalexsocial/rizzy` → Settings → Trusted
  Publisher → GitHub Actions: organization `JalexSocial`, repository `Rizzy`, workflow
  `release.yml`, environment `release` (all case-sensitive). No npm token is stored. npm asks for a
  publish through a new trusted publisher by a deadline (shown on the settings page) to validate
  it; if that passes, add it again before releasing.
- **After the first 6.x release:** deprecate Rizzy 5.4.0 on nuget.org (it depends on a pre-htmx-4
  Rizzy.Htmx, RIZZY-001) and point users to 6.0.
