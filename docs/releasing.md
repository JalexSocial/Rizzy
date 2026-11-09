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
6. **Tag** the merge commit and push the tag:
   ```bash
   git tag -a vX.Y.Z -m "Rizzy X.Y.Z" && git push origin vX.Y.Z
   ```
   Versions with a pre-release suffix (`6.0.0-preview.1`) are published to npm under the `next`
   tag and marked as pre-releases on GitHub.
7. **Check** the GitHub release (its notes are generated from merged PRs; paste the changelog
   section in as well), nuget.org and npm.

## One-time setup (maintainers)

- A `release` environment in the GitHub repository with the `NUGET_API_KEY` and `NPM_TOKEN`
  secrets.
- Deprecate the broken Rizzy 5.4.0 on nuget.org (RIZZY-001).
