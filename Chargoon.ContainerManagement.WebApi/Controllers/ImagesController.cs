using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Images;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.WebApi.Filters;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService imageService;

        public ImagesController(IImageService ImageService)
        {
            this.imageService = ImageService;
        }

        [HttpGet(""), Log]
        public OperationResult<IEnumerable<ImageGetDto>> GetAll()
        {
            return new OperationResult<IEnumerable<ImageGetDto>>(imageService.GetAll());
        }

        [HttpGet("{id:int}"), Log]
        public OperationResult<ImageGetDto> Get(int id)
        {
            return new OperationResult<ImageGetDto>(imageService.Get(id));
        }

        [HttpPost(""), Log]
        public OperationResult<ImageGetDto> Add([FromBody] ImageAddDto dto)
        {
            return new OperationResult<ImageGetDto>(imageService.Add(dto));
        }

        [HttpPatch("{id:int}"), Log]
        public OperationResult<ImageGetDto> Change(int id, [FromBody] ImageChangeDto dto)
        {
            return new OperationResult<ImageGetDto>(imageService.Change(id, dto));
        }

        [HttpDelete("{id:int}"), Log]
        public OperationResult<ImageGetDto> Remove(int id)
        {
            return new OperationResult<ImageGetDto>(imageService.Remove(id));
        }

        [HttpGet("{id:int}/BuildLogs"), Log]
        public OperationResult<IEnumerable<ImageBuildLogDto>> GetAllBuildLogs(int id)
        {
            return new OperationResult<IEnumerable<ImageBuildLogDto>>(imageService.GetAllBuildLog(id));
        }

        [HttpGet("{id:int}/BuildLogs/{buildname}/{filename}")]
        public FileStreamResult GetBuildLog(int id, string buildname, string filename)
        {
            return File(imageService.GetBuildLog(id, buildname, filename), "application/octet-stream");
        }
    }
}
