using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.DataModels;
using Microsoft.Extensions.Configuration;
using Dapper.FastCrud;
using System.Linq;

namespace Chargoon.ContainerManagement.Data.Repositories
{
    public class UserRepository : BaseCrudRepository<User, int>, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public User GetByUsername(string username)
        {
            return conn
                .Find<User>(s => s.WithParameters(new { Username = username }).Where($"{nameof(User.Username)} = @Username"))
                .FirstOrDefault();
        }
    }
}
