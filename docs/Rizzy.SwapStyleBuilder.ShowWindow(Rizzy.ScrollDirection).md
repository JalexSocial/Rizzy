#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.ShowWindow(ScrollDirection) Method

Specifies that the swap should show in the window by smoothly scrolling to either the top or bottom of the window.

```csharp
public Rizzy.SwapStyleBuilder ShowWindow(Rizzy.ScrollDirection direction);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilder.ShowWindow(Rizzy.ScrollDirection).direction'></a>

`direction` [ScrollDirection](Rizzy.ScrollDirection.md 'Rizzy.ScrollDirection')

The direction to scroll the window after the swap.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `show:window:`, directing the swap to display the specified  
element at the bottom of the window.