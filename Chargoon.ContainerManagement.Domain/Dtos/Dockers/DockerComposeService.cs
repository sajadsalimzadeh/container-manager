using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerComposeService
    {
        public string image { get; set; }
        public DockerComposeServiceDeploy Deploy { get; set; }
        public IEnumerable<string> env_file { get; set; }
        public IEnumerable<string> environment { get; set; }
        public IEnumerable<string> ports { get; set; }
        public Dictionary<string, DockerComposeServiceNetwork> networks { get; set; }
        public string restart { get; set; }
        public IEnumerable<string> depends_on { get; set; }
        public IEnumerable<string> volumes { get; set; }
    }
}
