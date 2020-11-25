using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Customs;
using Chargoon.ContainerManagement.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chargoon.ContainerManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CustomController : ControllerBase
    {
        private readonly ICustomService customeService;

        public CustomController(ICustomService customeService)
        {
            this.customeService = customeService;
        }

        [HttpGet("NightlyBuildLogs/{branch}/{date}")]
        public OperationResult<IEnumerable<CustomNightlyBuildLogDto>> GetNightlyBuildLogs(string branch, DateTime date)
        {
            return new OperationResult<IEnumerable<CustomNightlyBuildLogDto>>(customeService.GetNightlyBuildLogs(branch, date));
        }

        [HttpGet("NightlyBuildLogs/{branch}/{date}/{filename}")]
        public FileContentResult GetNightlyBuildLogs(string branch, DateTime date, string filename)
        {
            var result = customeService.GetNightlyBuildLogStream(branch, date, filename);
            var bytes = new byte[result.Length];
            result.Read(bytes, 0, bytes.Length);
            return new FileContentResult(bytes, "text/html; charset=UTF-8");
        }
    }
}
