using System;

namespace RegionOrebroLan.Configuration
{
	public class ConfigurationChangedEventArgs : EventArgs
	{
		#region Properties

		public virtual DateTimeOffset Timestamp { get; set; }

		#endregion
	}
}