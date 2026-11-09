<img src="https://jalexsocial.github.io/Rizzy/media/rizzy-logo.png?cache=bust3" width="600"/>

# Rizzy: Seamlessly blend Razor Components with Asp.net MVC and HTMX

Rizzy is a lightweight library that enhances Asp.net MVC applications by seamlessly integrating Razor components for UI development and working with HTMX for progressive enhancement. With Rizzy, you can use composable Razor components to create dynamic and interactive user interfaces while ensuring a smooth user experience through HTMX.

## Documentation Site

For detailed documentation and examples, please visit the [Rizzy Documentation Site](https://jalexsocial.github.io/rizzy.docs/).

**Upgrading from Rizzy 5 / htmx 2?** Rizzy 6 targets htmx 4.0.1+. See the [upgrade guide](docs/migration/htmx2-to-htmx4.md) for the compatibility table, required markup changes and breaking API changes.

## Quick start

Rizzy 6 requires .NET 10 and htmx 4.0.1 or later.

**1. Install the package** (it brings the matching `Rizzy.Htmx` and the client script):

```bash
dotnet add package Rizzy
```

**2. Register Rizzy** in `Program.cs`:

```csharp
using Rizzy;
using MyApp; // the namespace of your components (your project's root namespace)

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // only for MVC controllers
builder.Services.AddRizzy(options =>
{
    options.RootComponent = typeof(HtmxApp<AppLayout>);    // the HTML document
    options.DefaultLayout = typeof(HtmxLayout<MainLayout>); // page chrome, skipped for htmx fragments
});

var app = builder.Build();

app.UseStaticFiles();   // serves _content/Rizzy/js/rizzy.js
app.UseRouting();
app.UseAntiforgery();
app.UseRizzy();

app.MapGet("/", (IRizzyService rizzy) => rizzy.View<Home>());
app.MapPost("/greet", (IRizzyService rizzy) => rizzy.PartialView<Greeting>(new { Name = "htmx" }));
app.MapDefaultControllerRoute();

app.Run();
```

**3. Add `_Imports.razor`** next to your components:

```razor
@* _Imports.razor *@
@using Microsoft.AspNetCore.Components.Web
@using Rizzy
```

**4. Create the root layout.** It must render `<RzHeadOutlet />` (Blazor's `HeadOutlet` plus Rizzy's
htmx config and antiforgery token) before the htmx script:

```razor
@* AppLayout.razor *@
@inherits LayoutComponentBase

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <base href="/" />
    <RzHeadOutlet />
    <script src="https://cdn.jsdelivr.net/npm/htmx.org@4.0.1/dist/htmx.min.js"
            integrity="sha384-Qp9rdC8H7EqOoX+nR0P3RPG2Noji+qdwJlGpfzwQJsfOpp3sGwIdSP9xO3+UlEhn"
            crossorigin="anonymous"></script>
    <script src="_content/Rizzy/js/rizzy.js"></script>
</head>
<body>
    @Body
</body>
</html>
```

```razor
@* MainLayout.razor *@
@inherits LayoutComponentBase

<main>@Body</main>
```

**5. Write components** and let htmx swap them:

```razor
@* Home.razor *@
<h1>Hello</h1>
<button hx-post="/greet" hx-target="#greeting">Greet</button>
<div id="greeting"></div>
```

```razor
@* Greeting.razor *@
<p>Hello, @Name!</p>

@code {
    [Parameter] public string? Name { get; set; }
}
```

The POST carries the antiforgery token automatically, and `View<T>` renders a full page for a normal
request but only the fragment (no layouts) for an htmx request that targets an element. Open the
browser console while you work: Rizzy and htmx 4 log configuration problems and errors there.

`src/RizzyDemo` in this repository shows the rest: MVC controllers, 422 validation, out-of-band
swaps, streaming rendering, confirm dialogs and a nonce-based Content Security Policy.

## Leverage the Best of Both Worlds: Robust MVC Structure meets Modern Blazor UI Components

Remember the power and structure of ASP.NET MVC? The clear separation of concerns, the robust routing and model binding, the mature ecosystem for handling business logic, security, and data access? It's a solid foundation many .NET developers know and trust.

Now, think about building modern UIs. Razor Components (Blazor) offer a productive, component-based approach using C# for everything – logic, markup, composition. Building reusable, encapsulated UI pieces feels natural and leverages your existing .NET skills, avoiding the often-jarring context switch to heavy JavaScript frameworks.

### **So, why choose between them? Rizzy lets you combine their strengths.**

Imagine using MVC's reliable request pipeline to handle your core application logic, routing, and security, but **rendering your views using the power and reusability of Razor Components.**

### **But wait, doesn't that mean full page reloads for every interaction?** 

Not with Rizzy. By integrating HTMX seamlessly behind the scenes, Rizzy allows those server-rendered Razor Components to drive dynamic, partial page updates.

### **Here's why this combination is compelling:**

1.  **Productivity Boost:** Write your UI *and* backend logic primarily in C#. Reuse Blazor components across your application, simplifying UI development and maintenance compared to managing separate HTML templates and complex JavaScript.
2.  **Leverage Existing Skills & Ecosystem:** Continue using the familiar, powerful features of ASP.NET MVC for your application's core structure, authentication, validation, and data handling. Benefit from the vast MVC ecosystem.
3.  **Simplified Interactivity:** Achieve dynamic partial updates (like SPAs) without writing mountains of JavaScript or adopting complex client-side frameworks. Let HTMX (managed by Rizzy) handle the AJAX, while Blazor handles rendering the HTML fragments.
4.  **Maintainability:** Keep your UI logic closer to your backend logic within the .NET ecosystem. Using strongly-typed components often leads to more maintainable and refactorable code than manipulating HTML strings or managing disparate client-side scripts.
5.  **Progressive Enhancement:** Start with a solid, server-rendered foundation (great for SEO and initial load) and layer dynamic behavior on top where needed, using the same Blazor components for both initial render and HTMX updates.

**In essence, Rizzy offers a "sweet spot":** keep the robust, scalable backend architecture of MVC you already know, but build your UIs with the modern, productive component model of Blazor, and get dynamic, interactive user experiences without getting bogged down in JavaScript complexity. It's about leveraging C# and .NET end-to-end, for both structure *and* interactive UI rendering.

## Contributions

See [CONTRIBUTING.md](CONTRIBUTING.md) for how to build and test, and [CHANGELOG.md](CHANGELOG.md) for release notes. Contributions are welcome! If you encounter any issues, have feature requests, or would like to contribute to Rizzy, please [open an issue](https://github.com/jalexsocial/rizzy/issues) or submit a pull request.

### Contributors

- Michael Tanczos
- Egil Hansen
- Khalid Abuhakmeh
- Ryan Elian
- .NET Foundation

## License

Rizzy is released under the [MIT License](https://opensource.org/licenses/MIT).
