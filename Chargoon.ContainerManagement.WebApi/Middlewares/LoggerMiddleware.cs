using Chargoon.ContainerManagement.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chargoon.ContainerManagement.WebApi.Middleware
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerService logger;

        public LoggerMiddleware(RequestDelegate next, ILoggerService logger)
        {
            _next = next;
            this.logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            var Query = request.Query.ToDictionary(x => x.Key, x => x.Value);
            var Header = request.Headers.ToDictionary(x => x.Key, x => x.Value);
            logger.LogInformation(request.GetDisplayUrl(), new { Query, Header });
            await _next(context);
        }
    }
}
