using Chargoon.ContainerManagement.Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Data.Repositories
{
    public interface IUserRepository : ICrudRepository<User, int>
    {
        User GetByUsername(string username);
    }
}
