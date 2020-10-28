using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.DataModels
{
    [Table(nameof(Branch))]
    public class Branch
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string DockerCompose { get; set; }
        public bool IsBuildEnable { get; set; }
    }
}
