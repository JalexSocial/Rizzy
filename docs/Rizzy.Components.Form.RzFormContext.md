#### [Rizzy](index.md 'index')
### [Rizzy.Components.Form](Rizzy.Components.Form.md 'Rizzy.Components.Form')

## RzFormContext Class

Represents a form context with a strongly typed model. This record encapsulates form identification, naming, and state management through an EditContext.

```csharp
public sealed class RzFormContext :
System.IEquatable<Rizzy.Components.Form.RzFormContext>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RzFormContext

Implements [System.IEquatable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')[RzFormContext](Rizzy.Components.Form.RzFormContext.md 'Rizzy.Components.Form.RzFormContext')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')

| Constructors | |
| :--- | :--- |
| [RzFormContext(string, object)](Rizzy.Components.Form.RzFormContext.RzFormContext(string,object).md 'Rizzy.Components.Form.RzFormContext.RzFormContext(string, object)') | Initializes a new instance of the RzFormContext class with the specified form name and model, generating an identifier automatically. |
| [RzFormContext(string, string, object)](Rizzy.Components.Form.RzFormContext.RzFormContext(string,string,object).md 'Rizzy.Components.Form.RzFormContext.RzFormContext(string, string, object)') | Initializes a new instance of the RzFormContext class with the specified form name and model, generating an identifier automatically. |
| [RzFormContext(string, string, string, EditContext)](Rizzy.Components.Form.RzFormContext.RzFormContext(string,string,string,Microsoft.AspNetCore.Components.Forms.EditContext).md 'Rizzy.Components.Form.RzFormContext.RzFormContext(string, string, string, Microsoft.AspNetCore.Components.Forms.EditContext)') | Initializes a new instance of the RzFormContext class with the specified identifier, form name, and existing EditContext.<br/>This constructor allows for reusing an existing EditContext with a new form context. |
| [RzFormContext(string, string, string, object)](Rizzy.Components.Form.RzFormContext.RzFormContext(string,string,string,object).md 'Rizzy.Components.Form.RzFormContext.RzFormContext(string, string, string, object)') | Initializes a new instance of the RzFormContext class with the specified identifier, form name, and model. |

| Properties | |
| :--- | :--- |
| [EditContext](Rizzy.Components.Form.RzFormContext.EditContext.md 'Rizzy.Components.Form.RzFormContext.EditContext') | Gets the EditContext associated with the form. |
| [FormName](Rizzy.Components.Form.RzFormContext.FormName.md 'Rizzy.Components.Form.RzFormContext.FormName') | Gets the name of the form. |
| [FormUrl](Rizzy.Components.Form.RzFormContext.FormUrl.md 'Rizzy.Components.Form.RzFormContext.FormUrl') | Gets the action url for the form. |
| [Id](Rizzy.Components.Form.RzFormContext.Id.md 'Rizzy.Components.Form.RzFormContext.Id') | Gets or sets the unique identifier for the form context. |

| Methods | |
| :--- | :--- |
| [Model&lt;TModel&gt;()](Rizzy.Components.Form.RzFormContext.Model_TModel_().md 'Rizzy.Components.Form.RzFormContext.Model<TModel>()') | Retrieves the current data strongly-typed data model for EditContext. Note that EditContext must be set with a strongly-typed<br/>model for the form model to be accessible.  This is essentially a shortcut for EditContext.Model which does the typecasting for<br/>you. |
