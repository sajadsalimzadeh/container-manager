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
        private readonly IUserRepository userRepository;
        private readonly ITemplateRepository templateRepository;

        public InstanceRepository(IConfiguration configuration,
            IUserRepository userRepository,
            ITemplateRepository templateRepository
            ) : base(configuration)
        {
            this.userRepository = userRepository;
            this.templateRepository = templateRepository;
        }

        public override Instance Get(int key)
        {
            var result = base.Get(key);
            if (result != null)
            {
                result.User = userRepository.Get(result.UserId);
                if (result.TemplateId.HasValue) result.Template = templateRepository.Get(result.TemplateId.Value);
            }
            return result;
        }

        public IEnumerable<Instance> GetAllByUserId(int id)
        {
            return conn.Find<Instance>(s => s.Include<Template>().WithParameters(new { UserId = id }).Where($"{nameof(Instance.UserId):C} = @UserId"));
        }

        public Instance GetByName(string name)
        {
            return conn.Find<Instance>(s => s.Where($"{nameof(Instance.Name):C} = @Name").WithParameters(new { Name = name })).FirstOrDefault();
        }
    }
}
