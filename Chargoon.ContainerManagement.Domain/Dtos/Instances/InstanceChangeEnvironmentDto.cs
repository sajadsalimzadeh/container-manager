using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Instances
{
    public class InstanceChangeEnvironmentDto
    {
        public string Type { get; set; }
        public string Branch { get; set; }
        public string DatabaseUsername { get; set; }
        public string DatabasePassword { get; set; }
        public string BuildVersion { get; set; }
        public string ContainerTag { get; set; }
    }
}
