using System;
using Microsoft.Extensions.Configuration;

namespace RegionOrebroLan.Configuration.EnvironmentVariables
{
	[CLSCompliant(false)]
	public class EnvironmentVariablesConfigurationSource : IConfigurationSource
	{
		#region Properties

		/// <summary>A prefix used to filter environment variables.</summary>
		public virtual string Prefix { get; set; }

		#endregion

		#region Methods

		public virtual IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new EnvironmentVariablesConfigurationProvider(this.Prefix);
		}

		#endregion
	}
}