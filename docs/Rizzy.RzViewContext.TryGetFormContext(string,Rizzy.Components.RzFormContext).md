#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[RzViewContext](Rizzy.RzViewContext 'Rizzy.RzViewContext')

## RzViewContext.TryGetFormContext(string, RzFormContext) Method

Attempts to get a form context by name.

```csharp
public bool TryGetFormContext(string formName, out Rizzy.Components.RzFormContext context);
```
#### Parameters

<a name='Rizzy.RzViewContext.TryGetFormContext(string,Rizzy.Components.RzFormContext).formName'></a>

`formName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The name of the form.

<a name='Rizzy.RzViewContext.TryGetFormContext(string,Rizzy.Components.RzFormContext).context'></a>

`context` [RzFormContext](Rizzy.Components.RzFormContext 'Rizzy.Components.RzFormContext')

The form context, if found.

#### Returns
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
True if the form context was found; otherwise, false.