using Chargoon.ContainerManagement.Domain.Models;
using Chargoon.ContainerManagement.Domain.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System;

namespace Chargoon.ContainerManagement.WebApi.Helper
{
    public static class RegisterHangfire
    {
        private static AppSettings appSettings;
        private static IServiceCollection services;
        private static IConfiguration configuration;

        public static void AddHangfire(this IServiceCollection services)
        {
            configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            appSettings = services.BuildServiceProvider().GetService<IOptions<AppSettings>>().Value;

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

                RecurringJob.AddOrUpdate(() => DockerSystemPrune(), appSettings.Hangfire.DockerSystemPruneCron);
                RecurringJob.AddOrUpdate(() => DockerClearExitedCommandCache(), appSettings.Hangfire.DockerClearExitedCommandCacheCron);
                RecurringJob.AddOrUpdate(() => ClearLogs(), appSettings.Hangfire.ClearLogCron);
            });

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            RegisterHangfire.services = services;
        }

        public static void UseHangfire(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
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
    }
}
