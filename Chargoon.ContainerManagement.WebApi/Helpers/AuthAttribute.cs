using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chargoon.ContainerManagement.WebApi.Helpers
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthAttribute : Attribute, IAuthorizationFilter
    {
        public IEnumerable<string> Roles { get; set; }
        public AuthAttribute()
        {

        }
        public AuthAttribute(string roles)
        {
            Roles = roles.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (UserGetDto)context.HttpContext.Items["User"];
            if (user == null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else if (Roles?.Count() > 0)
            {
                var userRoles = user.Roles;
                foreach (var item in Roles)
                {
                    if (userRoles.Contains(item)) return;
                }
                context.Result = new JsonResult(new { message = "Access Denied" }) { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
    }
}
