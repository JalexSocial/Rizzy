#### [Rizzy](index 'index')
### [Rizzy.Components](Rizzy.Components 'Rizzy.Components').[DataAnnotationsProcessor](Rizzy.Components.DataAnnotationsProcessor 'Rizzy.Components.DataAnnotationsProcessor')

## DataAnnotationsProcessor.AddAttributeHandler<TAttribute>(Action<TAttribute,IDictionary<string,object>,string>) Method

Adds a custom handler for a specific type of [System.ComponentModel.DataAnnotations.ValidationAttribute](https://docs.microsoft.com/en-us/dotnet/api/System.ComponentModel.DataAnnotations.ValidationAttribute 'System.ComponentModel.DataAnnotations.ValidationAttribute').

```csharp
public void AddAttributeHandler<TAttribute>(System.Action<TAttribute,System.Collections.Generic.IDictionary<string,object>,string> handler)
    where TAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute;
```
#### Type parameters

<a name='Rizzy.Components.DataAnnotationsProcessor.AddAttributeHandler_TAttribute_(System.Action_TAttribute,System.Collections.Generic.IDictionary_string,object_,string_).TAttribute'></a>

`TAttribute`

The type of the attribute to handle.
#### Parameters

<a name='Rizzy.Components.DataAnnotationsProcessor.AddAttributeHandler_TAttribute_(System.Action_TAttribute,System.Collections.Generic.IDictionary_string,object_,string_).handler'></a>

`handler` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')[TAttribute](Rizzy.Components.DataAnnotationsProcessor.AddAttributeHandler_TAttribute_(System.Action_TAttribute,System.Collections.Generic.IDictionary_string,object_,string_)#Rizzy.Components.DataAnnotationsProcessor.AddAttributeHandler_TAttribute_(System.Action_TAttribute,System.Collections.Generic.IDictionary_string,object_,string_).TAttribute 'Rizzy.Components.DataAnnotationsProcessor.AddAttributeHandler<TAttribute>(System.Action<TAttribute,System.Collections.Generic.IDictionary<string,object>,string>).TAttribute')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')[System.Collections.Generic.IDictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IDictionary-2 'System.Collections.Generic.IDictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IDictionary-2 'System.Collections.Generic.IDictionary`2')[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IDictionary-2 'System.Collections.Generic.IDictionary`2')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')

The handler that processes the attribute.