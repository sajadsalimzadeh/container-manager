using Chargoon.ContainerManagement.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServicePort
    {
        [Yaml(IsIgnore = true)]
        public string Name { get; set; }
        public string Target { get; set; }
        public string Published { get; set; }
        public string Protocol { get; set; }
        public string Mode { get; set; }
    }
}
