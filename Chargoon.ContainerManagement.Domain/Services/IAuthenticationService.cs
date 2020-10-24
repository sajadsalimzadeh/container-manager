using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface IAuthenticationService
    {
        int? UserId { get; }

        User GetUser();
        bool HasRole(string role);
        LoginResponse Login(LoginRequest model);
    }
}
