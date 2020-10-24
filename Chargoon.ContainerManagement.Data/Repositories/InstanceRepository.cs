using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Dapper;
using Dapper.FastCrud;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Data.Repositories
{
    public class InstanceRepository : BaseCrudRepository<Instance, int>, IInstanceRepository
    {
        public InstanceRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public IEnumerable<Instance> GetAllByUserId(int id)
        {
            return conn.Find<Instance>(s => s.WithParameters(new { UserId = id }).Where($"{nameof(Instance.UserId)} = @UserId"));
        }

        public Instance GetByName(string name)
        {
            return conn.Query<Instance>($"{nameof(Instance.Name):C} = @Name", new { Name = name }).FirstOrDefault();
        }
    }
}
