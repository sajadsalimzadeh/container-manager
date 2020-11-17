using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.TemplateCommands;
using Chargoon.ContainerManagement.Domain.Dtos.Templates;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Mappings
{
    public static class TemplateCommandMapper
    {
        public static TemplateCommandGetDto ToDto(this TemplateCommand model)
        {
            return new TemplateCommandGetDto()
            {
                Id = model.Id,
                Name = model.Name,
                Color = model.Color,
                RunOnStartup = model.RunOnStartup,
                TemplateId = model.TemplateId,
                ServiceName = model.ServiceName,
            };
        }
        public static IEnumerable<TemplateCommandGetDto> ToDto(this IEnumerable<TemplateCommand> templates)
        {
            return templates.Select(x => x.ToDto());
        }
    }
}
