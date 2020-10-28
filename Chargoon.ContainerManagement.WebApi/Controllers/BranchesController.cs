using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Branches;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.WebApi.Filters;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService branchService;

        public BranchesController(IBranchService branchService)
        {
            this.branchService = branchService;
        }

        [HttpGet(""), Log]
        public OperationResult<IEnumerable<BranchGetDto>> GetAll()
        {
            return new OperationResult<IEnumerable<BranchGetDto>>(branchService.GetAll());
        }

        [HttpGet("{id:int}"), Log]
        public OperationResult<BranchGetDto> Get(int id)
        {
            return new OperationResult<BranchGetDto>(branchService.Get(id));
        }

        [HttpPost(""), Log]
        public OperationResult<BranchGetDto> Add([FromBody] BranchAddDto dto)
        {
            return new OperationResult<BranchGetDto>(branchService.Add(dto));
        }

        [HttpPatch("{id:int}"), Log]
        public OperationResult<BranchGetDto> Change(int id, [FromBody] BranchChangeDto dto)
        {
            return new OperationResult<BranchGetDto>(branchService.Change(id, dto));
        }
    }
}
