using Chargoon.ContainerManagement.Domain.Services;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chargoon.ContainerManagement.WebApi
{
    public class RegisterHangfire
    {
        private static IServiceCollection services;
        private static IConfiguration configuration;
        public static void Register(IServiceCollection services)
        {
            RegisterHangfire.services = services;
            RegisterHangfire.configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            GlobalConfiguration.Configuration.UseSqlServerStorage(configuration.GetConnectionString("Hangfire"));
        }
    }
}
