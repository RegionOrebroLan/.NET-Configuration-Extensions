using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace RegionOrebroLan.Configuration.Extensions
{
	[CLSCompliant(false)]
	public static class ConfigurationProviderExtension
	{
		#region Methods

		internal static void PopulateDictionary(this IConfigurationProvider configurationProvider, IDictionary<string, string> dictionary, string parent)
		{
			if(configurationProvider == null)
				throw new ArgumentNullException(nameof(configurationProvider));

			if(dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			var keys = configurationProvider.GetChildKeys(Enumerable.Empty<string>(), parent).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

			if(keys.Any())
			{
				foreach(var key in keys)
				{
					configurationProvider.PopulateDictionary(dictionary, parent != null ? ConfigurationPath.Combine(parent, key) : key);
				}
			}
			else if(parent != null && configurationProvider.TryGet(parent, out var value))
			{
				dictionary[parent] = value;
			}
		}

		public static IDictionary<string, string> ToDictionary(this IConfigurationProvider configurationProvider)
		{
			if(configurationProvider == null)
				throw new ArgumentNullException(nameof(configurationProvider));

			var dictionary = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			configurationProvider.PopulateDictionary(dictionary, null);

			return dictionary;
		}

		#endregion
	}
}