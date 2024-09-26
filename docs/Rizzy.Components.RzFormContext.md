#### [Rizzy](index 'index')
### [Rizzy.Components](Rizzy.Components 'Rizzy.Components')

## RzFormContext Class

Represents a form context with a strongly typed model. This record encapsulates form identification, naming, and state management through an EditContext.

```csharp
public sealed class RzFormContext :
System.IEquatable<Rizzy.Components.RzFormContext>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RzFormContext

Implements [System.IEquatable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')[RzFormContext](Rizzy.Components.RzFormContext 'Rizzy.Components.RzFormContext')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')

| Constructors | |
| :--- | :--- |
| [RzFormContext(string, object)](Rizzy.Components.RzFormContext.RzFormContext(string,object) 'Rizzy.Components.RzFormContext.RzFormContext(string, object)') | Initializes a new instance of the RzFormContext class with the specified form name and model, generating an identifier automatically. |
| [RzFormContext(string, string, object)](Rizzy.Components.RzFormContext.RzFormContext(string,string,object) 'Rizzy.Components.RzFormContext.RzFormContext(string, string, object)') | Initializes a new instance of the RzFormContext class with the specified form name and model, generating an identifier automatically. |
| [RzFormContext(string, string, string, EditContext)](Rizzy.Components.RzFormContext.RzFormContext(string,string,string,Microsoft.AspNetCore.Components.Forms.EditContext) 'Rizzy.Components.RzFormContext.RzFormContext(string, string, string, Microsoft.AspNetCore.Components.Forms.EditContext)') | Initializes a new instance of the RzFormContext class with the specified identifier, form name, and existing EditContext.<br/>This constructor allows for reusing an existing EditContext with a new form context. |
| [RzFormContext(string, string, string, object)](Rizzy.Components.RzFormContext.RzFormContext(string,string,string,object) 'Rizzy.Components.RzFormContext.RzFormContext(string, string, string, object)') | Initializes a new instance of the RzFormContext class with the specified identifier, form name, and model. |

| Properties | |
| :--- | :--- |
| [EditContext](Rizzy.Components.RzFormContext.EditContext 'Rizzy.Components.RzFormContext.EditContext') | Gets the EditContext associated with the form. |
| [FormName](Rizzy.Components.RzFormContext.FormName 'Rizzy.Components.RzFormContext.FormName') | Gets the name of the form. |
| [FormUrl](Rizzy.Components.RzFormContext.FormUrl 'Rizzy.Components.RzFormContext.FormUrl') | Gets the action url for the form. |
| [Id](Rizzy.Components.RzFormContext.Id 'Rizzy.Components.RzFormContext.Id') | Gets or sets the unique identifier for the form context. |

| Methods | |
| :--- | :--- |
| [Model&lt;TModel&gt;()](Rizzy.Components.RzFormContext.Model_TModel_() 'Rizzy.Components.RzFormContext.Model<TModel>()') | Retrieves the current data strongly-typed data model for EditContext. Note that EditContext must be set with a strongly-typed<br/>model for the form model to be accessible.  This is essentially a shortcut for EditContext.Model which does the typecasting for<br/>you. |
