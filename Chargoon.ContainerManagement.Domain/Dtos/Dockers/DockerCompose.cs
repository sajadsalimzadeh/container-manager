using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerCompose
    {
        public List<DockerComposeService> Services { get; set; }
        public List<DockerComposeNetwork> Networks { get; set; }
        public List<DockerComposeVolume> Volumes { get; set; }
    }
}
