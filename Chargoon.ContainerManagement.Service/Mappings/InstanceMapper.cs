using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Mappings
{
    public static class InstanceMapper
    {
        private static InstanceGetDto MergeEnvironments(this InstanceGetDto dto)
        {
            var basePort = (dto.UserId * 1000) + (dto.Id * 10);
            var envs = dto.Environments;
            if (string.IsNullOrEmpty(envs.Type)) envs.Type = "full"; 
            if (string.IsNullOrEmpty(envs.Branch)) envs.Branch = "Release";
            if (string.IsNullOrEmpty(envs.DatabaseUsername)) envs.DatabaseUsername = "didgah";
            if (string.IsNullOrEmpty(envs.DatabasePassword)) envs.DatabasePassword = "lfdc82zo";
            if (string.IsNullOrEmpty(envs.BuildVersion)) envs.BuildVersion = $"{DateTime.Now:yyyyy.MM.dd}";
            if (string.IsNullOrEmpty(envs.ContainerTag)) envs.ContainerTag = "CONTAINER_TAG";

            envs.User = dto.User.Username;
            envs.ComposeProjectName = $"{dto.User.Username}_{dto.Name}";
            envs.Registry = "dockerhub:5001";
            envs.SqlServerPort = (basePort + 0);
            envs.FileManagerPort = (basePort + 1);
            envs.Didgah4Port = (basePort + 4);
            envs.Didgah5Port = (basePort + 5);
            return dto;
        }

        public static InstanceGetDto ToDto(this Instance instance)
        {
            return new InstanceGetDto()
            {
                Id = instance.Id,
                UserId = instance.UserId,
                Name = instance.Name,
                Environments = JsonConvert.DeserializeObject<Environments>(instance.Environments) ?? new Environments(),

                User = (instance.User != null ? instance.User.ToDto() : null)
            }.MergeEnvironments();
        }
    }
}
