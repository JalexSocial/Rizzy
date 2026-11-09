# @jalexsocial/rizzy

[![npm version](https://img.shields.io/npm/v/@jalexsocial/rizzy.svg)](https://www.npmjs.com/package/@jalexsocial/rizzy)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

The client script for [Rizzy](https://github.com/jalexsocial/rizzy): htmx 4 extensions that make
server-rendered Razor components work with htmx. The same script ships in the Rizzy NuGet package
as `_content/Rizzy/js/rizzy.js`, so most apps don't need this npm package at all.

## What it does

| Extension | Purpose |
|---|---|
| `rizzy-antiforgery` | Adds the ASP.NET Core antiforgery token (from the page's `<meta name="htmx-config">`) to every non-GET htmx request, and refreshes it after boosted navigation. |
| `rizzy-nonce` | Lets server-rendered `<script>`/`<style>` in htmx responses run under a nonce-based Content Security Policy, while injected ones stay blocked. Does nothing on pages without a nonce. |
| `rizzy-streaming` | Makes Blazor streaming rendering (`[StreamRendering]`) work with htmx: the initial HTML is swapped normally and `<blazor-ssr>` updates are applied as they arrive. |
| `rizzy-confirm` | `hx-confirm` uses the browser's `confirm()`, or a custom dialog that handles the `rz:confirm-dialog` event (`window.Rizzy.confirm`). |
| diagnostics | One console warning when the page is missing Rizzy's config meta tag or the server runs another Rizzy major version; extension errors are logged the way htmx logs its own. |

## Usage

Load htmx 4 first, then Rizzy. The bundle is a classic (UMD) script, so don't add `type="module"`.
In a Rizzy app, the root layout's `<head>` looks like this:

```html
<RzHeadOutlet />  <!-- Blazor's HeadOutlet + Rizzy's htmx-config meta tag; must come before htmx -->
<script src="https://cdn.jsdelivr.net/npm/htmx.org@4.0.1/dist/htmx.min.js"
        integrity="sha384-Qp9rdC8H7EqOoX+nR0P3RPG2Noji+qdwJlGpfzwQJsfOpp3sGwIdSP9xO3+UlEhn"
        crossorigin="anonymous"></script>
<script src="_content/Rizzy/js/rizzy.js"></script>
```

From npm instead of the NuGet static asset:

```bash
npm install @jalexsocial/rizzy htmx.org@^4.0.1
```

Then serve `node_modules/@jalexsocial/rizzy/dist/js/rizzy.min.js` (or import `@jalexsocial/rizzy`
in your bundler after htmx). Use the same version as your Rizzy NuGet packages; the diagnostics warn
when the major versions differ.

The extensions register themselves when the script loads; no `hx-ext` attributes are needed. If you
restrict extensions with `htmx.config.extensions` (`HtmxConfig.Extensions`), include
`rizzy-nonce, rizzy-streaming, rizzy-antiforgery, rizzy-confirm`.

## Browser support

Modern evergreen browsers (the bundle targets ES2020), as required by htmx 4.

## Contributing

See the main repository's [CONTRIBUTING.md](https://github.com/jalexsocial/rizzy/blob/main/CONTRIBUTING.md).
The source is in `packages/rizzy/src/js`; `npm test` runs the tests against the real htmx runtime.

## License

MIT. See [LICENSE.md](LICENSE.md).
