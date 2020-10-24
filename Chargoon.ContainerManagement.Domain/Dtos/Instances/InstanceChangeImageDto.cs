using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Instances
{
    public class InstanceChangeImageDto
    {
        public string Branch { get; set; }
        public string BuildVersion { get; set; }
    }
}
