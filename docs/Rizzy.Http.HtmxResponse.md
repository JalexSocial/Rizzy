#### [Rizzy](index 'index')
### [Rizzy.Http](Rizzy.Http 'Rizzy.Http')

## HtmxResponse Class

```csharp
public sealed class HtmxResponse
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; HtmxResponse

| Methods | |
| :--- | :--- |
| [EmptyBody()](Rizzy.Http.HtmxResponse.EmptyBody() 'Rizzy.Http.HtmxResponse.EmptyBody()') | Do not render any component markup to the client, even if the component would have<br/>produced markup normally. Headers and cookies are still returned as normal. |
| [Location(LocationTarget)](Rizzy.Http.HtmxResponse.Location(Rizzy.Http.LocationTarget) 'Rizzy.Http.HtmxResponse.Location(Rizzy.Http.LocationTarget)') | Allows you to do a client-side redirect that does not do a full page reload. |
| [Location(string)](Rizzy.Http.HtmxResponse.Location(string) 'Rizzy.Http.HtmxResponse.Location(string)') | Allows you to do a client-side redirect that does not do a full page reload. |
| [PreventBrowserCurrentUrlUpdate()](Rizzy.Http.HtmxResponse.PreventBrowserCurrentUrlUpdate() 'Rizzy.Http.HtmxResponse.PreventBrowserCurrentUrlUpdate()') | Prevents the browser’s current url from being updated<br/>Overwrites ReplaceUrl response if already present. |
| [PreventBrowserHistoryUpdate()](Rizzy.Http.HtmxResponse.PreventBrowserHistoryUpdate() 'Rizzy.Http.HtmxResponse.PreventBrowserHistoryUpdate()') | Prevents the browser’s history from being updated.<br/>Overwrites PushUrl response if already present. |
| [PushUrl(string)](Rizzy.Http.HtmxResponse.PushUrl(string) 'Rizzy.Http.HtmxResponse.PushUrl(string)') | Pushes a new url onto the history stack. |
| [PushUrl(Uri)](Rizzy.Http.HtmxResponse.PushUrl(System.Uri) 'Rizzy.Http.HtmxResponse.PushUrl(System.Uri)') | Pushes a new url onto the history stack. |
| [Redirect(string)](Rizzy.Http.HtmxResponse.Redirect(string) 'Rizzy.Http.HtmxResponse.Redirect(string)') | Can be used to do a client-side redirect to a new location. |
| [Redirect(Uri)](Rizzy.Http.HtmxResponse.Redirect(System.Uri) 'Rizzy.Http.HtmxResponse.Redirect(System.Uri)') | Can be used to do a client-side redirect to a new location. |
| [Refresh()](Rizzy.Http.HtmxResponse.Refresh() 'Rizzy.Http.HtmxResponse.Refresh()') | Enables a client-side full refresh of the page. |
| [ReplaceUrl(string)](Rizzy.Http.HtmxResponse.ReplaceUrl(string) 'Rizzy.Http.HtmxResponse.ReplaceUrl(string)') | Replaces the current URL in the location bar. |
| [ReplaceUrl(Uri)](Rizzy.Http.HtmxResponse.ReplaceUrl(System.Uri) 'Rizzy.Http.HtmxResponse.ReplaceUrl(System.Uri)') | Replaces the current URL in the location bar. |
| [Reselect(string)](Rizzy.Http.HtmxResponse.Reselect(string) 'Rizzy.Http.HtmxResponse.Reselect(string)') | A CSS selector that allows you to choose which part of the response is used to be swapped in. |
| [Reswap(SwapStyle, string)](Rizzy.Http.HtmxResponse.Reswap(Rizzy.SwapStyle,string) 'Rizzy.Http.HtmxResponse.Reswap(Rizzy.SwapStyle, string)') | Allows you to specify how the response will be swapped. |
| [Reswap(SwapStyleBuilder)](Rizzy.Http.HtmxResponse.Reswap(Rizzy.SwapStyleBuilder) 'Rizzy.Http.HtmxResponse.Reswap(Rizzy.SwapStyleBuilder)') | Allows you to specify how the response will be swapped. |
| [Reswap(string)](Rizzy.Http.HtmxResponse.Reswap(string) 'Rizzy.Http.HtmxResponse.Reswap(string)') | Allows you to specify how the response will be swapped. |
| [Retarget(string)](Rizzy.Http.HtmxResponse.Retarget(string) 'Rizzy.Http.HtmxResponse.Retarget(string)') | A CSS selector that updates the target of the content update to a different element on the page. |
| [StatusCode(HttpStatusCode)](Rizzy.Http.HtmxResponse.StatusCode(System.Net.HttpStatusCode) 'Rizzy.Http.HtmxResponse.StatusCode(System.Net.HttpStatusCode)') | Sets the response status code to [statusCode](Rizzy.Http.HtmxResponse.StatusCode(System.Net.HttpStatusCode)#Rizzy.Http.HtmxResponse.StatusCode(System.Net.HttpStatusCode).statusCode 'Rizzy.Http.HtmxResponse.StatusCode(System.Net.HttpStatusCode).statusCode'). |
| [StopPolling()](Rizzy.Http.HtmxResponse.StopPolling() 'Rizzy.Http.HtmxResponse.StopPolling()') | Sets response code to stop polling |
| [Trigger(string, TriggerTiming)](Rizzy.Http.HtmxResponse.Trigger(string,Rizzy.TriggerTiming) 'Rizzy.Http.HtmxResponse.Trigger(string, Rizzy.TriggerTiming)') | Allows you to trigger client-side events. |
| [Trigger&lt;TEventDetail&gt;(string, TEventDetail, TriggerTiming, JsonSerializerOptions)](Rizzy.Http.HtmxResponse.Trigger_TEventDetail_(string,TEventDetail,Rizzy.TriggerTiming,System.Text.Json.JsonSerializerOptions) 'Rizzy.Http.HtmxResponse.Trigger<TEventDetail>(string, TEventDetail, Rizzy.TriggerTiming, System.Text.Json.JsonSerializerOptions)') | Allows you to trigger client-side events. |
