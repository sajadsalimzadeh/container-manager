using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Data.Repositories
{
    public interface IBranchRepository : ICrudRepository<Branch, int>
    {
    }
}
