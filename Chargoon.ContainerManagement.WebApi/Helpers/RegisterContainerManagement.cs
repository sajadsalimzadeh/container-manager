using Chargoon.ContainerManagement.Data.Migrations;
using Chargoon.ContainerManagement.Data.Repositories;
using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Chargoon.ContainerManagement.WebApi.Helper
{
    public static class RegisterContainerManagement
    {
        public static void AddContainerManagement(this IServiceCollection services)
        {
            services.AddScoped<IStartupService, StartupService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IInstanceRepository, InstanceRepository>();
            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<ITemplateCommandRepository, TemplateCommandRepository>();

            services.AddTransient<ILoggerService, LoggerService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IDockerService, DockerService>();
            services.AddScoped<IInstanceService, InstanceService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITemplateCommandService, TemplateCommandService>();


            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            var migrator = new Migrator(configuration);
            migrator.Up();

            var logger = services.BuildServiceProvider().GetService<ILogger>();
            var startups = services.BuildServiceProvider().GetServices<IStartupService>();

            foreach (var startup in startups)
            {
                Task.Run(() =>
                {
                    try
                    {
                        startup.RunAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.Message);
                    }
                });
            }

            foreach (var startup in startups)
            {
                try
                {
                    startup.Run();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}
