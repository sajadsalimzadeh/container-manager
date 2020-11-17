using Chargoon.ContainerManagement.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.DataModels
{
    [Table(nameof(TemplateCommand))]
    public class TemplateCommand
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey(nameof(Template))]
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public string ServiceName { get; set; }
        public string Command { get; set; }
        public TemplateCommandColor Color { get; set; } = TemplateCommandColor.None;
        public bool RunOnStartup { get; set; } = false;

		public Template Template { get; set; }
    }
}
