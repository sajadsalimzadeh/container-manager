using Chargoon.ContainerManagement.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerCompose
    {
        public string version { get; set; } = "3.8";
        public Dictionary<string, DockerComposeService> services { get; set; }
        public Dictionary<string, DockerComposeNetwork> networks { get; set; }
        public Dictionary<string, DockerComposeVolume> volumes { get; set; }
    }
}
