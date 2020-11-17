using Chargoon.ContainerManagement.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServicePort
    {
        [Yaml(IsIgnore = true)]
        public string name { get; set; }
        public string target { get; set; }
        public string published { get; set; }
        public string protocol { get; set; }
        public string mode { get; set; }
    }
}
