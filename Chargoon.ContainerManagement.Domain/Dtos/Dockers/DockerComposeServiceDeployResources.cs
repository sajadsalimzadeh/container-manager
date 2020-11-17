using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceDeployResources
    {
        public DockerComposeServiceDeployResourcesLimits limits { get; set; }
        public DockerComposeServiceDeployResourcesReservations reservations { get; set; }
    }
}
