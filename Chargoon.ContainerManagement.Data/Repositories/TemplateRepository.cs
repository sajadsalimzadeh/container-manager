using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.DataModels;
using Microsoft.Extensions.Configuration;

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
    }
}
