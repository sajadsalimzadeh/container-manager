using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceNetwork
    {
        public string Aliases { get; set; }
        public string Ipv4Address { get; set; }
        public string Ipv6Address { get; set; }
    }
}
