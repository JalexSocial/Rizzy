# Contributing to Rizzy

Thank you for considering contributing to Rizzy! Rizzy makes it painless to integrate Razor components with MVC for UI development along with HTMX. This unique combination allows developers to build dynamic, interactive user interfaces while ensuring a seamless user experience.

## Getting Started

Before you dive in, we encourage you to participate in our [Discussions](https://github.com/JalexSocial/Rizzy/discussions) page. It's a great place to ask questions, share ideas, or get to know the Rizzy community.

### Reporting Issues

Found a bug or have a feature suggestion? Please check our [Issues](https://github.com/JalexSocial/Rizzy/issues) page to see if it has already been reported. If not, feel free to open a new issue. When creating an issue, provide as much relevant information as possible to help us understand the problem or feature request.

### Suggested Workflow for Contributions

We recommend the following workflow for contributions to Rizzy:

1. **Issue Creation:**
   - For significant changes, create an issue first. Describe the proposed change and get feedback from the community.
   - For minor changes or bug fixes, you can skip this step.
   - Use existing issues when possible. If you plan to tackle an issue, comment that you're working on it and request assignment.

2. **Fork and Branch:**
   - Fork the Rizzy repository and create a branch from `main` for your work (`git checkout -b mybranch main`). `main` is the only long-lived branch; every change reaches it through a pull request.

3. **Developing:**
   - Implement your changes, adhering to the Rizzy coding standards and guidelines.
   - Write or update tests for your changes.
   - Ensure your code follows the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) style for commit messages.

4. **Testing:** run the same checks as CI (see [Building and testing](#building-and-testing) below).

5. **Submitting a Pull Request (PR):**
   - Push your changes to your fork and open a PR against **`main`**. CI must pass before it is merged.
   - In the PR description, reference the issue it addresses.
   - Await feedback or approval from the project maintainers.

6. **After Your PR is Merged:**
   - Once approved and merged, you may delete your branch.
   - Congratulations! You've contributed to Rizzy. 🎉

## Building and testing

You need the .NET 10 SDK (`global.json` pins the 10.0 feature band) and Node.js 22. These are the steps CI runs, from the repository root:

```bash
dotnet build src/Rizzy.sln -c Release
```

```bash
dotnet test test/Rizzy.Tests/Rizzy.Tests.csproj -c Release
```

The client library lives in `packages/rizzy`. Its build writes the bundle to `packages/rizzy/dist` **and** to `src/Rizzy/wwwroot/js`, which ship in the Rizzy NuGet package. Both are committed, and CI fails if they differ from a fresh build, so rebuild and commit them whenever you change `packages/rizzy/src`:

```bash
cd packages/rizzy && npm ci && npm test && npm run build:check
```

To check the NuGet packages the way a consumer sees them (pack, inspect the contents, then build a fresh app against them):

```bash
dotnet pack src/Rizzy.Htmx/Rizzy.Htmx.csproj -c Release -o ./artifacts/nupkgs
```

```bash
dotnet pack src/Rizzy/Rizzy.csproj -c Release -o ./artifacts/nupkgs
```

```bash
python eng/check-package-contents.py ./artifacts/nupkgs && bash eng/consumer-smoke-test.sh ./artifacts/nupkgs 6.0.0-dev
```

Other things worth knowing:

- **Public API changes** fail `PublicApiBaselineTests`. If the change is intended, regenerate the baselines with `RIZZY_UPDATE_API_BASELINE=1 dotnet test test/Rizzy.Tests/Rizzy.Tests.csproj` and commit `test/Rizzy.Tests/PublicApi/*.txt`, and list breaking changes in `CHANGELOG.md`.
- **Browser tests** run the demo in Chromium with Playwright: `cd test/e2e && npm ci && npx playwright install chromium && npx playwright test` (see `test/e2e/README.md`). CI runs them on every pull request.
- **The demo** (`src/RizzyDemo`) is part of the solution: `dotnet run --project src/RizzyDemo --launch-profile http`, then open http://localhost:5088/home/.
- **Benchmarks:** see `benchmarks/Rizzy.Benchmarks/README.md`.
- **Versions** are set in one place, `Directory.Build.props`. Releases are cut by the maintainers from a `vX.Y.Z` tag (see `CHANGELOG.md` and `.github/workflows/release.yml`).

### Contribution Guidelines

- **Code Style:** Follow existing coding styles in the project. This includes using meaningful variable names, commenting your code when necessary, and adhering to the project's architectural patterns.
- **Testing:** Add tests for every new feature or bug fix. Ensure that all tests pass before submitting a PR.
- **Documentation:** Update documentation to reflect any changes in functionality or usage patterns.
- **Respect:** Treat all contributors with respect. We strive to make this project welcoming to all.

### Code of Conduct

See [CODE-OF-CONDUCT.md](./CODE-OF-CONDUCT.md)

### Need Help?

If you're stuck or have questions, don't hesitate to reach out on our [Discussions](https://github.com/JalexSocial/Rizzy/discussions) page. Our community is here to help!

Thank you for contributing to Rizzy, and we look forward to your contributions!
