using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceDeploy
    {
        public int Replicas { get; set; } = 1;
        public string Mode { get; set; }
        public string EndpointMode { get; set; }
        public DockerComposeServiceDeployPlacement Placement { get; set; }
        public DockerComposeServiceDeployResources Resources { get; set; }
    }
}
