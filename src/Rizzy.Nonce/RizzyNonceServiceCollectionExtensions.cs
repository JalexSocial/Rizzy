using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rizzy.Nonce;

/// <summary>
/// Provides extension methods for registering nonce-related services.
/// </summary>
public static class RizzyNonceServiceCollectionExtensions
{
	/// <summary>
	/// Registers the IRizzyNonceProvider with its default implementation (<see cref="RizzyNonceProvider"/>)
	/// and allows additional configuration of <see cref="NonceOptions"/>.
	/// </summary>
	/// <param name="services">The service collection to register the nonce provider in.</param>
	/// <param name="configure">An optional action to configure the <see cref="NonceOptions"/>.</param>
	/// <returns>The updated service collection.</returns>
	public static IServiceCollection AddRizzyNonceProvider(
		this IServiceCollection services,
		Action<NonceOptions>? configure = null)
	{
		if (configure != null)
		{
			// Configure NonceOptions with the supplied delegate.
			services.Configure(configure);
		}
		else
		{
			services.Configure<NonceOptions>(options => { });
        }

		// Register HttpContextAccessor.
		services.AddHttpContextAccessor();

        services.TryAddSingleton<RizzyNonceGenerator>();
        services.TryAddScoped<IRizzyNonceProvider, RizzyNonceProvider>();

		return services;
	}
}
