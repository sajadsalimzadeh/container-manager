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
    public class TemplateCommandRepository : BaseCrudRepository<TemplateCommand, int>, ITemplateCommandRepository
    {
        public TemplateCommandRepository(IConfiguration configuration) : base(configuration)
        {
        }


        public IEnumerable<TemplateCommand> GetAllByTemplateId(int templateId)
        {
            return conn.Find<TemplateCommand>(s => s.WithParameters(new { TemplateId = templateId }).Where($"{nameof(TemplateCommand.TemplateId):C} = @TemplateId"));
        }
    }
}
