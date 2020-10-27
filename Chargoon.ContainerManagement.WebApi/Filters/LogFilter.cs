using Chargoon.ContainerManagement.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chargoon.ContainerManagement.WebApi.Filters
{
    public class LogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILoggerService>();

            if (logger != null)
            {
                var request = context.HttpContext.Request;
                var Query = request.Query.ToDictionary(x => x.Key, x => x.Value);
                var Header = request.Headers.ToDictionary(x => x.Key, x => x.Value);
                logger.LogInformation(request.GetDisplayUrl(), new { Query, Header });
            }
        }
    }
}
