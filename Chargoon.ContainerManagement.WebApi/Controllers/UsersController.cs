using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Auth;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.WebApi.Filters;
using Chargoon.ContainerManagement.WebApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService userService;
        private readonly IInstanceService instanceService;

        public UsersController(IUserService userService, IInstanceService instanceService)
        {
            this.userService = userService;
            this.instanceService = instanceService;
        }

        [HttpGet, Auth("Admin")]
        public OperationResult<IEnumerable<UserGetDto>> GetAll()
        {
            return new OperationResult<IEnumerable<UserGetDto>>(userService.GetAll());
        }

        [HttpGet("Own"), Auth]
        public OperationResult<UserGetDto> GetOwn()
        {
            return new OperationResult<UserGetDto>(userService.GetOwn());
        }

        [HttpGet("{id:int}/Instances"), Auth("Admin"), Log]
        public OperationResult<IEnumerable<InstanceGetDto>> GetAllInstances(int id)
        {
            return new OperationResult<IEnumerable<InstanceGetDto>>(instanceService.GetAllByUserId(id));
        }

        [HttpPost(""), Auth("Admin"), Log]
        public OperationResult<UserGetDto> Add([FromBody] UserAddDto dto)
        {
            return new OperationResult<UserGetDto>(userService.Add(dto));
        }

        [HttpPatch("{id:int}/ChangePassword"), Auth("Admin"), Log]
        public OperationResult<UserGetDto> ChangePassword(int id, [FromBody] UserChangePasswordDto dto)
        {
            return new OperationResult<UserGetDto>(userService.ChangePassword(id, dto));
        }

        [HttpPatch("Own/ChangePassword"), Auth, Log]
        public OperationResult<UserGetDto> ChangeOwnPassword([FromBody] UserChangePasswordDto dto)
        {
            return new OperationResult<UserGetDto>(userService.ChangeOwnPassword(dto));
        }
    }
}
