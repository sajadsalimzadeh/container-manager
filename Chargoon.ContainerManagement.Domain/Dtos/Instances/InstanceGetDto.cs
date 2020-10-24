using Chargoon.ContainerManagement.Domain.Dtos.Users;
using Chargoon.ContainerManagement.Domain.Models;

namespace Chargoon.ContainerManagement.Domain.Dtos.Instances
{
    public class InstanceGetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public Environments Environments { get; set; }

        public UserGetDto User { get; set; }
    }
}
