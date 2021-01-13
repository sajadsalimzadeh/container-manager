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
        UserGetDto ChangeOwnProfile(UserChangeProfileDto dto);
		UserGetDto ChangePassword(int id, UserChangePasswordDto dto);
        UserGetDto ChangeProfile(int id, UserChangeProfileDto dto);
		IEnumerable<UserGetDto> GetAll();
        UserGetDto GetById(int id);
        UserGetDto GetOwn();
        UserGetDto ResetPassword(int id, UserResetPasswordDto dto);
    }
}
