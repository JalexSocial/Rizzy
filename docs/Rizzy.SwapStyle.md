#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy')

## SwapStyle Enum

How to swap the response into the target element.

```csharp
public enum SwapStyle
```
### Fields

<a name='Rizzy.SwapStyle.afterbegin'></a>

`afterbegin` 4

Insert the response before the first child of the target element.

<a name='Rizzy.SwapStyle.afterend'></a>

`afterend` 6

Insert the response after the target element.

<a name='Rizzy.SwapStyle.beforebegin'></a>

`beforebegin` 3

Insert the response before the target element.

<a name='Rizzy.SwapStyle.beforeend'></a>

`beforeend` 5

Insert the response after the last child of the target element.

<a name='Rizzy.SwapStyle.Default'></a>

`Default` 0

Default style is what is specified in [DefaultSwapStyle](Rizzy.HtmxConfig.DefaultSwapStyle 'Rizzy.HtmxConfig.DefaultSwapStyle') for the application  
or htmx's default, which is [innerHTML](Rizzy.SwapStyle#Rizzy.SwapStyle.innerHTML 'Rizzy.SwapStyle.innerHTML').

### Remarks
This SwapStyle cannot be used directly in markup

<a name='Rizzy.SwapStyle.delete'></a>

`delete` 7

Deletes the target element regardless of the response.

<a name='Rizzy.SwapStyle.innerHTML'></a>

`innerHTML` 1

Replace the inner html of the target element.

<a name='Rizzy.SwapStyle.none'></a>

`none` 8

Does not append content from response (out of band items will still be processed).

<a name='Rizzy.SwapStyle.outerHTML'></a>

`outerHTML` 2

Replace the entire target element with the response.

### Remarks
The casing on each of these values matches htmx attributes so that they can be used directly in markup