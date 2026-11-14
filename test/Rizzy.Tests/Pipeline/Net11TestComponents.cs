using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Rizzy.Tests.Pipeline;

/// <summary>A page that sets its title with RzPageTitle's Text parameter.</summary>
public sealed class TestTitleTextPage : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<RzPageTitle>(0);
        builder.AddComponentParameter(1, nameof(RzPageTitle.Text), "Text title");
        builder.CloseComponent();
        builder.AddMarkupContent(2, "<p id=\"page\">titled</p>");
    }
}

#if NET11_0_OR_GREATER
/// <summary>Counts its renders, so a test can tell a CacheView hit from a fresh render.</summary>
public sealed class TestRenderCounter : ComponentBase
{
    private static int s_renders;
    private int _value;

    public static int Renders => Volatile.Read(ref s_renders);

    protected override void OnInitialized() => _value = Interlocked.Increment(ref s_renders);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "id", "counter");
        builder.AddContent(2, _value);
        builder.CloseElement();
    }
}

/// <summary>An RzPageTitle and a render counter inside a CacheView (.NET 11).</summary>
public sealed class TestCachedTitlePage : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CacheView>(0);
        builder.AddComponentParameter(1, nameof(CacheView.ExpiresAfter), TimeSpan.FromMinutes(5));
        builder.AddComponentParameter(2, nameof(CacheView.ChildContent), (RenderFragment)(b =>
        {
            b.OpenComponent<RzPageTitle>(0);
            b.AddComponentParameter(1, nameof(RzPageTitle.Text), "Cached title");
            b.CloseComponent();
            b.OpenComponent<TestRenderCounter>(2);
            b.CloseComponent();
        }));
        builder.CloseComponent();
    }
}

/// <summary>An async validation attribute that always fails, standing in for a database lookup.</summary>
public sealed class TestUsernameTakenAttribute : AsyncValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        => throw new InvalidOperationException("Only validates asynchronously.");

    protected override async Task<ValidationResult?> IsValidAsync(object? value, ValidationContext validationContext, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return new ValidationResult("That username is taken.", [validationContext.MemberName!]);
    }
}

public sealed class TestAsyncModel
{
    [TestUsernameTaken]
    public string? Username { get; set; } = "ada";
}

/// <summary>A form validated on render by RzInitialValidator, with an async attribute on its model.</summary>
public sealed class TestAsyncFormPage : ComponentBase
{
    private readonly TestAsyncModel _model = new();

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<EditForm>(0);
        builder.AddComponentParameter(1, nameof(EditForm.Model), _model);
        builder.AddComponentParameter(2, nameof(EditForm.FormName), "async");
        builder.AddComponentParameter(3, nameof(EditForm.ChildContent), (RenderFragment<EditContext>)(_ => b =>
        {
            b.OpenComponent<DataAnnotationsValidator>(0);
            b.CloseComponent();
            b.OpenComponent<RzInitialValidator>(1);
            b.CloseComponent();
            b.OpenComponent<RzValidationMessageBase<string?>>(2);
            b.AddComponentParameter(3, "For", (Expression<Func<string?>>)(() => _model.Username));
            b.CloseComponent();
        }));
        builder.CloseComponent();
    }
}
#endif
