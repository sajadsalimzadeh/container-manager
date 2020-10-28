using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Dtos.Users
{
    public class UserChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string newPassword { get; set; }
    }
}
