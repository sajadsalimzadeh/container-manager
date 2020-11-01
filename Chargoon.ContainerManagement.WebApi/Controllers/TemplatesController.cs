using System.Collections.Generic;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Templates;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.WebApi.Filters;
using Chargoon.ContainerManagement.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateService templateService;

        public TemplatesController(ITemplateService templateService)
        {
            this.templateService = templateService;
        }

        [HttpGet, Auth, Log]
        public OperationResult<IEnumerable<TemplateGetDto>> GetAll()
        {
            return new OperationResult<IEnumerable<TemplateGetDto>>(templateService.GetAll());
        }

        [HttpPost, Auth("Admin"), Log]
        public OperationResult<TemplateGetDto> Add([FromBody] TemplateAddDto dto)
        {
            return new OperationResult<TemplateGetDto>(templateService.Add(dto));
        }

        [HttpPatch("{id:int}"), Auth("Admin")]
        public OperationResult<TemplateGetDto> Change(int id, [FromBody] TemplateChangeDto dto)
        {
            return new OperationResult<TemplateGetDto>(templateService.Change(id, dto));
        }

        [HttpPut("{id:int}/Dupplicate"), Auth("Admin")]
        public OperationResult<TemplateGetDto> Dupplicate(int id)
        {
            return new OperationResult<TemplateGetDto>(templateService.DupplicateFrom(id));
        }

        [HttpDelete("{id:int}"), Auth("Admin")]
        public OperationResult<TemplateGetDto> Remove(int id)
        {
            return new OperationResult<TemplateGetDto>(templateService.Remove(id));
        }
    }
}
