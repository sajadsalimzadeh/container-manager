using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.DataModels
{
    [Table(nameof(Template))]
    public class Template
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string DockerCompose { get; set; }
        public string Environments { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<TemplateCommand> Commands { get; set; }
    }
}
