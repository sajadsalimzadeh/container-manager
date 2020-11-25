using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.DataModels;
using Dapper.FastCrud;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Chargoon.ContainerManagement.Data.Repositories
{
    public class TemplateRepository : BaseCrudRepository<Template, int>, ITemplateRepository
    {
        private readonly ITemplateCommandRepository templateCommandRepository;

        public TemplateRepository(IConfiguration configuration, ITemplateCommandRepository templateCommandRepository) : base(configuration)
        {
            this.templateCommandRepository = templateCommandRepository;
        }

        public override Template Get(int key)
        {
            var result = base.Get(key);
            if (result != null)
            {
                result.Commands = templateCommandRepository.GetAllByTemplateId(result.Id);
            }
            return result;
        }

        public IEnumerable<Template> GetAllExpired()
		{
            return conn.Find<Template>(s => s.Where($"{nameof(Template.ExpireTime):C} < @Time").WithParameters(new { Time = DateTime.Now }));
		}
    }
}
