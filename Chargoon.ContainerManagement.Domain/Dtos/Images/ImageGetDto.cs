using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Images
{
    public class ImageGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BuildCron { get; set; }
		public int? LifeTime { get; set; }
		public string BuildPath { get; set; }
    }
}
