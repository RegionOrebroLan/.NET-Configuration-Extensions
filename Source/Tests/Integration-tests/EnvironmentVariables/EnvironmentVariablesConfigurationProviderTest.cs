using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Configuration.EnvironmentVariables;

namespace IntegrationTests.EnvironmentVariables
{
	[TestClass]
	public class EnvironmentVariablesConfigurationProviderTest
	{
		#region Methods

		[TestMethod]
		public async Task Test()
		{
			await Task.CompletedTask;

			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.Add(new Microsoft.Extensions.Configuration.EnvironmentVariables.EnvironmentVariablesConfigurationSource());
			var keysToExclude = configurationBuilder.Build()
				.AsEnumerable().Select(item => item.Key)
				.Where(key => !key.StartsWith(new EnvironmentVariablesConfigurationProvider(null).AppSettingsJsonKey, StringComparison.OrdinalIgnoreCase))
				.ToHashSet(StringComparer.OrdinalIgnoreCase);

			configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.Add(new EnvironmentVariablesConfigurationSource());
			var data = new SortedDictionary<string, string>(configurationBuilder.Build().AsEnumerable().ToDictionary(item => item.Key, item => item.Value));

			foreach(var key in keysToExclude)
			{
				data.Remove(key);
			}

			if(!data.Any())
				Assert.Fail("The test can not be run with ReSharper yet.");
			else
				Assert.AreEqual(344, data.Count);
		}

		#endregion
	}
}