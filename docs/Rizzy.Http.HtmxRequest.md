#### [Rizzy](index 'index')
### [Rizzy.Http](Rizzy.Http 'Rizzy.Http')

## HtmxRequest Class

Htmx request headers

```csharp
public class HtmxRequest
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; HtmxRequest

| Constructors | |
| :--- | :--- |
| [HtmxRequest(HttpContext)](Rizzy.Http.HtmxRequest.HtmxRequest(Microsoft.AspNetCore.Http.HttpContext) 'Rizzy.Http.HtmxRequest.HtmxRequest(Microsoft.AspNetCore.Http.HttpContext)') | Creates a new instance of [HtmxRequest](Rizzy.Http.HtmxRequest 'Rizzy.Http.HtmxRequest'). |

| Properties | |
| :--- | :--- |
| [CurrentURL](Rizzy.Http.HtmxRequest.CurrentURL 'Rizzy.Http.HtmxRequest.CurrentURL') | Gets the current URL of the browser. |
| [IsBoosted](Rizzy.Http.HtmxRequest.IsBoosted 'Rizzy.Http.HtmxRequest.IsBoosted') | Gets whether or not the current request is an request initiated via an element using hx-boost. |
| [IsHistoryRestore](Rizzy.Http.HtmxRequest.IsHistoryRestore 'Rizzy.Http.HtmxRequest.IsHistoryRestore') | Gets whether or not the current request is an Htmx history restore request. |
| [IsHtmx](Rizzy.Http.HtmxRequest.IsHtmx 'Rizzy.Http.HtmxRequest.IsHtmx') | Gets whether or not the current request is an Htmx triggered request. |
| [Method](Rizzy.Http.HtmxRequest.Method 'Rizzy.Http.HtmxRequest.Method') | Gets the HTTP method of the current request. |
| [Path](Rizzy.Http.HtmxRequest.Path 'Rizzy.Http.HtmxRequest.Path') | Gets the HTTP method of the current request. |
| [Prompt](Rizzy.Http.HtmxRequest.Prompt 'Rizzy.Http.HtmxRequest.Prompt') | Gets the user response to an hx-prompt, if any. |
| [Target](Rizzy.Http.HtmxRequest.Target 'Rizzy.Http.HtmxRequest.Target') | Gets the `id` of the target element if it exists. |
| [Trigger](Rizzy.Http.HtmxRequest.Trigger 'Rizzy.Http.HtmxRequest.Trigger') | Gets the `id` of the triggered element if it exists. |
| [TriggerName](Rizzy.Http.HtmxRequest.TriggerName 'Rizzy.Http.HtmxRequest.TriggerName') | Gets the `name` of the triggered element if it exists. |
