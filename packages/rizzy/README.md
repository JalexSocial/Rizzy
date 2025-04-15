# Rizzy Client

[![npm version](https://img.shields.io/npm/v/rizzy.svg)](https://www.npmjs.com/package/rizzy)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Client-side JavaScript components and utilities for the [Rizzy Framework](https://github.com/jalexsocial/rizzy), designed to seamlessly integrate HTMX with Blazor Server-Side Rendering (SSR).

## Features

*   **HTMX Extension Integration:** Includes necessary extensions for Rizzy features:
    *   `rizzy-nonce`: Securely handles Content Security Policy (CSP) nonces for dynamically loaded scripts and styles within HTMX responses. Requires corresponding server-side setup (e.g., `HtmxConfigHeadOutlet` and `IRizzyNonceProvider`).
    *   `rizzy-streaming`: Enables HTMX to correctly process Blazor's streaming rendering updates within swapped content.
*   **Antiforgery Token Handling:** Automatically includes ASP.NET Core antiforgery tokens in HTMX POST/PUT/DELETE/PATCH requests based on configuration provided by the server (via `HtmxConfigHeadOutlet` and `HtmxAntiforgeryScript`).
*   **Client-Side Validation Integration:** Sets up the `aspnet-client-validation` library for immediate use with Rizzy's form components (like `RzInputText`, `RzValidationMessage`).

## Installation

```bash
npm install rizzy 
```

## Usage

This package is intended to be used alongside the server-side Rizzy components. It typically requires minimal client-side configuration as it relies on data and configuration provided by Rizzy's .NET components rendered into the HTML.

1.  **Include the script:** Add the bundled script to your main layout file (e.g., `_Layout.cshtml`, `App.razor`, `index.html`). Make sure to include it *after* HTMX.

    ```html
    <!-- Make sure HTMX is loaded first -->
    <script src="https://unpkg.com/htmx.org@latest"></script>

    <!-- Include the Rizzy client script -->
    <script src="/_content/Rizzy/js/rizzy.js" type="module"></script>
    ```

2.  **Ensure Rizzy Server Components:** Your Blazor application should include the necessary Rizzy components, primarily:
    *   `<HtmxConfigHeadOutlet />`: Renders HTMX configuration, including nonce and antiforgery settings, into a `<meta>` tag.
    *   `<HtmxAntiforgeryScript />`: (If using `AntiforgeryStrategy.GenerateTokensPerPage`) Provides the initial antiforgery token.
    *   Rizzy Form Components (e.g., `<RzInputText>`, `<RzValidationMessage>`): These work with the validation setup included in this package.

3.  **HTMX Extension Activation:**
    *   The HTMX extensions (`rizzy-nonce`, `rizzy-streaming`) are defined when the script loads. You typically activate them on relevant elements using the `hx-ext` attribute:
        ```html
        <div hx-get="/some/content" hx-ext="rizzy-streaming, rizzy-nonce">
          <!-- Content that might use streaming rendering or have scripts/styles -->
        </div>
        ```
    *   Refer to the main Rizzy documentation for specific usage scenarios.

4.  **Antiforgery & Validation:** These features generally work automatically once the script is loaded, provided the server-side components (`HtmxConfigHeadOutlet`, `HtmxAntiforgeryScript`, and Rizzy Form Components) are correctly configured and rendered.

## Dependencies

*   **HTMX:** This library requires HTMX (version 1.9+ recommended) to be loaded on the page *before* this script.
*   **aspnet-client-validation:** This library is used for client-side validation integration. The necessary setup (`ValidationService().bootstrap()`) is included in this package's bundle.

## Browser Support

Targets modern browsers supporting ES Modules.

## Contributing

Contributions are welcome! Please refer to the main [Rizzy repository's contributing guidelines](https://github.com/jalexsocial/rizzy/blob/main/CONTRIBUTING.md).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
