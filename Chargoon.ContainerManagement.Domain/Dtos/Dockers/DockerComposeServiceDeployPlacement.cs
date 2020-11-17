using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceDeployPlacement
    {
        public int maxReplicasPerNode { get; set; } = 1;
    }
}
