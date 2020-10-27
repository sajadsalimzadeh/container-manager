using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Auth;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.WebApi.Filters;
using Chargoon.ContainerManagement.WebApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("Login"), Log]
        public OperationResult<LoginResponse> Authenticate([FromBody] LoginRequest model)
        {
            var response = authenticationService.Login(model);

            if (response == null)
                return new OperationResult<LoginResponse>(false, "Username or password is incorrect");

            return new OperationResult<LoginResponse>(response);
        }

        [HttpPost("ChangeUser/{id:int}"), Auth("Admin"), Log]
        public OperationResult<LoginResponse> ChangeUser(int id)
        {
            var response = authenticationService.Login(id);
            return new OperationResult<LoginResponse>(response);
        }
    }
}
