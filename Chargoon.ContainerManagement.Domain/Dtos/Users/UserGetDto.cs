using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Users
{
    public class UserGetDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        public List<InstanceGetDto> Instances { get; set; }
    }
}
