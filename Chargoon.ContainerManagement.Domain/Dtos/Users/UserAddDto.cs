using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Users
{
    public class UserAddDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string Instances { get; set; }
    }
}
