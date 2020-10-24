using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chargoon.ContainerManagement.Domain.Dtos.Auth;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.WebApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [Auth]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = userService.GetAll();
            return Ok(users);
        }
    }
}
