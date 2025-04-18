<img src="https://jalexsocial.github.io/Rizzy/media/rizzy-logo.png?cache=bust3" width="600"/>

# Rizzy: Seamlessly blend Razor Components with Asp.net MVC and HTMX

Rizzy is a lightweight library that enhances Asp.net MVC applications by seamlessly integrating Razor components for UI development and working with HTMX for progressive enhancement. With Rizzy, you can use composable Razor components to create dynamic and interactive user interfaces while ensuring a smooth user experience through HTMX.

## Documentation Site

For detailed documentation and examples, please visit the [Rizzy Documentation Site](https://jalexsocial.github.io/rizzy.docs/).

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

Contributions are welcome! If you encounter any issues, have feature requests, or would like to contribute to Rizzy, please [open an issue](https://github.com/jalexsocial/rizzy/issues) or submit a pull request.

### Contributors

- Michael Tanczos
- Egil Hansen
- Khalid Abuhakmeh
- Ryan Elian
- .NET Foundation

## Referenced Libraries

1. Asp.net Client Validation - https://github.com/haacked/aspnet-client-validation


## License

Rizzy is released under the [MIT License](https://opensource.org/licenses/MIT).
