using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Configuration;
using RegionOrebroLan.Configuration.DependencyInjection.Extensions;
using RegionOrebroLan.Configuration.Internal;
using TestHelpers.Mocks.Logging;

namespace IntegrationTests
{
	[TestClass]
	public class ConfigurationMonitorTest
	{
		#region Fields

		private static readonly string _resourcesDirectoryPath = Path.Combine(Global.ProjectDirectoryPath, "Resources", nameof(ConfigurationMonitorTest));
		private static readonly DateTimeOffset _utcNow = new(2000, 1, 1, 1, 1, 1, TimeSpan.Zero);

		#endregion

		#region Methods

		[TestMethod]
		public async Task Changed_IfAConfigurationChangeIsMade_ShouldRaiseTheEvent()
		{
			DateTimeOffset? actualTimestamp = null;
			var expectedTimestamp = _utcNow;
			var temporaryTestDirectoryPath = await Global.CreateTemporaryTestDirectoryAsync();

			try
			{
				File.Copy(Path.Combine(_resourcesDirectoryPath, Global.AppSettingsFileName), Path.Combine(temporaryTestDirectoryPath, Global.AppSettingsFileName));
				Thread.Sleep(200);

				var configuration = await CreateConfigurationAsync(temporaryTestDirectoryPath, Global.AppSettingsFileName);
				var services = Global.CreateServices(configuration);
				services.AddSingleton(await this.CreateSystemClockAsync());
				services.AddConfigurationMonitor(configuration);

				await using(var serviceProvider = services.BuildServiceProvider())
				{
					var optionsMonitor = (ConfigurationMonitor)serviceProvider.GetRequiredService<IConfigurationMonitor>();
					optionsMonitor.Changed += (_, e) =>
					{
						actualTimestamp = e.Timestamp;
					};
					var loggerMock = (LoggerMock)optionsMonitor.Logger;

					Assert.AreEqual(0, loggerMock.Logs.Count);

					// Trigger change.
					File.Copy(Path.Combine(_resourcesDirectoryPath, Global.AppSettingsFileName), Path.Combine(temporaryTestDirectoryPath, Global.AppSettingsFileName), true);
					Thread.Sleep(500);

					Assert.IsNotNull(actualTimestamp);
					Assert.AreEqual(expectedTimestamp, actualTimestamp.Value);

					Assert.AreEqual(1, loggerMock.Logs.Count);
					Assert.AreEqual("The configuration has changed.", loggerMock.Logs[0].Message);
				}
			}
			finally
			{
				if(Directory.Exists(temporaryTestDirectoryPath))
					Directory.Delete(temporaryTestDirectoryPath, true);
			}
		}

		protected internal virtual async Task<IConfiguration> CreateConfigurationAsync(string directoryPath, params string[] jsonFiles)
		{
			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.SetBasePath(directoryPath);

			// ReSharper disable InvertIf
			if(jsonFiles != null)
			{
				foreach(var jsonFile in jsonFiles)
				{
					configurationBuilder.AddJsonFile(jsonFile, false, true);
				}
			}
			// ReSharper restore InvertIf

			return await Task.FromResult(configurationBuilder.Build());
		}

		protected internal virtual async Task<ISystemClock> CreateSystemClockAsync()
		{
			return await this.CreateSystemClockAsync(_utcNow);
		}

		protected internal virtual async Task<ISystemClock> CreateSystemClockAsync(DateTimeOffset utcNow)
		{
			var systemClockMock = new Mock<ISystemClock>();
			systemClockMock.Setup(systemClock => systemClock.UtcNow).Returns(utcNow);

			return await Task.FromResult(systemClockMock.Object);
		}

		#endregion
	}
}