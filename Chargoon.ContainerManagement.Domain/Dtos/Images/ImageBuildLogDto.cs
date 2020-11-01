using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Images
{
    public class ImageBuildLogDto
    {
        public string BuildName { get; set; }
        public IEnumerable<string> Scripts { get; set; }
    }
}
