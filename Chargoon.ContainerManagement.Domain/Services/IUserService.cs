using Chargoon.ContainerManagement.Domain.Dtos.Auth;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface IUserService
    {
        List<UserGetDto> GetAll();
        UserGetDto GetById(int id);
    }
}
