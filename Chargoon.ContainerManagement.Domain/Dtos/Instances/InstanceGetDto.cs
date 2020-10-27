using Chargoon.ContainerManagement.Domain.Dtos.Templates;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using Chargoon.ContainerManagement.Domain.Models;
using System.Collections.Generic;

namespace Chargoon.ContainerManagement.Domain.Dtos.Instances
{
    public class InstanceGetDto
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public int UserId { get; set; }
        public int? TemplateId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Environments { get; set; }

        public UserGetDto User { get; set; }
        public TemplateGetDto Template { get; set; }

        public string GetStackName() => User?.Username + "_" + Name;
    }
}
