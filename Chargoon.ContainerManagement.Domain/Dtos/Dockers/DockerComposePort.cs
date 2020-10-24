using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposePort
    {
        public int Container { get; set; }
        public int Host { get; set; }
    }
}
