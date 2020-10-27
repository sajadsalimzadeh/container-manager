using Chargoon.ContainerManagement.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.TemplateCommands
{
    public class TemplateCommandGetDto
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public string ServiceName { get; set; }
        public TemplateCommandColor Color { get; set; }
    }
}
