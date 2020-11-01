using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Mappings
{
    public static class InstanceMapper
    {

        public static InstanceGetDto ToDto(this Instance instance)
        {
            var dto = new InstanceGetDto()
            {
                Id = instance.Id,
                Code = instance.Code,
                Name = instance.Name,
                UserId = instance.UserId,
                TemplateId = instance.TemplateId,

            };

            try
            {
                dto.Environments = (!string.IsNullOrEmpty(instance.Environments) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(instance.Environments) : new Dictionary<string, string>());
            }
            catch { }

            try
            {
                dto.User = (instance.User != null ? instance.User.ToDto() : null);
            }
            catch { }

            try
            {
                dto.Template = (instance.Template != null ? instance.Template.ToDto() : null);
            }
            catch { }

            return dto.MergeEnvironments();
        }

        public static IEnumerable<InstanceGetDto> ToDto(this IEnumerable<Instance> instances)
        {
            return instances.Select(x => x.ToDto());
        }

        private static InstanceGetDto MergeEnvironments(this InstanceGetDto dto)
        {
            var envs = dto.Environments;
            var basePort = (dto.UserId * 1000) + (dto.Code * 100);

            envs["BASE_PORT"] = (basePort / 100).ToString();
            envs["REGISTERY"] = "dockerhub:5001";
            envs["CODE"] = dto.Code.ToString();

            if (dto.User != null)
            {
                envs["COMPOSE_PROJECT_NAME"] = $"{dto.User.Username}_{dto.Name}";
                envs["USER"] = dto.User.Username;
            }

            if (dto.Template != null)
            {
                var defaultEnvironments = dto.Template.Environments;

                foreach (var item in defaultEnvironments)
                {
                    if (!envs.ContainsKey(item.Key)) envs[item.Key] = item.Value;
                    else if (string.IsNullOrEmpty(envs[item.Key])) envs[item.Key] = item.Value;
                }

                envs["TEMPLATE"] = dto.Template.Name;
            }

            return dto;
        }
    }
}
