using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Dtos.TemplateCommands;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.WebApi.Filters;
using Chargoon.ContainerManagement.WebApi.Helpers;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class InstancesController : ControllerBase
    {
        private readonly IInstanceService instanceService;
        private readonly IDockerService dockerService;

        public InstancesController(
            IInstanceService instanceService,
            IDockerService dockerService)
        {
            this.instanceService = instanceService;
            this.dockerService = dockerService;
        }

        [HttpGet(""), Auth("Admin"), Log]
        public OperationResult<IEnumerable<InstanceGetDto>> GetAll()
        {
            return new OperationResult<IEnumerable<InstanceGetDto>>(instanceService.GetAll());
        }

        [HttpPost(""), Auth("Admin"), Log]
        public OperationResult<InstanceGetDto> Add([FromBody] InstanceAddDto dto)
        {
            return new OperationResult<InstanceGetDto>(instanceService.Add(dto));
        }

        [HttpPut("{id:int}/Start"), Auth("Admin"), Log]
        public OperationResult<InstanceGetDto> Start(int id)
        {
            return new OperationResult<InstanceGetDto>(instanceService.Start(id));
        }

        [HttpPut("{id:int}/Stop"), Auth("Admin"), Log]
        public OperationResult<InstanceGetDto> Stop(int id)
        {
            return new OperationResult<InstanceGetDto>(instanceService.Stop(id));
        }

        [HttpPatch("{id:int}/ChangeTemplate"), Auth("Admin"), Log]
        public OperationResult<InstanceGetDto> ChangeTemplate(int id, [FromBody] InstanceChangeTemplateDto dto)
        {
            return new OperationResult<InstanceGetDto>(instanceService.ChangeTemplate(id, dto));
        }

        [HttpGet("Own"), Auth, Log]
        public OperationResult<IEnumerable<InstanceGetDto>> GetAllOwn()
        {
            return new OperationResult<IEnumerable<InstanceGetDto>>(instanceService.GetAllOwn());
        }

        [HttpGet("Own/{id:int}/Services"), Auth]
        public OperationResult<IEnumerable<SwarmService>> GetAllOwnService(int id)
        {
            return new OperationResult<IEnumerable<SwarmService>>(instanceService.GetAllOwnService(id));
        }

        [HttpGet("Own/{id:int}/Commands"), Auth]
        public OperationResult<IEnumerable<TemplateCommandExecDto>> GetAllOwnCommand(int id)
        {
            return new OperationResult<IEnumerable<TemplateCommandExecDto>>(instanceService.GetAllOwnCommands(id));
        }

        [HttpPatch("Own/{id:int}/ChangeTemplate"), Auth, Log]
        public OperationResult<InstanceGetDto> ChangeOwnTemplate(int id, [FromBody] InstanceChangeTemplateDto dto)
        {
            return new OperationResult<InstanceGetDto>(instanceService.ChangeOwnTemplate(id, dto));
        }

        [HttpPut("Own/{id:int}/Start"), Auth, Log]
        public OperationResult<InstanceGetDto> StartOwn(int id)
        {
            return new OperationResult<InstanceGetDto>(instanceService.StartOwn(id));
        }

        [HttpPut("Own/{id:int}/Stop"), Auth, Log]
        public OperationResult<InstanceGetDto> StopOwn(int id)
        {
            return new OperationResult<InstanceGetDto>(instanceService.StopOwn(id));
        }

        [HttpPut("Own/{id:int}/RunCommand/{templateCommandId:int}"), Auth, Log]
        public OperationResult<InstanceGetDto> RunOwnCommand(int id, int templateCommandId)
        {
            return new OperationResult<InstanceGetDto>(instanceService.RunOwnCommand(id, templateCommandId));
        }

        [HttpDelete("{id:int}"), Auth, Log]
        public OperationResult<bool> Remove(int id)
        {
            return new OperationResult<bool>(instanceService.Remove(id));
        }

        [HttpGet("Own/Signal"), Auth]
        public OperationResult<IEnumerable<InstanceSignalDto>> GetAllOwnSignal()
        {
            var result = new List<InstanceSignalDto>();
            foreach (var instance in instanceService.GetAllOwn())
            {
                var item = new InstanceSignalDto()
                {
                    InstanceId = instance.Id,
                    Services = instanceService.GetAllOwnService(instance.Id),
                    TemplateCommandExecs = instanceService.GetAllOwnCommands(instance.Id),
                };
                item.Services = item.Services.ToList().ConvertAll(x =>
                {
                    x.PreviousSpec = null;
                    x.Spec.TaskTemplate = null;
                    return x;
                });
                result.Add(item);
            }
            return new OperationResult<IEnumerable<InstanceSignalDto>>(result);
        }
    }
}
