#### [Rizzy](index.md 'index')
### [Rizzy.Framework.Mvc](Rizzy.Framework.Mvc.md 'Rizzy.Framework.Mvc').[RzController](Rizzy.Framework.Mvc.RzController.md 'Rizzy.Framework.Mvc.RzController')

## RzController.OnActionExecutionAsync(ActionExecutingContext, ActionExecutionDelegate) Method

Called before the action method is invoked.

```csharp
public virtual System.Threading.Tasks.Task OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context, Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate next);
```
#### Parameters

<a name='Rizzy.Framework.Mvc.RzController.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate).context'></a>

`context` [Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext 'Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext')

The action executing context.

<a name='Rizzy.Framework.Mvc.RzController.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate).next'></a>

`next` [Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate 'Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate')

The [Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate 'Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate') to execute. Invoke this delegate in the body  
            of [OnActionExecutionAsync(ActionExecutingContext, ActionExecutionDelegate)](Rizzy.Framework.Mvc.RzController.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate).md 'Rizzy.Framework.Mvc.RzController.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext, Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate)') to continue execution of the action.

Implements [OnActionExecutionAsync(ActionExecutingContext, ActionExecutionDelegate)](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.IAsyncActionFilter.OnActionExecutionAsync#Microsoft_AspNetCore_Mvc_Filters_IAsyncActionFilter_OnActionExecutionAsync_Microsoft_AspNetCore_Mvc_Filters_ActionExecutingContext,Microsoft_AspNetCore_Mvc_Filters_ActionExecutionDelegate_ 'Microsoft.AspNetCore.Mvc.Filters.IAsyncActionFilter.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate)')

#### Returns
[System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task')  
A [System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task') instance.