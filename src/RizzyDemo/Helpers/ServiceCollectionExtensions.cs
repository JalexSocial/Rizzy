using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace RizzyDemo.Helpers;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAntiforgeryValidation(
		this IServiceCollection services,
		Action<AntiforgeryOptions> setupAction)
	{
		var types = Assembly.Load("Microsoft.AspNetCore.Mvc.ViewFeatures")
			.GetTypes();
		var autoType = types.First(t => t.Name == "AutoValidateAntiforgeryTokenAuthorizationFilter"); // necessary for the AutoValidateAntiforgeryTokenAttribute
		var defaultType = types.First(t => t.Name == "ValidateAntiforgeryTokenAuthorizationFilter"); // necessary for the ValidateAntiforgeryTokenAttribute
		services.TryAddSingleton(autoType);
		services.TryAddSingleton(defaultType);
		
		return services;
	}

}