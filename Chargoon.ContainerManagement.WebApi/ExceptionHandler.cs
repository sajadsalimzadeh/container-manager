using Chargoon.ContainerManagement.Domain.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Chargoon.ContainerManagement.WebApi
{
    public static class ExceptionHandler
    {
        public static IApplicationBuilder AddExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>();
                    var op = new OperationResult<string>();
                    op.Code = 500;
                    op.Message = exception.Error.Message;
                    op.Data = exception.Error.StackTrace;
                    context.Response.StatusCode = op.Code;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(op));
                });
            });
            return app;
        }
    }
}
