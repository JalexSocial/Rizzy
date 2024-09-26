#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[RzViewContext](Rizzy.RzViewContext 'Rizzy.RzViewContext')

## RzViewContext.AddFormContext(string, string, string, object, bool) Method

Attempts to add a form context with the specified name and model.

```csharp
public Rizzy.Components.RzFormContext AddFormContext(string id, string formName, string formAction, object model, bool useDataAnnotations=true);
```
#### Parameters

<a name='Rizzy.RzViewContext.AddFormContext(string,string,string,object,bool).id'></a>

`id` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

html id for the form

<a name='Rizzy.RzViewContext.AddFormContext(string,string,string,object,bool).formName'></a>

`formName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The name of the form.

<a name='Rizzy.RzViewContext.AddFormContext(string,string,string,object,bool).formAction'></a>

`formAction` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

<a name='Rizzy.RzViewContext.AddFormContext(string,string,string,object,bool).model'></a>

`model` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

The model associated with the form.

<a name='Rizzy.RzViewContext.AddFormContext(string,string,string,object,bool).useDataAnnotations'></a>

`useDataAnnotations` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

Determines whether to use data annotations for validation.

#### Returns
[RzFormContext](Rizzy.Components.RzFormContext 'Rizzy.Components.RzFormContext')  
True if the form context was added successfully; otherwise, false.