using Chargoon.ContainerManagement.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceVolume
    {
        public DockerComposeServiceVolumeType Type { get; set; } = DockerComposeServiceVolumeType.Volume;
        public string Raw { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public bool NoCopy { get; set; } = true;
    }
}
