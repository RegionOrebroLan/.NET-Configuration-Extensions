using System;
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

		[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
		protected internal virtual async Task DefaultToDictionaryTest(IDictionary<string, string> dictionary)
		{
			await Task.CompletedTask;

			Assert.IsNotNull(dictionary);
			Assert.AreEqual(7, dictionary.Count);
			Assert.AreEqual("A:A:A", dictionary.ElementAt(0).Key);
			Assert.AreEqual("A-value", dictionary.ElementAt(0).Value);
			Assert.AreEqual("A:B:A", dictionary.ElementAt(1).Key);
			Assert.AreEqual("A-value", dictionary.ElementAt(1).Value);
			Assert.AreEqual("A:C:A", dictionary.ElementAt(2).Key);
			Assert.AreEqual(string.Empty, dictionary.ElementAt(2).Value);
			Assert.AreEqual("B:A:A", dictionary.ElementAt(3).Key);
			Assert.AreEqual("A-value", dictionary.ElementAt(3).Value);
			Assert.AreEqual("C:A:A:A:A:A", dictionary.ElementAt(4).Key);
			Assert.AreEqual("A-value", dictionary.ElementAt(4).Value);
			Assert.AreEqual("C:A:A:A:A:B", dictionary.ElementAt(5).Key);
			Assert.AreEqual("B-value", dictionary.ElementAt(5).Value);
			Assert.AreEqual("C:A:A:A:A:C", dictionary.ElementAt(6).Key);
			Assert.AreEqual("C-value", dictionary.ElementAt(6).Value);
		}

		[TestMethod]
		public async Task GetChildKeys_Prerequisite_Test()
		{
			var configurationBuilder = await this.CreateConfigurationBuilderAsync("Default");
			var configuration = configurationBuilder.Build();
			var provider = configuration.Providers.ElementAt(0);
			var childKeys = provider.GetChildKeys(Enumerable.Empty<string>(), null).ToArray();
			Assert.AreEqual(7, childKeys.Length);
			Assert.AreEqual("A", childKeys.ElementAt(0));
			Assert.AreEqual("A", childKeys.ElementAt(1));
			Assert.AreEqual("A", childKeys.ElementAt(2));
			Assert.AreEqual("B", childKeys.ElementAt(3));
			Assert.AreEqual("C", childKeys.ElementAt(4));
			Assert.AreEqual("C", childKeys.ElementAt(5));
			Assert.AreEqual("C", childKeys.ElementAt(6));
			Assert.AreEqual(3, childKeys.Count(key => key.Equals("A", StringComparison.Ordinal)));
			Assert.AreEqual(1, childKeys.Count(key => key.Equals("B", StringComparison.Ordinal)));
			Assert.AreEqual(3, childKeys.Count(key => key.Equals("C", StringComparison.Ordinal)));

			configurationBuilder = await this.CreateConfigurationBuilderAsync("Complex");
			configuration = configurationBuilder.Build();
			provider = configuration.Providers.ElementAt(0);
			childKeys = provider.GetChildKeys(Enumerable.Empty<string>(), null).ToArray();
			Assert.AreEqual(215, childKeys.Length);
		}

		protected internal virtual async Task<IDictionary<string, string>> GetInternalDictionaryAsync(IConfigurationProvider configurationProvider)
		{
			if(configurationProvider == null)
				throw new ArgumentNullException(nameof(configurationProvider));

			if(!(configurationProvider is ConfigurationProvider provider))
				throw new ArgumentException($"The configuration-provider is not of type \"{typeof(ConfigurationProvider)}\".", nameof(configurationProvider));

			// ReSharper disable PossibleNullReferenceException
			var dictionary = (IDictionary<string, string>)provider.GetType().GetProperty("Data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(provider);
			// ReSharper restore PossibleNullReferenceException

			return await Task.FromResult(dictionary);
		}

		[TestMethod]
		public async Task ToDictionary_Test()
		{
			var configurationBuilder = await this.CreateConfigurationBuilderAsync("Complex");
			var configuration = configurationBuilder.Build();
			var provider = configuration.Providers.ElementAt(0);
			var dictionary = provider.ToDictionary();
			Assert.AreEqual(215, dictionary.Count);

			configurationBuilder = await this.CreateConfigurationBuilderAsync("Default");
			configuration = configurationBuilder.Build();
			provider = configuration.Providers.ElementAt(0);
			dictionary = provider.ToDictionary();
			await this.DefaultToDictionaryTest(dictionary);

			var jsonConfigurationProvider = (JsonConfigurationProvider)provider;
			var internalDictionary = await this.GetInternalDictionaryAsync(jsonConfigurationProvider);
			await this.DefaultToDictionaryTest(internalDictionary);

			configurationBuilder = await this.CreateConfigurationBuilderAsync("Empty");
			configuration = configurationBuilder.Build();
			provider = configuration.Providers.ElementAt(0);
			dictionary = provider.ToDictionary();
			Assert.IsFalse(dictionary.Any());
		}

		#endregion
	}
}