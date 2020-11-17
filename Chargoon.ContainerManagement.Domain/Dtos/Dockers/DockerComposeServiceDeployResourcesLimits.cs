using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceDeployResourcesLimits
    {
        public string cpus { get; set; }
        public string memory { get; set; }
    }
}
