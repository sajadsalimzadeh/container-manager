using Chargoon.ContainerManagement.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceVolume
    {
        public DockerComposeServiceVolumeType type { get; set; } = DockerComposeServiceVolumeType.Volume;
        public string raw { get; set; }
        public string source { get; set; }
        public string target { get; set; }
        public bool nocopy { get; set; } = true;
    }
}
