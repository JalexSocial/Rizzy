# Rizzy.Htmx

[![Documentation](https://img.shields.io/badge/docs-Rizzy.Htmx-blue)](https://jalexsocial.github.io/rizzy.docs/docs/htmx/overview/)

**Rizzy.Htmx** is a lightweight .NET library designed to integrate [htmx](https://htmx.org/) (version 2.0 or greater) functionality into your Rizzy-powered web applications. It centralizes all htmx-specific logic in one place, making it easier to manage dynamic client interactions, HTML swapping, and related response operations from your server-side code.

## Features

- **Seamless htmx Integration:** Easily integrate htmx features into your application with centralized configuration and helper methods.
- **Dynamic Response Operations:** Use the `Response.Htmx` action method to execute multiple operations on your response. For example:

  ```csharp
  HttpContext.Response.Htmx(response => {
      response.PushUrl("/new-url")
              .Reswap("outerHTML");
  });
  ```
- **Fluent Builder APIs:** Construct and customize triggers, swap styles, and other htmx behaviors using fluent builder classes.
- **Server-Side Configuration:** Leverage source-generated JSON contexts for efficient serialization and deserialization of htmx configuration data.
- **Integrated with Rizzy:** Works hand in hand with other Rizzy services and components to provide a cohesive experience.

## Installation

Install the library via NuGet:

```bash
dotnet add package Rizzy.Htmx
```

## Getting Started

### 1. Configuration

In your application’s startup (e.g., in `Program.cs`), add Rizzy.Htmx to the service collection and configure it as needed:

```csharp
using Rizzy.Htmx;
using Rizzy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHtmx(options =>
{
    // Enable browser history, define default swap delays, and more:
    options.HistoryEnabled = true;
    options.RefreshOnHistoryMiss = true;
    options.DefaultSwapDelay = TimeSpan.FromMilliseconds(250);
    // Additional configuration properties...
});
```

### 2. Using HTMX in Your Controllers

In your controllers, leverage Rizzy.Htmx to build your responses when handling htmx requests:

```csharp
using Rizzy.Htmx;
using Rizzy.Framework.Mvc;

public class HomeController : RzController
{
    public IResult Index()
    {
        // Return a view with htmx enhancements:
        return View<MyComponent>(new { message = "Hello HTMX!" });
    }

    public IResult Update()
    {
        // Set multiple htmx response operations using the Response.Htmx action method:
        HttpContext.Response.Htmx(response =>
        {
            response.PushUrl("/new-url")
                    .Reswap("outerHTML");
        });

        return Results.Ok("Update succeeded.");
    }
}
```

### 3. Client-Side Integration

Ensure you are using **htmx version 2.0 or greater**. In your HTML pages, include the htmx client script via a CDN or serve it locally:

```html
<script src="https://unpkg.com/htmx.org@2.0.0"></script>
```

When you use Rizzy.Htmx alongside other Rizzy components, required assets may be automatically loaded.

## API Overview

The Rizzy.Htmx library provides several key features:

- **Configuration Objects:** Define htmx behavior via objects such as `HtmxConfig` (including properties for history, caching, swap delays, etc.).
- **Request and Response Helpers:** Use `HtmxRequest` and `HtmxResponse` to access or manipulate htmx-specific headers, such as push URL, redirection, and content swapping.
- **Fluent Builders:** Create and manipulate htmx triggers and swap styles using builder classes (e.g., `TriggerBuilder`, `SwapStyleBuilder`).
- **Response.Htmx Action Method:** Easily compose multiple response actions, for example:
  
  ```csharp
  HttpContext.Response.Htmx(response =>
  {
      response.PushUrl("/new-url")
              .Reswap("outerHTML");
  });
  ```

For complete API documentation, please refer to the [Rizzy.Htmx documentation](https://jalexsocial.github.io/rizzy.docs/docs/htmx/overview/).

## Contributing

Contributions to Rizzy.Htmx are welcome. If you find bugs, have feature requests, or wish to contribute to the project, please create an issue or submit a pull request on the [GitHub repository](https://github.com/Jalexsocial/rizzy).

## License

This project is licensed under the [MIT License](LICENSE).

## Acknowledgements

- [htmx](https://htmx.org/) for a robust client-side interaction framework.
- The Rizzy community for their feedback and contributions.

---

This README provides an overview of the **Rizzy.Htmx** library along with installation, configuration, and usage examples. Feel free to modify or extend it based on the needs of your project.