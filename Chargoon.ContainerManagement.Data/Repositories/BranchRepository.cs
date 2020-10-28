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
    public class BranchRepository : BaseCrudRepository<Branch, int>, IBranchRepository
    {
        public BranchRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
