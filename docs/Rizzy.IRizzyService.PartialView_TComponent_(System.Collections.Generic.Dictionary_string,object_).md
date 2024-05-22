#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[IRizzyService](Rizzy.IRizzyService.md 'Rizzy.IRizzyService')

## IRizzyService.PartialView<TComponent>(Dictionary<string,object>) Method

Renders a Razor component without a layout

```csharp
Microsoft.AspNetCore.Http.IResult PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object?> data)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.IRizzyService.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_).TComponent'></a>

`TComponent`
#### Parameters

<a name='Rizzy.IRizzyService.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_).data'></a>

`data` [System.Collections.Generic.Dictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')

#### Returns
[Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult')