﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceDeployResourcesLimits
    {
        public string Cpus { get; set; }
        public string Memory { get; set; }
    }
}
