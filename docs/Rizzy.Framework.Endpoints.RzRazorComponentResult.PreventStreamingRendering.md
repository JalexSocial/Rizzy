#### [Rizzy](index 'index')
### [Rizzy.Framework.Endpoints](Rizzy.Framework.Endpoints 'Rizzy.Framework.Endpoints').[RzRazorComponentResult](Rizzy.Framework.Endpoints.RzRazorComponentResult 'Rizzy.Framework.Endpoints.RzRazorComponentResult')

## RzRazorComponentResult.PreventStreamingRendering Property

Gets or sets a flag to indicate whether streaming rendering should be prevented. If true, the renderer will  
wait for the component hierarchy to complete asynchronous tasks such as loading before supplying the HTML response.  
If false, streaming rendering will be determined by the components being rendered.  
  
The default value is false.

```csharp
public bool PreventStreamingRendering { get; set; }
```

#### Property Value
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')