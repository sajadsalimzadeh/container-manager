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
        public string Description { get; set; }
        public string DockerCompose { get; set; }
        public string Environments { get; set; }
        public string InsertCron { get; set; }
		public int? InsertLifeTime { get; set; }
		public DateTime? ExpireTime { get; set; }
		public string BeforeStartCommand { get; set; }
		public string AfterStartCommand { get; set; }
		public string BeforeStopCommand { get; set; }
		public string AfterStopCommand { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }

        public IEnumerable<TemplateCommand> Commands { get; set; }
    }
}
