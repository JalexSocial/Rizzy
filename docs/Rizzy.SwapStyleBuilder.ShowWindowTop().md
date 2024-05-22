#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.ShowWindowTop() Method

Specifies that the swap should smoothly scroll to the top of the window.

```csharp
public Rizzy.SwapStyleBuilder ShowWindowTop();
```

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `show:window:top`, instructing the content to be displayed  
at the top of the window following a swap by smoothly animating the scrollbar position. This can be useful  
for ensuring that important content or notifications at the top of the page are immediately visible to  
the user after a swap operation.