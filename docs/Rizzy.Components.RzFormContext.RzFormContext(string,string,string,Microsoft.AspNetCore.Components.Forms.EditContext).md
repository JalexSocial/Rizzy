#### [Rizzy](index 'index')
### [Rizzy.Components](Rizzy.Components 'Rizzy.Components').[RzFormContext](Rizzy.Components.RzFormContext 'Rizzy.Components.RzFormContext')

## RzFormContext(string, string, string, EditContext) Constructor

Initializes a new instance of the RzFormContext class with the specified identifier, form name, and existing EditContext.  
This constructor allows for reusing an existing EditContext with a new form context.

```csharp
public RzFormContext(string id, string formName, string formUrl, Microsoft.AspNetCore.Components.Forms.EditContext context);
```
#### Parameters

<a name='Rizzy.Components.RzFormContext.RzFormContext(string,string,string,Microsoft.AspNetCore.Components.Forms.EditContext).id'></a>

`id` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The unique identifier for the form context.

<a name='Rizzy.Components.RzFormContext.RzFormContext(string,string,string,Microsoft.AspNetCore.Components.Forms.EditContext).formName'></a>

`formName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The name of the form.

<a name='Rizzy.Components.RzFormContext.RzFormContext(string,string,string,Microsoft.AspNetCore.Components.Forms.EditContext).formUrl'></a>

`formUrl` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Callback url for the form

<a name='Rizzy.Components.RzFormContext.RzFormContext(string,string,string,Microsoft.AspNetCore.Components.Forms.EditContext).context'></a>

`context` [Microsoft.AspNetCore.Components.Forms.EditContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.EditContext 'Microsoft.AspNetCore.Components.Forms.EditContext')

The EditContext to be associated with the form.