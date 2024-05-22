#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[RizzyService](Rizzy.RizzyService.md 'Rizzy.RizzyService')

## RizzyService.PartialView<TComponent>(Dictionary<string,object>) Method

Renders a partial view using the specified Razor component with explicitly provided data in the form of a dictionary.  
This method is intended for rendering components without a layout, suitable for inclusion in other views.

```csharp
public Microsoft.AspNetCore.Http.IResult PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object?> data)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.RizzyService.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_).TComponent'></a>

`TComponent`

The type of the Razor component to render.
#### Parameters

<a name='Rizzy.RizzyService.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_).data'></a>

`data` [System.Collections.Generic.Dictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')

A dictionary containing the data to pass to the component.

Implements [PartialView&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.IRizzyService.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_).md 'Rizzy.IRizzyService.PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object>)')

#### Returns
[Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult')  
An [Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult') that can render the specified component as a partial view.