using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Customs
{
    public class CustomNightlyBuildLogDto
    {
        public string Branch { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }
}
