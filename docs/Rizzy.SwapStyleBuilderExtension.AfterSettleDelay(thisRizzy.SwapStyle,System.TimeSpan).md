#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension.md 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.AfterSettleDelay(this SwapStyle, TimeSpan) Method

Modifies the amount of time that htmx will wait between the swap   
and the settle logic by including the modifier `settle:`.

```csharp
public static Rizzy.SwapStyleBuilder AfterSettleDelay(this Rizzy.SwapStyle style, System.TimeSpan time);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.AfterSettleDelay(thisRizzy.SwapStyle,System.TimeSpan).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle.md 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.AfterSettleDelay(thisRizzy.SwapStyle,System.TimeSpan).time'></a>

`time` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

The amount of time htmx should wait after receiving a   
            response to swap the content.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
[time](Rizzy.SwapStyleBuilderExtension.AfterSettleDelay(thisRizzy.SwapStyle,System.TimeSpan).md#Rizzy.SwapStyleBuilderExtension.AfterSettleDelay(thisRizzy.SwapStyle,System.TimeSpan).time 'Rizzy.SwapStyleBuilderExtension.AfterSettleDelay(this Rizzy.SwapStyle, System.TimeSpan).time') will be converted to milliseconds if less than 1000, otherwise seconds,   
            meaning the resulting modifier will be `settle:500ms` for a [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan') of 500 milliseconds   
            or `settle:2s` for a [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan') of 2 seconds..