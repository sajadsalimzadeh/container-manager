using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceDeploy
    {
        public int replicas { get; set; } = 1;
        public string mode { get; set; }
        public string endpointMode { get; set; }
        public DockerComposeServiceDeployPlacement placement { get; set; }
        public DockerComposeServiceDeployResources resources { get; set; }
    }
}
