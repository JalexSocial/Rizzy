<img src="https://jalexsocial.github.io/Rizzy/media/rizzy-logo.png?cache=bust" width="600"/>

# Rizzy MVC: Seamlessly blend the power of Razor Components with Asp.net and HTMX

Rizzy is a lightweight library that enhances Asp.net MVC applications by seamlessly integrating Razor components for UI development and working harmoniously with HTMX for progressive enhancement. With Rizzy, you can leverage the power of Razor components to create dynamic and interactive user interfaces while ensuring a smooth user experience through HTMX.

## Motivation

### Razor Components vs. View Components

Traditional Asp.net MVC development often involves the use of View Components to create modular and reusable UI elements. While View Components serve their purpose, Razor Components offer several advantages:

1. **Code Reusability**: Razor Components promote better code reuse by encapsulating both the UI and the code logic into a single component. This leads to cleaner and more maintainable code.

2. **Separation of Concerns**: Razor Components follow a component-based architecture, separating concerns between UI and logic. This improves code organization and makes it easier to manage complex applications.

3. **Consistent Syntax**: Razor Components use the same syntax as Razor views, making it easier for developers familiar with Asp.net MVC to transition and maintain a consistent coding style.

4. **Strongly Typed**: Razor Components benefit from the strongly typed nature of C#, providing compile-time checking and improved developer productivity.

### Why Rizzy?

Rizzy combines the strengths of Razor Components with the seamless integration of HTMX, offering a powerful solution for Asp.net MVC developers looking to enhance their applications with modern UI development practices.

## Features

- **Razor Component Integration**: Easily use Razor components within your Asp.net MVC views, enhancing the modularity and maintainability of your UI code.

- **HTMX Compatibility**: Rizzy works effortlessly with HTMX, enabling progressive enhancement for your MVC applications. Make your web pages dynamic and interactive without compromising on performance.

- **Simplified Syntax**: Rizzy simplifies the integration process, allowing developers to seamlessly blend Razor components and HTMX features into their existing MVC projects.

- **Efficient Development**: Leverage the benefits of Razor Components for efficient and structured UI development, resulting in more maintainable and scalable applications.

## Getting Started

To start using Rizzy in your Asp.net MVC project, follow these simple steps:

1. Install the Rizzy NuGet package: `Install-Package Rizzy`

2. Update your `Startup.cs` file to include Rizzy configuration:

   ```csharp
   // Add this line to ConfigureServices method
   services.AddRizzy();
   ```

3. Coming soon

4. Enjoy the benefits of Razor Components and HTMX in your Asp.net MVC application!

## Contributions

Contributions are welcome! If you encounter any issues, have feature requests, or would like to contribute to Rizzy, please [open an issue](https://github.com/jalexsocial/rizzy/issues) or submit a pull request.

## License

Rizzy is released under the [MIT License](https://opensource.org/licenses/MIT).
