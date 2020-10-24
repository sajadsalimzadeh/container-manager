using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceDeployResources
    {
        public DockerComposeServiceDeployResourcesLimits Limits { get; set; }
        public DockerComposeServiceDeployResourcesReservations Reservations { get; set; }
    }
}
