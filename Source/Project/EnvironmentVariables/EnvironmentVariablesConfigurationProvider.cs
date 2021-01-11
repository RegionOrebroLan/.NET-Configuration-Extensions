using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using RegionOrebroLan.Configuration.Extensions;

namespace RegionOrebroLan.Configuration.EnvironmentVariables
{
	[CLSCompliant(false)]
	public class EnvironmentVariablesConfigurationProvider : Microsoft.Extensions.Configuration.EnvironmentVariables.EnvironmentVariablesConfigurationProvider
	{
		#region Fields

		private const string _appSettingsJsonKey = "AppSettings_json";
		private const string _keyDelimiter = "__";

		#endregion

		#region Constructors

		public EnvironmentVariablesConfigurationProvider(string prefix) : base(prefix) { }

		#endregion

		#region Properties

		public virtual string AppSettingsJsonKey => _appSettingsJsonKey;
		protected internal virtual JsonConfigurationProvider JsonConfigurationProvider { get; } = new JsonConfigurationProvider(new JsonConfigurationSource());

		#endregion

		#region Methods

		protected internal virtual IDictionary<string, string> CreateData(string value)
		{
			using(var stream = this.CreateStream(value))
			{
				this.JsonConfigurationProvider.Load(stream);

				return this.JsonConfigurationProvider.ToDictionary();
			}
		}

		protected internal virtual Stream CreateStream(string value)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(value));
		}

		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		public override void Load()
		{
			base.Load();

			var appSettings = this.Data.Where(item => item.Key.StartsWith(this.AppSettingsJsonKey, StringComparison.OrdinalIgnoreCase)).ToList();

			foreach(var item in appSettings)
			{
				this.Data.Remove(item.Key);
			}

			var data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			foreach(var item in this.Data)
			{
				data.Add(item.Key, item.Value);
			}

			foreach(var item in appSettings)
			{
				try
				{
					foreach(var entry in this.CreateData(item.Value))
					{
						var key = entry.Key.Replace(ConfigurationPath.KeyDelimiter, _keyDelimiter);

						if(data.ContainsKey(key))
							continue;

						data.Add(key, entry.Value);
					}
				}
				catch
				{
					data.Add(item.Key, item.Value);
				}
			}

			this.Data = data;
		}

		#endregion
	}
}