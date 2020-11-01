using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.DataModels
{
    [Table(nameof(Image))]
    public class Image
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string BuildCron { get; set; }
        public string BuildPath { get; set; }
    }
}
