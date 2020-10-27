using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Chargoon.ContainerManagement.WebApi.Helper
{
    public static class RegisterExceptionHandler
    {
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var logger = app.ApplicationServices.GetService<ILoggerService>();
                    var exception = context.Features.Get<IExceptionHandlerFeature>();
                    logger.LogError(exception.Error);
                    var op = new OperationResult<string>();
                    op.Code = 500;
                    op.Success = false;
                    op.Message = exception.Error.Message;
                    op.Data = exception.Error.StackTrace;
                    context.Response.StatusCode = op.Code;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(op, new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                });
            });
            return app;
        }
    }
}
