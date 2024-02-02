using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Rizzy.Antiforgery;
using Rizzy.Configuration;
using Rizzy.Http;

namespace Rizzy;

/// <summary>
/// This class has extension methods for <see cref="IHostApplicationBuilder"/> and <see cref="IApplicationBuilder"/> 
/// that enable configuration of Htmx in the application.
/// </summary>
public static class RizzyApplicationBuilderExtensions
{
	/// <summary>
	/// Add and configure Htmx.
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="configBuilder"></param>
	public static RizzyConfigBuilder AddRizzy(this IHostApplicationBuilder builder, Action<RizzyConfig> configBuilder) =>
		new RizzyConfigBuilder(builder, configBuilder);

	/// <summary>
	/// Enable Htmx to use antiforgery tokens to secure requests.
	/// </summary>
	/// <param name="builder"></param>
	/// <returns></returns>
	public static IApplicationBuilder UseRizzy(this IApplicationBuilder builder)
	{
		builder.UseMiddleware<HtmxAntiforgeryMiddleware>();
		return builder;
	}
}
