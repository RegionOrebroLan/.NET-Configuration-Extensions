using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace RegionOrebroLan.Configuration.Extensions
{
	public static class ConfigurationBuilderExtension
	{
		#region Methods

		/// <summary>
		/// Replaces text in file-configuration-source paths. To get it working on Linux where file-names are case-sensitive.
		/// </summary>
		[CLSCompliant(false)]
		public static void ResolvePaths(this IConfigurationBuilder configurationBuilder, IDictionary<string, string> replacements)
		{
			if(configurationBuilder == null)
				throw new ArgumentNullException(nameof(configurationBuilder));

			if(replacements == null)
				return;

			foreach(var configurationSource in configurationBuilder.Sources)
			{
				if(!(configurationSource is FileConfigurationSource fileConfigurationSource))
					continue;

				foreach(var replacement in replacements)
				{
					fileConfigurationSource.Path = fileConfigurationSource.Path.Replace(replacement.Key, replacement.Value);
				}
			}
		}

		#endregion
	}
}