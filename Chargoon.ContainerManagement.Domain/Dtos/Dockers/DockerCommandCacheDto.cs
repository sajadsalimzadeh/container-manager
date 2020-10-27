using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Dockers
{
    public class DockerCommandCacheDto
    {
        public string CommandId { get; set; }
        public string ContainerId { get; set; }
        public string Output { get; set; }
    }
}
