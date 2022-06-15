using System;

namespace RegionOrebroLan.Configuration
{
	/// <summary>
	/// Used to monitor configuration changes.
	/// </summary>
	public interface IConfigurationMonitor
	{
		#region Events

		event EventHandler<ConfigurationChangedEventArgs> Changed;

		#endregion
	}
}