using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeService
    {
        public string Image { get; set; }
        public List<DockerComposePort> Ports { get; set; }
        public Dictionary<string, string> Environments { get; set; }
        public List<DockerComposeNetwork> Networks { get; set; }
    }
}
