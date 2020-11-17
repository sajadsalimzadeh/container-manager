using Chargoon.ContainerManagement.Domain.Models;
using Chargoon.ContainerManagement.Domain.Services;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chargoon.ContainerManagement.WebApi.Helper
{
	public static class RegisterHangfire
	{
		private static AppSettings appSettings;
		private static IServiceCollection services;
		private static IConfiguration configuration;

		public static void AddHangfire(this IServiceCollection services)
		{
			var provider = services.BuildServiceProvider();
			configuration = provider.GetService<IConfiguration>();

			services.AddHangfire(c =>
			{
				c.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
					.UseSimpleAssemblyNameTypeSerializer()
					.UseRecommendedSerializerSettings()
					.UseSqlServerStorage(configuration.GetConnectionString("Hangfire"), new SqlServerStorageOptions
					{
						CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
						SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
						QueuePollInterval = TimeSpan.Zero,
						UseRecommendedIsolationLevel = true,
						DisableGlobalLocks = true
					});
			});

			// Add the processing server as IHostedService
			services.AddHangfireServer();

			RegisterHangfire.services = services;
		}

		public static void UseHangfire(this IApplicationBuilder app)
		{
			app.UseHangfireDashboard();

			var provider = services.BuildServiceProvider();
			appSettings = provider.GetService<IOptions<AppSettings>>().Value;
			var imageService = provider.GetService<IImageService>();
			var templateService = provider.GetService<ITemplateService>();
			var logger = provider.GetService<ILoggerService>();

			RecurringJob.AddOrUpdate("DockerSystemPrune", () => DockerSystemPrune(), appSettings.Hangfire.DockerSystemPruneCron);
			RecurringJob.AddOrUpdate("DockerClearExitedCommandCache", () => DockerClearExitedCommandCache(), appSettings.Hangfire.DockerClearExitedCommandCacheCron);
			RecurringJob.AddOrUpdate("ClearLogs", () => ClearLogs(), appSettings.Hangfire.ClearLogCron);
			RecurringJob.AddOrUpdate("ClearImageBuildLogs", () => ClearImageBuildLogs(), appSettings.Hangfire.ClearImageBuildLogCron);

			var existsRecurringJobIds = new List<string>();
			foreach (var image in imageService.GetAll())
			{
				var scheduleId = $"ImageBuild-{image.Id}-{image.Name}";
				if (!string.IsNullOrEmpty(image.BuildCron))
				{
					try
					{
						RecurringJob.AddOrUpdate(scheduleId, () => BuildImage(image.Id), image.BuildCron);
					}
					catch (Exception ex)
					{
						logger.LogError(ex);
					}
				}
				else
				{
					RecurringJob.RemoveIfExists(scheduleId);
				}
				existsRecurringJobIds.Add(scheduleId);
			}


			foreach (var template in templateService.GetAll())
			{
				var scheduleId = $"TemplateInsert-{template.Id}-{template.Name}";
				if (!string.IsNullOrEmpty(template.InsertCron))
				{
					try
					{
						RecurringJob.AddOrUpdate(scheduleId, () => InsertTemplate(template.Id), template.InsertCron);
					}
					catch (Exception ex)
					{
						logger.LogError(ex);
					}
				}
				else
				{
					RecurringJob.RemoveIfExists(scheduleId);
				}
				existsRecurringJobIds.Add(scheduleId);
			}

			using (var connection = JobStorage.Current.GetConnection())
			{
				var recurringJobs = connection.GetRecurringJobs();
				var notExistsRecurringJobs = recurringJobs.Where(x =>
					!existsRecurringJobIds.Contains(x.Id) &&
					(x.Id.StartsWith("TemplateInsert") || x.Id.StartsWith("ImageBuild"))
				).ToList();
				foreach (var recuringJob in notExistsRecurringJobs)
				{
					RecurringJob.RemoveIfExists(recuringJob.Id);
				}
			}
		}

		public static void DockerSystemPrune()
		{
			var logger = services.BuildServiceProvider().GetService<ILoggerService>();
			try
			{
				var dockerService = services.BuildServiceProvider().GetService<IDockerService>();
				logger.LogInformation($"Hangfire: {nameof(DockerSystemPrune)} Start");
				dockerService.SystemPrune();
				logger.LogInformation($"Hangfire: {nameof(DockerSystemPrune)} End");
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
			}
		}

		public static void DockerClearExitedCommandCache()
		{
			var logger = services.BuildServiceProvider().GetService<ILoggerService>();
			try
			{
				var dockerService = services.BuildServiceProvider().GetService<IDockerService>();
				logger.LogInformation($"Hangfire: {nameof(DockerClearExitedCommandCache)} Start");
				dockerService.ClearExitedCommands();
				logger.LogInformation($"Hangfire: {nameof(DockerClearExitedCommandCache)} End");
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
			}
		}

		public static void ClearLogs()
		{
			var logger = services.BuildServiceProvider().GetService<ILoggerService>();
			try
			{
				var datetime = DateTime.Now.AddDays(-appSettings.Logging.Lifetime);
				logger.LogInformation($"Hangfire: {nameof(ClearLogs)} Start");
				logger.ClearBefore(datetime);
				logger.LogInformation($"Hangfire: {nameof(ClearLogs)} End");
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
			}
		}

		public static void ClearImageBuildLogs()
		{
			var logger = services.BuildServiceProvider().GetService<ILoggerService>();
			try
			{
				var imageService = services.BuildServiceProvider().GetService<IImageService>();
				var datetime = DateTime.Now.AddDays(-appSettings.Image.BuildLogLifetime);
				logger.LogInformation($"Hangfire: {nameof(ClearLogs)} Start");
				imageService.ClearBuildLogBefore(datetime);
				logger.LogInformation($"Hangfire: {nameof(ClearLogs)} End");
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
			}
		}

		public static void BuildImage(int imageId)
		{
			var logger = services.BuildServiceProvider().GetService<ILoggerService>();
			try
			{
				var imageService = services.BuildServiceProvider().GetService<IImageService>();
				var image = imageService.Get(imageId);
				if (image != null)
				{
					logger.LogInformation($"Hangfire: {nameof(BuildImage)} Start");
					imageService.Build(imageId);
					logger.LogInformation($"Hangfire: {nameof(BuildImage)} End");
				}
				else
				{
					logger.LogInformation($"Hangfire: image not found");

				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
			}
		}

		public static void InsertTemplate(int templateId)
		{
			var logger = services.BuildServiceProvider().GetService<ILoggerService>();
			try
			{
				var templateService = services.BuildServiceProvider().GetService<ITemplateService>();
				logger.LogInformation($"Hangfire: {nameof(InsertTemplate)} Start");
				templateService.DupplicateFrom(templateId);
				logger.LogInformation($"Hangfire: {nameof(InsertTemplate)} End");
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
			}
		}
	}
}
