using Chargoon.ContainerManagement.WebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chargoon.ContainerManagement.WebApi.Helpers
{
    public static class RegisterJsonWebToken
    {

        public static void UseJsonWebToken(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtMiddleware>();
        }
    }
}
