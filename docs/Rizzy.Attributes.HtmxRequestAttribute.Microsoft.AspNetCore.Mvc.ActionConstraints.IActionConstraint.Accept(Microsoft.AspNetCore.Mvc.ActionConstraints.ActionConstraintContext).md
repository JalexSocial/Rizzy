#### [Rizzy](index.md 'index')
### [Rizzy.Attributes](Rizzy.Attributes.md 'Rizzy.Attributes').[HtmxRequestAttribute](Rizzy.Attributes.HtmxRequestAttribute.md 'Rizzy.Attributes.HtmxRequestAttribute')

## HtmxRequestAttribute.Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint.Accept(ActionConstraintContext) Method

Determines whether the action method should be executed based on the characteristics of the incoming HTTP request.

```csharp
bool Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint.Accept(Microsoft.AspNetCore.Mvc.ActionConstraints.ActionConstraintContext context);
```
#### Parameters

<a name='Rizzy.Attributes.HtmxRequestAttribute.Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint.Accept(Microsoft.AspNetCore.Mvc.ActionConstraints.ActionConstraintContext).context'></a>

`context` [Microsoft.AspNetCore.Mvc.ActionConstraints.ActionConstraintContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.ActionConstraints.ActionConstraintContext 'Microsoft.AspNetCore.Mvc.ActionConstraints.ActionConstraintContext')

The context in which the action constraint is evaluated.

Implements [Accept(ActionConstraintContext)](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint.Accept#Microsoft_AspNetCore_Mvc_ActionConstraints_IActionConstraint_Accept_Microsoft_AspNetCore_Mvc_ActionConstraints_ActionConstraintContext_ 'Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint.Accept(Microsoft.AspNetCore.Mvc.ActionConstraints.ActionConstraintContext)')

### Remarks
This method initializes an [HtmxRequest](Rizzy.Http.HtmxRequest.md 'Rizzy.Http.HtmxRequest') object using the current [Microsoft.AspNetCore.Http.HttpContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext 'Microsoft.AspNetCore.Http.HttpContext')  
and then checks the `IsHtmx` property to determine if the request originates from HTMX.