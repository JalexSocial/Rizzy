#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.AfterSwapDelay(this SwapStyle, TimeSpan) Method

Modifies the amount of time that htmx will wait after receiving a   
response to swap the content by including the modifier `swap:`.

```csharp
public static Rizzy.SwapStyleBuilder AfterSwapDelay(this Rizzy.SwapStyle style, System.TimeSpan time);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.AfterSwapDelay(thisRizzy.SwapStyle,System.TimeSpan).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.AfterSwapDelay(thisRizzy.SwapStyle,System.TimeSpan).time'></a>

`time` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

The amount of time htmx should wait after receiving a   
            response to swap the content.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
[time](Rizzy.SwapStyleBuilderExtension.AfterSwapDelay(thisRizzy.SwapStyle,System.TimeSpan)#Rizzy.SwapStyleBuilderExtension.AfterSwapDelay(thisRizzy.SwapStyle,System.TimeSpan).time 'Rizzy.SwapStyleBuilderExtension.AfterSwapDelay(this Rizzy.SwapStyle, System.TimeSpan).time') will be converted to milliseconds if less than 1000, otherwise seconds,   
            meaning the resulting modifier will be `swap:500ms` for a [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan') of 500 milliseconds   
            or `swap:2s` for a [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan') of 2 seconds..