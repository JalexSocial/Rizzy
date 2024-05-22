#### [Rizzy](index.md 'index')
### [Rizzy.Attributes](Rizzy.Attributes.md 'Rizzy.Attributes')

## HtmxResponseAttribute Class

An attribute to apply HTMX response headers to an action result. This enables server-side control  
of client-side HTMX behaviors such as redirection, refreshing, and element updates.

```csharp
public sealed class HtmxResponseAttribute : System.Attribute,
Microsoft.AspNetCore.Mvc.Filters.IResultFilter,
Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; HtmxResponseAttribute

Implements [Microsoft.AspNetCore.Mvc.Filters.IResultFilter](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.IResultFilter 'Microsoft.AspNetCore.Mvc.Filters.IResultFilter'), [Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata 'Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata')

| Properties | |
| :--- | :--- |
| [Location](Rizzy.Attributes.HtmxResponseAttribute.Location.md 'Rizzy.Attributes.HtmxResponseAttribute.Location') | Specifies a client-side redirect to a new location without a full page reload. |
| [PreventBrowserCurrentUrlUpdate](Rizzy.Attributes.HtmxResponseAttribute.PreventBrowserCurrentUrlUpdate.md 'Rizzy.Attributes.HtmxResponseAttribute.PreventBrowserCurrentUrlUpdate') | Prevents the browser's current URL from being updated. |
| [PreventBrowserHistoryUpdate](Rizzy.Attributes.HtmxResponseAttribute.PreventBrowserHistoryUpdate.md 'Rizzy.Attributes.HtmxResponseAttribute.PreventBrowserHistoryUpdate') | Prevents the browser's history from being updated. |
| [PushUrl](Rizzy.Attributes.HtmxResponseAttribute.PushUrl.md 'Rizzy.Attributes.HtmxResponseAttribute.PushUrl') | Pushes a new URL onto the browser's history stack. |
| [Redirect](Rizzy.Attributes.HtmxResponseAttribute.Redirect.md 'Rizzy.Attributes.HtmxResponseAttribute.Redirect') | Initiates a client-side redirect to a specified URL. |
| [Refresh](Rizzy.Attributes.HtmxResponseAttribute.Refresh.md 'Rizzy.Attributes.HtmxResponseAttribute.Refresh') | Triggers a client-side full page refresh. |
| [ReplaceUrl](Rizzy.Attributes.HtmxResponseAttribute.ReplaceUrl.md 'Rizzy.Attributes.HtmxResponseAttribute.ReplaceUrl') | Replaces the current URL in the browser's location bar. |
| [Reselect](Rizzy.Attributes.HtmxResponseAttribute.Reselect.md 'Rizzy.Attributes.HtmxResponseAttribute.Reselect') | Chooses which part of the response is used for the swap, using a CSS selector. |
| [Reswap](Rizzy.Attributes.HtmxResponseAttribute.Reswap.md 'Rizzy.Attributes.HtmxResponseAttribute.Reswap') | Specifies how the response will be swapped in the client-side, using HTMX swap strategies. |
| [Retarget](Rizzy.Attributes.HtmxResponseAttribute.Retarget.md 'Rizzy.Attributes.HtmxResponseAttribute.Retarget') | Updates the target of the content update to a different element on the page using a CSS selector. |
| [StopPolling](Rizzy.Attributes.HtmxResponseAttribute.StopPolling.md 'Rizzy.Attributes.HtmxResponseAttribute.StopPolling') | Sets return status code to stop an ongoing polling request |

| Methods | |
| :--- | :--- |
| [OnResultExecuted(ResultExecutedContext)](Rizzy.Attributes.HtmxResponseAttribute.OnResultExecuted(Microsoft.AspNetCore.Mvc.Filters.ResultExecutedContext).md 'Rizzy.Attributes.HtmxResponseAttribute.OnResultExecuted(Microsoft.AspNetCore.Mvc.Filters.ResultExecutedContext)') | Called after the action result is executed. This method is not used but is required by the interface. |
| [OnResultExecuting(ResultExecutingContext)](Rizzy.Attributes.HtmxResponseAttribute.OnResultExecuting(Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext).md 'Rizzy.Attributes.HtmxResponseAttribute.OnResultExecuting(Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext)') | Called before the action result is executed. This method applies the HTMX configurations<br/>specified by the attribute properties to the response headers. |
