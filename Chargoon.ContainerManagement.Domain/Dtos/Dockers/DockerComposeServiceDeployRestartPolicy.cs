using Chargoon.ContainerManagement.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceDeployRestartPolicy
    {
        public DockerComposeCondition Condition { get; set; }
        public string Deley { get; set; }
        public int MaxAttemps { get; set; }
        public string Window { get; set; }
    }
}
