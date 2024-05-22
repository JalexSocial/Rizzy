#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[IRizzyService](Rizzy.IRizzyService 'Rizzy.IRizzyService')

## IRizzyService.View<TComponent>(Dictionary<string,object>) Method

Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary.

```csharp
Microsoft.AspNetCore.Http.IResult View<TComponent>(System.Collections.Generic.Dictionary<string,object?> data)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.IRizzyService.View_TComponent_(System.Collections.Generic.Dictionary_string,object_).TComponent'></a>

`TComponent`

The type of the Razor component to render.
#### Parameters

<a name='Rizzy.IRizzyService.View_TComponent_(System.Collections.Generic.Dictionary_string,object_).data'></a>

`data` [System.Collections.Generic.Dictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')

A dictionary containing the data to pass to the component.

#### Returns
[Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult')  
An [Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult') that can render the specified component as a view.