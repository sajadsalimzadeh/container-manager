using Chargoon.ContainerManagement.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceDeployRestartPolicy
    {
        public DockerComposeCondition condition { get; set; }
        public string deley { get; set; }
        public int maxattemps { get; set; }
        public string window { get; set; }
    }
}
