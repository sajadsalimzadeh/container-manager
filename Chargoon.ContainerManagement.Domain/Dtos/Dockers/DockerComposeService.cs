using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeService
    {
        public string Image { get; set; }
        public DockerComposeServiceDeploy Deploy { get; set; }
        public IEnumerable<string> Env_File { get; set; }
        public IEnumerable<string> Environment { get; set; }
        public IEnumerable<DockerComposeServicePort> Ports { get; set; }
        public Dictionary<string, DockerComposeServiceNetwork> Networks { get; set; }
    }
}
