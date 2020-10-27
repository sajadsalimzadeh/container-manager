using Chargoon.ContainerManagement.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerCompose
    {
        [Yaml(WithQuotes = true)]
        public string Version { get; set; } = "3.8";
        public Dictionary<string, DockerComposeService> Services { get; set; }
        public Dictionary<string, DockerComposeNetwork> Networks { get; set; }
        public Dictionary<string, DockerComposeVolume> Volumes { get; set; }
    }
}
