using Chargoon.ContainerManagement.Domain.Dtos.Branches;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface IBranchService
    {
        BranchGetDto Add(BranchAddDto dto);
        BranchGetDto Change(int id, BranchChangeDto dto);
        BranchGetDto Get(int id);
        IEnumerable<BranchGetDto> GetAll();
    }
}
