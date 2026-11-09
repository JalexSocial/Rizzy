# Browser tests

Playwright tests that drive `src/RizzyDemo` in Chromium (roadmap RM-A7, finding RIZZY-025). They
cover what the jsdom tests in `packages/rizzy` cannot: native confirm dialogs, Content Security
Policy enforcement, real back/forward navigation and streaming over a real HTTP connection.

| Spec | Covers |
|---|---|
| `layout-and-history` | config meta tag before htmx (RIZZY-036), boosted navigation, back/forward with `hx-history-elt`, reload, antiforgery POST |
| `streaming` | `[StreamRendering]` over boosted navigation and direct loads, stream events |
| `examples` | table rows + out-of-band swap, native `hx-confirm` (dismiss and accept) + antiforgery DELETE, CSP nonce promotion vs. injected script, server errors |
| `validation` | 422 responses with field errors from attributes and controller rules (RIZZY-037), success |

Every test also fails on unexpected console errors, page errors or Rizzy configuration warnings.

## Running

```bash
cd test/e2e && npm ci && npx playwright install chromium
```

```bash
npx playwright test
```

The config starts the demo on http://localhost:5088 (`dotnet run -c Release`), or reuses one that
is already running there. Set `RIZZY_E2E_PORT` to use another port. CI runs this suite on every pull
request (the `e2e` job in `.github/workflows/ci.yml`).
