using Chargoon.ContainerManagement.Domain.Dtos.TemplateCommands;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Instances
{
    public class InstanceSignalDto
    {
        public int InstanceId { get; set; }
        public IEnumerable<SwarmService> Services { get; set; }
        public IEnumerable<ContainerListResponse> Containers { get; set; }
        public IEnumerable<TemplateCommandExecDto> TemplateCommandExecs { get; set; }
    }
}
