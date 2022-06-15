using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using TestHelpers.Mocks.Logging;

namespace IntegrationTests
{
	public static class Global
	{
		#region Fields

		private static IConfiguration _configuration;
		private static IHostEnvironment _hostEnvironment;
		public const string AppSettingsFileName = "appsettings.json";
		public const string DefaultEnvironmentName = "Integration-test";

		// ReSharper disable PossibleNullReferenceException
		public static readonly string ProjectDirectoryPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
		// ReSharper restore PossibleNullReferenceException

		public static readonly string TestDirectoryPath = Path.Combine(ProjectDirectoryPath, "Test-directory");

		#endregion

		#region Properties

		public static IConfiguration Configuration => _configuration ??= CreateConfiguration(AppSettingsFileName);
		public static IHostEnvironment HostEnvironment => _hostEnvironment ??= CreateHostEnvironment(DefaultEnvironmentName);

		#endregion

		#region Methods

		public static IConfiguration CreateConfiguration(params string[] jsonFilePaths)
		{
			return CreateConfigurationBuilder(jsonFilePaths).Build();
		}

		public static IConfigurationBuilder CreateConfigurationBuilder(params string[] jsonFilePaths)
		{
			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.SetFileProvider(HostEnvironment.ContentRootFileProvider);

			foreach(var path in jsonFilePaths ?? Array.Empty<string>())
			{
				configurationBuilder.AddJsonFile(path, false, true);
			}

			return configurationBuilder;
		}

		public static IHostEnvironment CreateHostEnvironment(string environmentName)
		{
			return new HostingEnvironment
			{
				ApplicationName = typeof(Global).Assembly.GetName().Name,
				ContentRootFileProvider = new PhysicalFileProvider(ProjectDirectoryPath),
				ContentRootPath = ProjectDirectoryPath,
				EnvironmentName = environmentName
			};
		}

		public static IServiceCollection CreateServices()
		{
			return CreateServices(Configuration);
		}

		public static IServiceCollection CreateServices(IConfiguration configuration)
		{
			var services = new ServiceCollection();

			services.AddSingleton(configuration);
			services.AddSingleton(HostEnvironment);
			services.AddSingleton<ILoggerFactory, LoggerFactoryMock>();
			services.AddSingleton<LoggerFactory>();
			services.AddLogging(loggingBuilder =>
			{
				loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
				loggingBuilder.AddConsole();
				loggingBuilder.AddDebug();
				loggingBuilder.AddEventSourceLogger();
				loggingBuilder.Configure(options => { options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId | ActivityTrackingOptions.TraceId | ActivityTrackingOptions.ParentId; });
			});

			return services;
		}

		public static async Task<string> CreateTemporaryTestDirectoryAsync()
		{
			var temporaryTestDirectoryPath = Path.Combine(TestDirectoryPath, $"{Guid.NewGuid()}");

			Directory.CreateDirectory(temporaryTestDirectoryPath);

			return await Task.FromResult(temporaryTestDirectoryPath);
		}

		#endregion
	}
}