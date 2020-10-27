using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Dtos.TemplateCommands;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface IInstanceService
    {
        InstanceGetDto Add(InstanceAddDto dto);
        InstanceGetDto ChangeOwnTemplate(int id, InstanceChangeTemplateDto dto);
        InstanceGetDto ChangeTemplate(int id, InstanceChangeTemplateDto dto);
        InstanceGetDto Get(int id);
        IEnumerable<InstanceGetDto> GetAll();
        IEnumerable<InstanceGetDto> GetAllByUserId(int userId);
        IEnumerable<InstanceGetDto> GetAllOwn();
        IEnumerable<TemplateCommandExecDto> GetAllOwnCommands(int id);
        IEnumerable<SwarmService> GetAllOwnService(int id);
        IEnumerable<InstanceGetDto> GetByUserId(int id);
        InstanceGetDto GetOwn(int id);
        bool Remove(int id);
        InstanceGetDto RunOwnCommand(int id, int templateCommandId);
        InstanceGetDto Start(int id);
        InstanceGetDto StartOwn(int id);
        InstanceGetDto Stop(int id);
        InstanceGetDto StopOwn(int id);
    }
}
