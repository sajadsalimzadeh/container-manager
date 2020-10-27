using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Chargoon.ContainerManagement.Domain.Dtos.TemplateCommands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Templates
{
    public class TemplateGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DockerCompose DockerCompose { get; set; }
        public Dictionary<string, string> Environments { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<TemplateCommandGetDto> Commands { get; set; }
    }
}
