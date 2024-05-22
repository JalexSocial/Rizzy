#### [Rizzy](index 'index')
### [Rizzy.Attributes](Rizzy.Attributes 'Rizzy.Attributes')

## HtmxRequestAttribute Class

An action filter attribute that implements [Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint 'Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint') to ensure  
an action method is executed only for HTMX requests.

```csharp
public sealed class HtmxRequestAttribute : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute,
Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint,
Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraintMetadata
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; [Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute 'Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute') &#129106; HtmxRequestAttribute

Implements [Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint 'Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint'), [Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraintMetadata](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraintMetadata 'Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraintMetadata')

### Remarks
HTMX requests are determined by the presence of specific HTTP headers. This attribute  
uses the [HtmxRequest](Rizzy.Http.HtmxRequest 'Rizzy.Http.HtmxRequest') class to check these headers in the incoming request.

| Properties | |
| :--- | :--- |
| [Target](Rizzy.Attributes.HtmxRequestAttribute.Target 'Rizzy.Attributes.HtmxRequestAttribute.Target') | Gets or sets the target element's identifier for which the action should be executed.<br/>If specified, only requests targeting this element will be accepted. |

| Explicit Interface Implementations | |
| :--- | :--- |
| [Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint.Accept(ActionConstraintContext)](Rizzy.Attributes.HtmxRequestAttribute.Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint.Accept(Microsoft.AspNetCore.Mvc.ActionConstraints.ActionConstraintContext) 'Rizzy.Attributes.HtmxRequestAttribute.Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint.Accept(Microsoft.AspNetCore.Mvc.ActionConstraints.ActionConstraintContext)') | Determines whether the action method should be executed based on the characteristics of the incoming HTTP request. |
