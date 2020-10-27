using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.TemplateCommands
{
    public class TemplateCommandExecDto
    {
        public int InstanceId { get; set; }
        public int TemplateId { get; set; }
        public int TemplateCommandId { get; set; }
        public string CommandId { get; set; }
        public ContainerExecInspectResponse Inspect { get; set; }
    }
}
