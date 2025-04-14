using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Rizzy;

/// <summary>
/// Contains extension methods for configuring the application builder to use Rizzy middleware.
/// </summary>
public static class AppBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="RizzyMiddleware"/> to the application's request pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance to configure.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder UseRizzy(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<RizzyMiddleware>();
        return builder;
    }
}