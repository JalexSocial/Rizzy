#### [Rizzy](index.md 'index')
### [Rizzy.Http](Rizzy.Http.md 'Rizzy.Http').[HtmxResponse](Rizzy.Http.HtmxResponse.md 'Rizzy.Http.HtmxResponse')

## HtmxResponse.Trigger(string, TriggerTiming) Method

Allows you to trigger client-side events.

```csharp
public Rizzy.Http.HtmxResponse Trigger(string eventName, Rizzy.TriggerTiming timing=Rizzy.TriggerTiming.Default);
```
#### Parameters

<a name='Rizzy.Http.HtmxResponse.Trigger(string,Rizzy.TriggerTiming).eventName'></a>

`eventName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The name of client side event to trigger.

<a name='Rizzy.Http.HtmxResponse.Trigger(string,Rizzy.TriggerTiming).timing'></a>

`timing` [TriggerTiming](Rizzy.TriggerTiming.md 'Rizzy.TriggerTiming')

When the event should be triggered.

#### Returns
[HtmxResponse](Rizzy.Http.HtmxResponse.md 'Rizzy.Http.HtmxResponse')  
This [HtmxResponse](Rizzy.Http.HtmxResponse.md 'Rizzy.Http.HtmxResponse') object instance.