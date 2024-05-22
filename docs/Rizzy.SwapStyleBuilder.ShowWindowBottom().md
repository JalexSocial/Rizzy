#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.ShowWindowBottom() Method

Specifies that the swap should smoothly scroll to the bottom of the window.

```csharp
public Rizzy.SwapStyleBuilder ShowWindowBottom();
```

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `show:window:bottom`, instructing the content to be displayed  
at the bottom of the window following a swap by smoothly animating the scrollbar position. This positioning  
can be used for infinite scrolling, footers, or information appended at the end of the page.