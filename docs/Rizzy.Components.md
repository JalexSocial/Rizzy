#### [Rizzy](index.md 'index')

## Rizzy.Components Namespace

| Classes | |
| :--- | :--- |
| [HtmxConfigHeadOutlet](Rizzy.Components.HtmxConfigHeadOutlet.md 'Rizzy.Components.HtmxConfigHeadOutlet') | This component will render a meta tag with the serialized [HtmxConfig](Rizzy.HtmxConfig.md 'Rizzy.HtmxConfig') object,<br/>enabling customization of Htmx. |
| [HtmxSwapContent](Rizzy.Components.HtmxSwapContent.md 'Rizzy.Components.HtmxSwapContent') | Razor component that fully renders any content from HtmxSwapService |
| [HtmxSwappable](Rizzy.Components.HtmxSwappable.md 'Rizzy.Components.HtmxSwappable') | A Blazor component that enables swapping HTML content dynamically based on specified parameters through Htmx. |
| [HtmxSwapService](Rizzy.Components.HtmxSwapService.md 'Rizzy.Components.HtmxSwapService') | Service for managing dynamic content swaps in a Blazor application.<br/>Allows for adding Razor components, RenderFragments, and raw HTML content,<br/>and provides a mechanism to render them within a specified layout or context. |
| [RzEditForm](Rizzy.Components.RzEditForm.md 'Rizzy.Components.RzEditForm') | |
| [RzHeadContent](Rizzy.Components.RzHeadContent.md 'Rizzy.Components.RzHeadContent') | Provides content to [RzHeadOutlet](Rizzy.Components.RzHeadOutlet.md 'Rizzy.Components.RzHeadOutlet') components. |
| [RzHeadOutlet](Rizzy.Components.RzHeadOutlet.md 'Rizzy.Components.RzHeadOutlet') | Renders content provided by [RzHeadContent](Rizzy.Components.RzHeadContent.md 'Rizzy.Components.RzHeadContent') components. |
| [RzInputCheckbox](Rizzy.Components.RzInputCheckbox.md 'Rizzy.Components.RzInputCheckbox') | An input component for editing Boolean values. |
| [RzInputDate&lt;TValue&gt;](Rizzy.Components.RzInputDate_TValue_.md 'Rizzy.Components.RzInputDate<TValue>') | An input component for editing date values. |
| [RzInputFile](Rizzy.Components.RzInputFile.md 'Rizzy.Components.RzInputFile') | A component that wraps the HTML file input element and supplies a Stream for each file's contents. |
| [RzInputNumber&lt;TValue&gt;](Rizzy.Components.RzInputNumber_TValue_.md 'Rizzy.Components.RzInputNumber<TValue>') | An input component for editing numeric values. Supported numeric types are<br/>Int32, Int64, Int16, Single, Double, Decimal. |
| [RzInputRadio&lt;TValue&gt;](Rizzy.Components.RzInputRadio_TValue_.md 'Rizzy.Components.RzInputRadio<TValue>') | An input component used for selecting a value from a group of choices. |
| [RzInputRadioGroup&lt;TValue&gt;](Rizzy.Components.RzInputRadioGroup_TValue_.md 'Rizzy.Components.RzInputRadioGroup<TValue>') | Groups child InputRadio components. |
| [RzInputSelect&lt;TValue&gt;](Rizzy.Components.RzInputSelect_TValue_.md 'Rizzy.Components.RzInputSelect<TValue>') | A dropdown selection component. |
| [RzPage](Rizzy.Components.RzPage.md 'Rizzy.Components.RzPage') | Page component container that provides a ViewContext cascading value |
| [RzPageTitle](Rizzy.Components.RzPageTitle.md 'Rizzy.Components.RzPageTitle') | Enables rendering an HTML `<title>` to a [RzHeadOutlet](Rizzy.Components.RzHeadOutlet.md 'Rizzy.Components.RzHeadOutlet') component. This component currently is not operational in SSR mode<br/>due to existing Blazor issues.  See https://github.com/dotnet/aspnetcore/issues/50268 |
| [RzSectionContent](Rizzy.Components.RzSectionContent.md 'Rizzy.Components.RzSectionContent') | This class acts as a proxy to the original [Microsoft.AspNetCore.Components.Sections.SectionContent](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Sections.SectionContent 'Microsoft.AspNetCore.Components.Sections.SectionContent'), forwarding all parameters directly to it<br/>and rendering its output with no additional markup. |
| [RzSectionOutlet](Rizzy.Components.RzSectionOutlet.md 'Rizzy.Components.RzSectionOutlet') | Renders content provided by [RzSectionContent](Rizzy.Components.RzSectionContent.md 'Rizzy.Components.RzSectionContent') components with matching [SectionId](Rizzy.Components.RzSectionOutlet.SectionId.md 'Rizzy.Components.RzSectionOutlet.SectionId')s.<br/>This class acts as a proxy to the original [Microsoft.AspNetCore.Components.Sections.SectionOutlet](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Sections.SectionOutlet 'Microsoft.AspNetCore.Components.Sections.SectionOutlet'), forwarding all parameters directly to it<br/>and rendering its output with no additional markup. |
| [RzValidationSummary](Rizzy.Components.RzValidationSummary.md 'Rizzy.Components.RzValidationSummary') | Inherits ValidationSummary - Included for consistency |

| Interfaces | |
| :--- | :--- |
| [IHtmxSwapService](Rizzy.Components.IHtmxSwapService.md 'Rizzy.Components.IHtmxSwapService') | |
