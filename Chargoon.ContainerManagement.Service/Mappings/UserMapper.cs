using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Mappings
{
    public static class UserMapper
    {
        public static UserGetDto ToDto(this User user)
        {
            return new UserGetDto()
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }
        public static IEnumerable<UserGetDto> ToDto(this IEnumerable<User> users)
        {
            return users.Select(x => x.ToDto());
        }
    }
}
