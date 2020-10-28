using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Branches
{
    public class BranchChangeDto
    {
        public string Name { get; set; }
        public DockerCompose DockerCompose { get; set; }
        public bool IsBuildEnable { get; set; }
    }
}
