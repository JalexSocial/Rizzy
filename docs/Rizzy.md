#### [Rizzy](index.md 'index')

## Rizzy Namespace

| Classes | |
| :--- | :--- |
| [HtmxTriggerSpecification](Rizzy.HtmxTriggerSpecification.md 'Rizzy.HtmxTriggerSpecification') | Represents an htmx trigger definition |
| [RizzyApplicationBuilderExtensions](Rizzy.RizzyApplicationBuilderExtensions.md 'Rizzy.RizzyApplicationBuilderExtensions') | This class has extension methods for [Microsoft.Extensions.Hosting.IHostApplicationBuilder](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.Hosting.IHostApplicationBuilder 'Microsoft.Extensions.Hosting.IHostApplicationBuilder') and [Microsoft.AspNetCore.Builder.IApplicationBuilder](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Builder.IApplicationBuilder 'Microsoft.AspNetCore.Builder.IApplicationBuilder') <br/>that enable configuration of Htmx in the application. |
| [RizzyService](Rizzy.RizzyService.md 'Rizzy.RizzyService') | Represents a proxy base for Rizzy services that facilitate access to Razor Component views. This class provides<br/>mechanisms to render both full and partial Razor views dynamically based on specified component types and parameters. |
| [RzViewContext](Rizzy.RzViewContext.md 'Rizzy.RzViewContext') | Represents the context for a view within an application, providing access to HTTP contexts, URL helpers, and component configurations. |
| [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') | A builder class for constructing a swap style command string for HTMX responses. |
| [SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension.md 'Rizzy.SwapStyleBuilderExtension') | Extension methods for the SwapStyle enum to facilitate building swap style commands. |
| [SwapStyleExtensions](Rizzy.SwapStyleExtensions.md 'Rizzy.SwapStyleExtensions') | |
| [Trigger](Rizzy.Trigger.md 'Rizzy.Trigger') | Creates builder objects to begin the fluent chain for constructing triggers |
| [TriggerBuilder](Rizzy.TriggerBuilder.md 'Rizzy.TriggerBuilder') | Provides methods to construct and manage htmx trigger definitions for htmx requests. |
| [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder') | Provides methods to add modifiers to htmx trigger definitions within a [TriggerBuilder](Rizzy.TriggerBuilder.md 'Rizzy.TriggerBuilder') context. |
| [TriggerSpecificationCache](Rizzy.TriggerSpecificationCache.md 'Rizzy.TriggerSpecificationCache') | htmx configuration allows for the creation of a trigger specification cache in order to improve<br/>trigger-handling performance.  The cache is a key/value store mapping well-formed hx-trigger parameters<br/>to their parsed specifications. |

| Interfaces | |
| :--- | :--- |
| [IRizzyService](Rizzy.IRizzyService.md 'Rizzy.IRizzyService') | |

| Enums | |
| :--- | :--- |
| [ScrollBehavior](Rizzy.ScrollBehavior.md 'Rizzy.ScrollBehavior') | The behavior for a boosted link on page transitions. |
| [ScrollDirection](Rizzy.ScrollDirection.md 'Rizzy.ScrollDirection') | Direction values for scroll and show modifier methods |
| [SwapStyle](Rizzy.SwapStyle.md 'Rizzy.SwapStyle') | How to swap the response into the target element. |
| [TriggerQueueOption](Rizzy.TriggerQueueOption.md 'Rizzy.TriggerQueueOption') | Determines how events are queued if an event occurs while a request for another event is in flight. |
| [TriggerTiming](Rizzy.TriggerTiming.md 'Rizzy.TriggerTiming') | |
