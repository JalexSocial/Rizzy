#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[HttpContextExtensions](Rizzy.HttpContextExtensions 'Rizzy.HttpContextExtensions')

## HttpContextExtensions.Htmx(this HttpResponse, Action<HtmxResponse>) Method

Extension method for HttpResponse that creates a new HtmxResponse and invokes the provided action.

```csharp
public static void Htmx(this Microsoft.AspNetCore.Http.HttpResponse response, System.Action<Rizzy.Http.HtmxResponse> action);
```
#### Parameters

<a name='Rizzy.HttpContextExtensions.Htmx(thisMicrosoft.AspNetCore.Http.HttpResponse,System.Action_Rizzy.Http.HtmxResponse_).response'></a>

`response` [Microsoft.AspNetCore.Http.HttpResponse](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpResponse 'Microsoft.AspNetCore.Http.HttpResponse')

The current HttpResponse.

<a name='Rizzy.HttpContextExtensions.Htmx(thisMicrosoft.AspNetCore.Http.HttpResponse,System.Action_Rizzy.Http.HtmxResponse_).action'></a>

`action` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-1 'System.Action`1')[HtmxResponse](Rizzy.Http.HtmxResponse 'Rizzy.Http.HtmxResponse')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-1 'System.Action`1')

An action that receives an HtmxResponse.