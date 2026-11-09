using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Rizzy.Tests.Pipeline;

// Razor components are written in C# so the test project does not need the Razor SDK.

/// <summary>Root layout: the full HTML document with a HeadOutlet for the htmx-config meta tag.</summary>
public sealed class TestRootLayout : LayoutComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0, "<!DOCTYPE html>");
        builder.OpenElement(1, "html");
        builder.OpenElement(2, "head");
        builder.OpenComponent<HeadOutlet>(3);
        builder.CloseComponent();
        builder.CloseElement();
        builder.OpenElement(4, "body");
        builder.AddAttribute(5, "data-layout", "root");
        builder.AddContent(6, Body);
        builder.CloseElement();
        builder.CloseElement();
    }
}

/// <summary>Application layout (navigation, chrome). Must disappear for partial htmx requests.</summary>
public sealed class TestMainLayout : LayoutComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "id", "main-layout");
        builder.AddContent(2, Body);
        builder.CloseElement();
    }
}

public sealed class TestPage : ComponentBase
{
    [Parameter] public string? Message { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "p");
        builder.AddAttribute(1, "id", "page");
        builder.AddContent(2, Message);
        builder.CloseElement();
    }
}

/// <summary>Adds an out-of-band swap through <see cref="IHtmxSwapService"/>.</summary>
public sealed class TestSwapPage : ComponentBase
{
    [Inject] public IHtmxSwapService SwapService { get; set; } = default!;

    protected override void OnInitialized()
    {
        SwapService.AddSwappableFragment("oob-target", b => b.AddContent(0, "oob content"));
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0, "<p id=\"swap-page\">main content</p>");
    }
}

public sealed class TestPerson
{
    [Required]
    public string? Name { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Range(1, 120)]
    public int Age { get; set; } = 30;

    public bool Subscribe { get; set; }

    public DateTime BirthDate { get; set; } = new(2000, 1, 2);

    [StringLength(200, MinimumLength = 2)]
    public string? Bio { get; set; }

    public string Color { get; set; } = "red";

    public string? Token { get; set; } = "t1";
}

/// <summary>A form using every Rizzy input component, for rendering and validation-attribute tests.</summary>
public sealed class TestFormPage : ComponentBase
{
    [Parameter] public TestPerson Person { get; set; } = new();

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<EditForm>(0);
        builder.AddComponentParameter(1, nameof(EditForm.Model), Person);
        builder.AddComponentParameter(2, nameof(EditForm.FormName), "person");
        builder.AddComponentParameter(3, nameof(EditForm.ChildContent), (RenderFragment<EditContext>)(_ => b =>
        {
            b.OpenComponent<RzModelStateProvider>(10);
            b.CloseComponent();

            Input<RzInputTextBase, string?>(b, 20, Person.Name, v => Person.Name = v, () => Person.Name);
            Input<RzInputTextBase, string?>(b, 30, Person.Email, v => Person.Email = v, () => Person.Email);
            Input<RzInputNumberBase<int>, int>(b, 40, Person.Age, v => Person.Age = v, () => Person.Age);
            Input<RzInputCheckboxBase, bool>(b, 50, Person.Subscribe, v => Person.Subscribe = v, () => Person.Subscribe);
            Input<RzInputDateBase<DateTime>, DateTime>(b, 60, Person.BirthDate, v => Person.BirthDate = v, () => Person.BirthDate);
            Input<RzInputTextAreaBase, string?>(b, 70, Person.Bio, v => Person.Bio = v, () => Person.Bio);
            Input<RzInputHidden<string>, string?>(b, 80, Person.Token, v => Person.Token = v, () => Person.Token);

            b.OpenComponent<RzInputSelectBase<string>>(90);
            b.AddComponentParameter(91, "Value", Person.Color);
            b.AddComponentParameter(92, "ValueChanged", EventCallback.Factory.Create<string>(this, v => Person.Color = v));
            b.AddComponentParameter(93, "ValueExpression", (Expression<Func<string>>)(() => Person.Color));
            b.AddComponentParameter(94, "ChildContent", (RenderFragment)(o => o.AddMarkupContent(0, "<option value=\"red\">Red</option><option value=\"blue\">Blue</option>")));
            b.CloseComponent();

            b.OpenComponent<RzValidationMessageBase<string?>>(100);
            b.AddComponentParameter(101, "For", (Expression<Func<string?>>)(() => Person.Name));
            b.CloseComponent();

            b.OpenComponent<RzValidationMessageBase<string?>>(110);
            b.AddComponentParameter(111, "For", (Expression<Func<string?>>)(() => Person.Email));
            b.CloseComponent();

            b.OpenComponent<RzValidationSummaryBase>(120);
            b.CloseComponent();
        }));
        builder.CloseComponent();
    }

    // Each call renders one component inside its own region, so fixed sequence numbers are valid.
#pragma warning disable ASP0006 // the region number is a per-call literal supplied by the caller
    private void Input<TComponent, TValue>(RenderTreeBuilder b, int region, TValue value, Action<TValue> setter, Expression<Func<TValue>> expression)
        where TComponent : IComponent
    {
        b.OpenRegion(region);
        b.OpenComponent<TComponent>(0);
        b.AddComponentParameter(1, "Value", value);
        b.AddComponentParameter(2, "ValueChanged", EventCallback.Factory.Create(this, setter));
        b.AddComponentParameter(3, "ValueExpression", expression);
        b.CloseComponent();
        b.CloseRegion();
    }
#pragma warning restore ASP0006
}

/// <summary>A page that binds a value posted with a named form (Blazor SSR form mapping).</summary>
public sealed class TestPostedFormPage : ComponentBase
{
    [SupplyParameterFromForm(FormName = "greet")] public string? Name { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "p");
        builder.AddAttribute(1, "id", "posted");
        builder.AddContent(2, "posted:" + (Name ?? "<null>"));
        builder.CloseElement();
    }
}
