using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Data.Repositories
{
    public interface IInstanceRepository : ICrudRepository<Instance, int>
    {
        Instance GetByName(string name);
        IEnumerable<Instance> GetAllByUserId(int id);
		IEnumerable<Instance> GetAllByTemplateId(int id);
	}
}
