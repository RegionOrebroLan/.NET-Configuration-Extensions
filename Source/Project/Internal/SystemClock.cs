using System;

namespace RegionOrebroLan.Configuration.Internal
{
	/// <inheritdoc />
	public class SystemClock : ISystemClock
	{
		#region Properties

		public virtual DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

		#endregion
	}
}