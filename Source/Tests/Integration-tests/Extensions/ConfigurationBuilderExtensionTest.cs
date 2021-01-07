using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Configuration.Extensions;

namespace IntegrationTests.Extensions
{
	[TestClass]
	public class ConfigurationBuilderExtensionTest
	{
		#region Methods

		[TestMethod]
		public void Test()
		{
			var fileConfigurationProviders = ((ConfigurationRoot)Host
					.CreateDefaultBuilder(null)
					.Build()
					.Services
					.GetRequiredService<IConfiguration>())
				.Providers.OfType<FileConfigurationProvider>().ToArray();
			Assert.AreEqual(2, fileConfigurationProviders.Length);
			Assert.AreEqual("appsettings.json", fileConfigurationProviders[0].Source.Path);
			Assert.AreEqual("appsettings.Production.json", fileConfigurationProviders[1].Source.Path);

			fileConfigurationProviders = ((ConfigurationRoot)Host
					.CreateDefaultBuilder(null)
					.ConfigureAppConfiguration(configurationBuilder => configurationBuilder
						.ResolvePaths(new Dictionary<string, string> {{"appsettings", "AppSettings"}}))
					.Build()
					.Services
					.GetRequiredService<IConfiguration>())
				.Providers.OfType<FileConfigurationProvider>().ToArray();
			Assert.AreEqual(2, fileConfigurationProviders.Length);
			Assert.AreEqual("AppSettings.json", fileConfigurationProviders[0].Source.Path);
			Assert.AreEqual("AppSettings.Production.json", fileConfigurationProviders[1].Source.Path);
		}

		#endregion
	}
}