#### [Rizzy](index.md 'index')
### [Rizzy.Http](Rizzy.Http.md 'Rizzy.Http').[HtmxResponse](Rizzy.Http.HtmxResponse.md 'Rizzy.Http.HtmxResponse')

## HtmxResponse.Trigger<TEventDetail>(string, TEventDetail, TriggerTiming, JsonSerializerOptions) Method

Allows you to trigger client-side events.

```csharp
public Rizzy.Http.HtmxResponse Trigger<TEventDetail>(string eventName, TEventDetail detail, Rizzy.TriggerTiming timing=Rizzy.TriggerTiming.Default, System.Text.Json.JsonSerializerOptions? jsonSerializerOptions=null);
```
#### Type parameters

<a name='Rizzy.Http.HtmxResponse.Trigger_TEventDetail_(string,TEventDetail,Rizzy.TriggerTiming,System.Text.Json.JsonSerializerOptions).TEventDetail'></a>

`TEventDetail`
#### Parameters

<a name='Rizzy.Http.HtmxResponse.Trigger_TEventDetail_(string,TEventDetail,Rizzy.TriggerTiming,System.Text.Json.JsonSerializerOptions).eventName'></a>

`eventName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The name of client side event to trigger.

<a name='Rizzy.Http.HtmxResponse.Trigger_TEventDetail_(string,TEventDetail,Rizzy.TriggerTiming,System.Text.Json.JsonSerializerOptions).detail'></a>

`detail` [TEventDetail](Rizzy.Http.HtmxResponse.Trigger_TEventDetail_(string,TEventDetail,Rizzy.TriggerTiming,System.Text.Json.JsonSerializerOptions).md#Rizzy.Http.HtmxResponse.Trigger_TEventDetail_(string,TEventDetail,Rizzy.TriggerTiming,System.Text.Json.JsonSerializerOptions).TEventDetail 'Rizzy.Http.HtmxResponse.Trigger<TEventDetail>(string, TEventDetail, Rizzy.TriggerTiming, System.Text.Json.JsonSerializerOptions).TEventDetail')

The details to pass the client side event.

<a name='Rizzy.Http.HtmxResponse.Trigger_TEventDetail_(string,TEventDetail,Rizzy.TriggerTiming,System.Text.Json.JsonSerializerOptions).timing'></a>

`timing` [TriggerTiming](Rizzy.TriggerTiming.md 'Rizzy.TriggerTiming')

When the event should be triggered.

<a name='Rizzy.Http.HtmxResponse.Trigger_TEventDetail_(string,TEventDetail,Rizzy.TriggerTiming,System.Text.Json.JsonSerializerOptions).jsonSerializerOptions'></a>

`jsonSerializerOptions` [System.Text.Json.JsonSerializerOptions](https://docs.microsoft.com/en-us/dotnet/api/System.Text.Json.JsonSerializerOptions 'System.Text.Json.JsonSerializerOptions')

The [System.Text.Json.JsonSerializerOptions](https://docs.microsoft.com/en-us/dotnet/api/System.Text.Json.JsonSerializerOptions 'System.Text.Json.JsonSerializerOptions') to use to convert the [detail](Rizzy.Http.HtmxResponse.Trigger_TEventDetail_(string,TEventDetail,Rizzy.TriggerTiming,System.Text.Json.JsonSerializerOptions).md#Rizzy.Http.HtmxResponse.Trigger_TEventDetail_(string,TEventDetail,Rizzy.TriggerTiming,System.Text.Json.JsonSerializerOptions).detail 'Rizzy.Http.HtmxResponse.Trigger<TEventDetail>(string, TEventDetail, Rizzy.TriggerTiming, System.Text.Json.JsonSerializerOptions).detail') into JSON.   
            If not specified, a [Microsoft.AspNetCore.Http.Json.JsonOptions.SerializerOptions](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.Json.JsonOptions.SerializerOptions 'Microsoft.AspNetCore.Http.Json.JsonOptions.SerializerOptions') is retrieved [Microsoft.AspNetCore.Http.HttpContext.RequestServices](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext.RequestServices 'Microsoft.AspNetCore.Http.HttpContext.RequestServices') and used if available.

#### Returns
[HtmxResponse](Rizzy.Http.HtmxResponse.md 'Rizzy.Http.HtmxResponse')  
This [HtmxResponse](Rizzy.Http.HtmxResponse.md 'Rizzy.Http.HtmxResponse') object instance.