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
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Chargoon.ContainerManagement.Service.Mappings
{
    public static class TemplateMapper
    {
        public static TemplateGetDto ToDto(this Template model)
        {
            var dto = new TemplateGetDto()
            {
                Id = model.Id,
                Name = model.Name,
                IsActive = model.IsActive,
                InsertCron = model.InsertCron,
                DockerCompose = model.DockerCompose,
            };
            try
            {
                var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                dto.DockerComposeObj = (!string.IsNullOrEmpty(model.DockerCompose) ? deserializer.Deserialize<DockerCompose>(model.DockerCompose) : new DockerCompose());
            }
            catch { }

            try
            {
                dto.Environments = (!string.IsNullOrEmpty(model.Environments) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(model.Environments) : new Dictionary<string, string>());
            }
            catch { }
            try
            {
                dto.Commands = (model.Commands != null ? model.Commands.ToDto() : new List<TemplateCommandGetDto>());
            }
            catch { }
            return dto;
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
                InsertCron = dto.InsertCron,
                DockerCompose = dto.DockerCompose,
                Environments = (dto.Environments != null ? JsonConvert.SerializeObject(dto.Environments) : null)
            };
        }

        public static Template ToDataModel(this Template model, TemplateChangeDto dto)
        {
            model.Name = dto.Name;
            model.IsActive = dto.IsActive;
            model.InsertCron = dto.InsertCron;
            model.DockerCompose = dto.DockerCompose;
            model.Environments = (dto.Environments != null ? JsonConvert.SerializeObject(dto.Environments) : null);
            return model;
        }
    }
}
