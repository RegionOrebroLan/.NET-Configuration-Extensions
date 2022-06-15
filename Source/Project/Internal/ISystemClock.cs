using System;

namespace RegionOrebroLan.Configuration.Internal
{
	public interface ISystemClock
	{
		#region Properties

		DateTimeOffset UtcNow { get; }

		#endregion
	}
}