using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using RegionOrebroLan.Configuration.Internal;
using RegionOrebroLan.Logging.Extensions;

namespace RegionOrebroLan.Configuration
{
	/// <inheritdoc />
	public class ConfigurationMonitor : IConfigurationMonitor
	{
		#region Constructors

		public ConfigurationMonitor(IConfiguration configuration, ILoggerFactory loggerFactory, ISystemClock sytemClock)
		{
			this.ChangeListener = ChangeToken.OnChange(() => (configuration ?? throw new ArgumentNullException(nameof(configuration))).GetReloadToken(), this.OnConfigurationChange);
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
			this.SystemClock = sytemClock ?? throw new ArgumentNullException(nameof(sytemClock));
		}

		#endregion

		#region Events

		public event EventHandler<ConfigurationChangedEventArgs> Changed;

		#endregion

		#region Properties

		protected internal virtual IDisposable ChangeListener { get; }
		protected internal virtual ILogger Logger { get; }
		protected internal virtual ISystemClock SystemClock { get; }

		#endregion

		#region Methods

		protected internal virtual void OnChanged(ConfigurationChangedEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException(nameof(e));

			this.Changed?.Invoke(this, e);
		}

		protected internal virtual void OnConfigurationChange()
		{
			this.Logger.LogDebugIfEnabled("The configuration has changed.");

			this.OnChanged(new ConfigurationChangedEventArgs
			{
				Timestamp = this.SystemClock.UtcNow
			});
		}

		#endregion

		#region Other members

		#region Finalizers

		~ConfigurationMonitor()
		{
			this.ChangeListener?.Dispose();
		}

		#endregion

		#endregion
	}
}