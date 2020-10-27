using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.WebApi.Filters;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class DockersController : ControllerBase
    {
        private readonly IDockerService dockerService;

        public DockersController(IDockerService imageService)
        {
            this.dockerService = imageService;
        }

        [HttpGet("Images"), Log]
        public OperationResult<IEnumerable<ImagesListResponse>> GetAllImages()
        {
            return new OperationResult<IEnumerable<ImagesListResponse>>(dockerService.GetAllImage());
        }

        [HttpGet("Containers"), Log]
        public OperationResult<IEnumerable<ContainerListResponse>> GetAllContainers()
        {
            return new OperationResult<IEnumerable<ContainerListResponse>>(dockerService.GetAllContainer());
        }

        [HttpGet("Commands/{id}/Log"), Log]
        public OperationResult<string> GetCommandLog(string id)
        {
            return new OperationResult<string>(dockerService.GetExecCommandContainerLog(id));
        }
    }
}
