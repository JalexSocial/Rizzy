#### [Rizzy](index 'index')
### [Rizzy.Attributes](Rizzy.Attributes 'Rizzy.Attributes').[HtmxResponseAttribute](Rizzy.Attributes.HtmxResponseAttribute 'Rizzy.Attributes.HtmxResponseAttribute')

## HtmxResponseAttribute.OnResultExecuting(ResultExecutingContext) Method

Called before the action result is executed. This method applies the HTMX configurations  
specified by the attribute properties to the response headers.

```csharp
public void OnResultExecuting(Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext context);
```
#### Parameters

<a name='Rizzy.Attributes.HtmxResponseAttribute.OnResultExecuting(Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext).context'></a>

`context` [Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext 'Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext')

The context in which the result is being executed.

Implements [OnResultExecuting(ResultExecutingContext)](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.IResultFilter.OnResultExecuting#Microsoft_AspNetCore_Mvc_Filters_IResultFilter_OnResultExecuting_Microsoft_AspNetCore_Mvc_Filters_ResultExecutingContext_ 'Microsoft.AspNetCore.Mvc.Filters.IResultFilter.OnResultExecuting(Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext)')

#### Exceptions

[System.ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentNullException 'System.ArgumentNullException')  
Thrown if the context is null.