using Chargoon.ContainerManagement.WebApi.Helpers;
using Chargoon.ContainerManagement.WebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Chargoon.ContainerManagement.WebApi.Helper
{
    public static class RegisterLogger
    {
        public static void AddLogger(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSeq(configuration.GetSection("Seq"));
            });
        }

        public static void UseLogger(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggerMiddleware>();
        }

    }
}
