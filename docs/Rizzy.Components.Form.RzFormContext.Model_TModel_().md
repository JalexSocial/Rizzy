#### [Rizzy](index 'index')
### [Rizzy.Components.Form](Rizzy.Components.Form 'Rizzy.Components.Form').[RzFormContext](Rizzy.Components.Form.RzFormContext 'Rizzy.Components.Form.RzFormContext')

## RzFormContext.Model<TModel>() Method

Retrieves the current data strongly-typed data model for EditContext. Note that EditContext must be set with a strongly-typed  
model for the form model to be accessible.  This is essentially a shortcut for EditContext.Model which does the typecasting for  
you.

```csharp
public TModel Model<TModel>();
```
#### Type parameters

<a name='Rizzy.Components.Form.RzFormContext.Model_TModel_().TModel'></a>

`TModel`

#### Returns
[TModel](Rizzy.Components.Form.RzFormContext.Model_TModel_()#Rizzy.Components.Form.RzFormContext.Model_TModel_().TModel 'Rizzy.Components.Form.RzFormContext.Model<TModel>().TModel')

#### Exceptions

[System.NullReferenceException](https://docs.microsoft.com/en-us/dotnet/api/System.NullReferenceException 'System.NullReferenceException')

[System.InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException 'System.InvalidOperationException')