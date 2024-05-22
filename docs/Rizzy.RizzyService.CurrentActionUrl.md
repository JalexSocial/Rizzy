#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[RizzyService](Rizzy.RizzyService.md 'Rizzy.RizzyService')

## RizzyService.CurrentActionUrl Property

Gets the current action method URL, which can be used as a callback URL in forms. This URL is automatically  
derived from the current HTTP request but can be manually overridden in form handler methods. It is particularly  
useful for specifying form action targets within Razor Component views.

```csharp
public string CurrentActionUrl { get; }
```

Implements [CurrentActionUrl](Rizzy.IRizzyService.CurrentActionUrl.md 'Rizzy.IRizzyService.CurrentActionUrl')

#### Property Value
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')