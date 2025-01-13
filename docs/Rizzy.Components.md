#### [Rizzy](index 'index')

## Rizzy.Components Namespace

| Classes | |
| :--- | :--- |
| [DataAnnotationsProcessor](Rizzy.Components.DataAnnotationsProcessor 'Rizzy.Components.DataAnnotationsProcessor') | Processes Data Annotations on model properties and converts them to HTML5 data attributes.<br/>Registration Scope: Singleton |
| [FragmentComponent](Rizzy.Components.FragmentComponent 'Rizzy.Components.FragmentComponent') | Component that encapsulates a RenderFragment for use as a partial or child component. |
| [HtmxConfigHeadOutlet](Rizzy.Components.HtmxConfigHeadOutlet 'Rizzy.Components.HtmxConfigHeadOutlet') | This component will render a meta tag with the serialized [HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig') object,<br/>enabling customization of Htmx. |
| [HtmxSwapContent](Rizzy.Components.HtmxSwapContent 'Rizzy.Components.HtmxSwapContent') | Razor component that fully renders any content from HtmxSwapService |
| [HtmxSwappable](Rizzy.Components.HtmxSwappable 'Rizzy.Components.HtmxSwappable') | A Blazor component that enables swapping HTML content dynamically based on specified parameters through Htmx. |
| [HtmxSwapService](Rizzy.Components.HtmxSwapService 'Rizzy.Components.HtmxSwapService') | Service for managing dynamic content swaps in a Blazor application.<br/>Allows for adding Razor components, RenderFragments, and raw HTML content,<br/>and provides a mechanism to render them within a specified layout or context. |
| [RzHeadContent](Rizzy.Components.RzHeadContent 'Rizzy.Components.RzHeadContent') | Provides content to [RzHeadOutlet](Rizzy.Components.RzHeadOutlet 'Rizzy.Components.RzHeadOutlet') components. |
| [RzHeadOutlet](Rizzy.Components.RzHeadOutlet 'Rizzy.Components.RzHeadOutlet') | Renders content provided by [RzHeadContent](Rizzy.Components.RzHeadContent 'Rizzy.Components.RzHeadContent') components. |
| [RzInputCheckbox](Rizzy.Components.RzInputCheckbox 'Rizzy.Components.RzInputCheckbox') | An input component for editing Boolean values. |
| [RzInputDate&lt;TValue&gt;](Rizzy.Components.RzInputDate_TValue_ 'Rizzy.Components.RzInputDate<TValue>') | An input component for editing date values. |
| [RzInputFile](Rizzy.Components.RzInputFile 'Rizzy.Components.RzInputFile') | A component that wraps the HTML file input element and supplies a Stream for each file's contents. |
| [RzInputNumber&lt;TValue&gt;](Rizzy.Components.RzInputNumber_TValue_ 'Rizzy.Components.RzInputNumber<TValue>') | An input component for editing numeric values. Supported numeric types are<br/>Int32, Int64, Int16, Single, Double, Decimal. |
| [RzInputRadio&lt;TValue&gt;](Rizzy.Components.RzInputRadio_TValue_ 'Rizzy.Components.RzInputRadio<TValue>') | An input component used for selecting a value from a group of choices. |
| [RzInputRadioGroup&lt;TValue&gt;](Rizzy.Components.RzInputRadioGroup_TValue_ 'Rizzy.Components.RzInputRadioGroup<TValue>') | Groups child InputRadio components. |
| [RzInputSelect&lt;TValue&gt;](Rizzy.Components.RzInputSelect_TValue_ 'Rizzy.Components.RzInputSelect<TValue>') | A dropdown selection component. |
| [RzPage](Rizzy.Components.RzPage 'Rizzy.Components.RzPage') | Page component container that provides a ViewContext cascading value |
| [RzPageTitle](Rizzy.Components.RzPageTitle 'Rizzy.Components.RzPageTitle') | Enables rendering an HTML `<title>` to a [RzHeadOutlet](Rizzy.Components.RzHeadOutlet 'Rizzy.Components.RzHeadOutlet') component. This component currently is not operational in SSR mode<br/>due to existing Blazor issues.  See https://github.com/dotnet/aspnetcore/issues/50268 |
| [RzSectionContent](Rizzy.Components.RzSectionContent 'Rizzy.Components.RzSectionContent') | This class acts as a proxy to the original [Microsoft.AspNetCore.Components.Sections.SectionContent](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Sections.SectionContent 'Microsoft.AspNetCore.Components.Sections.SectionContent'), forwarding all parameters directly to it<br/>and rendering its output with no additional markup. |
| [RzSectionOutlet](Rizzy.Components.RzSectionOutlet 'Rizzy.Components.RzSectionOutlet') | Renders content provided by [RzSectionContent](Rizzy.Components.RzSectionContent 'Rizzy.Components.RzSectionContent') components with matching [SectionId](Rizzy.Components.RzSectionOutlet.SectionId 'Rizzy.Components.RzSectionOutlet.SectionId')s.<br/>This class acts as a proxy to the original [Microsoft.AspNetCore.Components.Sections.SectionOutlet](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Sections.SectionOutlet 'Microsoft.AspNetCore.Components.Sections.SectionOutlet'), forwarding all parameters directly to it<br/>and rendering its output with no additional markup. |
| [RzValidationSummary](Rizzy.Components.RzValidationSummary 'Rizzy.Components.RzValidationSummary') | Inherits ValidationSummary - Included for consistency |

| Interfaces | |
| :--- | :--- |
| [IHtmxSwapService](Rizzy.Components.IHtmxSwapService 'Rizzy.Components.IHtmxSwapService') | |
