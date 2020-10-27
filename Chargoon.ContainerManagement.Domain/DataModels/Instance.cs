using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chargoon.ContainerManagement.Domain.DataModels
{
    [Table(nameof(Instance))]
    public class Instance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Code { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [ForeignKey(nameof(Template))]
        public int? TemplateId { get; set; }
        public string Name { get; set; }
        public string Environments { get; set; }

        public User User { get; set; }
        public Template Template { get; set; }
    }
}
