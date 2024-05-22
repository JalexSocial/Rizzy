#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[RzViewContext](Rizzy.RzViewContext.md 'Rizzy.RzViewContext')

## RzViewContext.TryGetFormContext(string, RzFormContext) Method

Attempts to get a form context by name.

```csharp
public bool TryGetFormContext(string formName, out Rizzy.Components.Form.RzFormContext context);
```
#### Parameters

<a name='Rizzy.RzViewContext.TryGetFormContext(string,Rizzy.Components.Form.RzFormContext).formName'></a>

`formName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The name of the form.

<a name='Rizzy.RzViewContext.TryGetFormContext(string,Rizzy.Components.Form.RzFormContext).context'></a>

`context` [RzFormContext](Rizzy.Components.Form.RzFormContext.md 'Rizzy.Components.Form.RzFormContext')

The form context, if found.

#### Returns
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
True if the form context was found; otherwise, false.