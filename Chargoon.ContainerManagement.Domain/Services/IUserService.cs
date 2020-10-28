using Chargoon.ContainerManagement.Domain.Dtos.Auth;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface IUserService
    {
        UserGetDto Add(UserAddDto dto);
        UserGetDto ChangeOwnPassword(UserChangePasswordDto dto);
        UserGetDto ChangePassword(int id, UserChangePasswordDto dto);
        IEnumerable<UserGetDto> GetAll();
        UserGetDto GetById(int id);
        UserGetDto GetOwn();
    }
}
