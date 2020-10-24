using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Services;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DockersController : ControllerBase
    {
        private readonly IDockerService dockerService;

        public DockersController(IDockerService imageService)
        {
            this.dockerService = imageService;
        }

        [HttpGet("Images")]
        public OperationResult<List<ImagesListResponse>> GetAllImages()
        {
            return OperationResult<List<ImagesListResponse>>.Succeed(dockerService.GetAllImages());
        }

        [HttpGet("Containers")]
        public OperationResult<List<ContainerListResponse>> GetAllContainers()
        {
            return OperationResult<List<ContainerListResponse>>.Succeed(dockerService.GetAllContainers());
        }
    }
}
