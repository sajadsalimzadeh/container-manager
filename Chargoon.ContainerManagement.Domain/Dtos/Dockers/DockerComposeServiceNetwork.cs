using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeServiceNetwork
    {
        public string aliases { get; set; }
        public string ipv4Address { get; set; }
        public string ipv6Address { get; set; }
    }
}
