using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstancesController : ControllerBase
    {
        private readonly IInstanceService instanceService;

        public InstancesController(IInstanceService instanceService)
        {
            this.instanceService = instanceService;
        }

        [HttpPost("{id:int}/Start-Container")]
        public OperationResult StartContainer()
        {

        }
        [HttpPost("{id:int}/Stop-Container")]
        public OperationResult StopContainer()
        {

        }
        [HttpPost("{id:int}/Start-AppPools")]
        public OperationResult StartAppPools()
        {

        }
        [HttpPost("{id:int}/Stop-AppPools")]
        public OperationResult StopAppPools()
        {

        }
        [HttpPost("{id:int}/Restart-AppPools")]
        public OperationResult RestartAppPools()
        {

        }
        [HttpPost("{id:int}/Run-FileManager")]
        public OperationResult RunFileManager()
        {

        }
        [HttpPost("{id:int}/Update-Database")]
        public OperationResult UpdateDatabase()
        {

        }
        [HttpPatch("{id:int}/Image")]
        public OperationResult ChangeImage()
        {

        }
        [HttpPatch("{id:int}/Type")]
        public OperationResult ChangeType()
        {

        }
    }
}
