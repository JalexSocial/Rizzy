using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Rizzy.Framework.Endpoints;

/// <summary>
/// An <see cref="IResult"/> that renders a Razor Component.
/// </summary>
public class RzRazorComponentResult<TComponent>
    : RzRazorComponentResult where TComponent : IComponent
{
    /// <summary>
    /// Constructs an instance of <see cref="RzRazorComponentResult"/>.
    /// </summary>
    public RzRazorComponentResult() : base(typeof(TComponent))
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="RzRazorComponentResult"/>.
    /// </summary>
    /// <param name="parameters">Parameters for the component.</param>
    public RzRazorComponentResult(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] object parameters) : base(typeof(TComponent), parameters)
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="RzRazorComponentResult"/>.
    /// </summary>
    /// <param name="parameters">Parameters for the component.</param>
    public RzRazorComponentResult(IReadOnlyDictionary<string, object?> parameters) : base(typeof(TComponent), parameters)
    {
    }
}