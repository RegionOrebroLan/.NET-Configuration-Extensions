using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Configuration.Extensions;

namespace IntegrationTests.Extensions
{
	[TestClass]
	public class ConfigurationProviderExtensionTest
	{
		#region Methods

		protected internal virtual async Task<IConfigurationBuilder> CreateConfigurationBuilderAsync(string fileName)
		{
			var jsonFilePath = Path.Combine(Global.ProjectPath, "Extensions", "Resources", "ConfigurationProviderExtension", $"{fileName}.json");

			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.SetBasePath(Global.ProjectPath);

			configurationBuilder.AddJsonFile(jsonFilePath, false, false);

			return await Task.FromResult(configurationBuilder);
		}

		protected internal virtual async Task<IDictionary<string, string>> GetInternalDictionaryAsync(IConfigurationProvider configurationProvider)
		{
			if(configurationProvider == null)
				throw new ArgumentNullException(nameof(configurationProvider));

			if(!(configurationProvider is ConfigurationProvider provider))
				throw new ArgumentException($"The configuration-provider is not of type \"{typeof(ConfigurationProvider)}\".", nameof(configurationProvider));

			// ReSharper disable PossibleNullReferenceException

			var dictionary = (IDictionary)provider.GetType().GetProperty("Data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(provider);

			var typedDictionary = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			foreach(DictionaryEntry entry in dictionary)
			{
				typedDictionary.Add((string)entry.Key, (string)entry.Value);
			}

			// ReSharper restore PossibleNullReferenceException

			return await Task.FromResult(typedDictionary);
		}

		[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
		protected internal virtual async Task Test(IDictionary<string, string> dictionary)
		{
			await Task.CompletedTask;

			Assert.IsNotNull(dictionary);
			Assert.AreEqual(3, dictionary.Count);
			Assert.AreEqual("A1:B1:C1:D1:E1:F1", dictionary.ElementAt(0).Key);
			Assert.AreEqual("f-value", dictionary.ElementAt(0).Value);
			Assert.AreEqual("A2:B2:C2", dictionary.ElementAt(1).Key);
			Assert.AreEqual(string.Empty, dictionary.ElementAt(1).Value);
			Assert.AreEqual("A3:B3:C3", dictionary.ElementAt(2).Key);
			Assert.AreEqual("c-value", dictionary.ElementAt(2).Value);
		}

		[TestMethod]
		public async Task Test()
		{
			var configurationBuilder = await this.CreateConfigurationBuilderAsync("Default");
			var configuration = configurationBuilder.Build();
			var provider = configuration.Providers.ElementAt(0);
			var dictionary = provider.ToDictionary();
			await this.Test(dictionary);

			var jsonConfigurationProvider = (JsonConfigurationProvider)provider;
			var internalDictionary = await this.GetInternalDictionaryAsync(jsonConfigurationProvider);
			await this.Test(internalDictionary);

			configurationBuilder = await this.CreateConfigurationBuilderAsync("Empty");
			configuration = configurationBuilder.Build();
			provider = configuration.Providers.ElementAt(0);
			dictionary = provider.ToDictionary();
			Assert.IsFalse(dictionary.Any());
		}

		#endregion
	}
}