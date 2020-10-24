using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chargoon.ContainerManagement.Domain.DataModels
{
    [Table(nameof(Instance))]
    public class Instance
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Environments { get; set; }

        public User User { get; set; }
    }
}
