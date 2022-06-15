using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RegionOrebroLan.Configuration.Internal;

namespace RegionOrebroLan.Configuration.DependencyInjection.Extensions
{
	public static class ServiceCollectionExtension
	{
		#region Methods

		public static IServiceCollection AddConfigurationMonitor(this IServiceCollection services, IConfiguration configuration)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			services.TryAddSingleton<IConfigurationMonitor, ConfigurationMonitor>();
			services.TryAddSingleton<ISystemClock, SystemClock>();

			return services;
		}

		#endregion
	}
}