using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Chargoon.ContainerManagement.Domain.Dtos.TemplateCommands;
using Chargoon.ContainerManagement.Domain.Dtos.Templates;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Mappings
{
    public static class TemplateMapper
    {
        public static TemplateGetDto ToDto(this Template template)
        {
            return new TemplateGetDto()
            {
                Id = template.Id,
                Name = template.Name,
                IsActive = template.IsActive,
                DockerCompose = (!string.IsNullOrEmpty(template.DockerCompose) ? JsonConvert.DeserializeObject<DockerCompose>(template.DockerCompose) : new DockerCompose()),
                Environments = (!string.IsNullOrEmpty(template.Environments) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(template.Environments) : new Dictionary<string, string>()),
                Commands = (template.Commands != null ? template.Commands.ToDto() : new List<TemplateCommandGetDto>())
            };
        }
        public static IEnumerable<TemplateGetDto> ToDto(this IEnumerable<Template> templates)
        {
            return templates.Select(x => x.ToDto());
        }

        public static Template ToDataModel(this TemplateAddDto dto)
        {
            return new Template()
            {
                Name = dto.Name,
                IsActive = dto.IsActive,
                DockerCompose = (dto.DockerCompose != null ? JsonConvert.SerializeObject(dto.DockerCompose) : null),
                Environments = (dto.Environments != null ? JsonConvert.SerializeObject(dto.Environments) : null)
            };
        }

        public static Template ToDataModel(this TemplateChangeDto dto)
        {
            return new Template()
            {
                Id = dto.Id,
                Name = dto.Name,
                IsActive = dto.IsActive,
                DockerCompose = (dto.DockerCompose != null ? JsonConvert.SerializeObject(dto.DockerCompose) : null),
                Environments = (dto.Environments != null ? JsonConvert.SerializeObject(dto.Environments) : null)
            };
        }
    }
}
