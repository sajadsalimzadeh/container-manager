using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Templates
{
    public class TemplateChangeDto
    {
        public string Name { get; set; }
        public string InsertCron { get; set; }
        public string DockerCompose { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<string, string> Environments { get; set; }
    }
}
