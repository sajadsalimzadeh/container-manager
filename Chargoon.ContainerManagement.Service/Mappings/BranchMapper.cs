using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Branches;
using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Docker.DotNet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Mappings
{
    public static class BranchMapper
    {

        public static BranchGetDto ToDto(this Branch branch)
        {
            return new BranchGetDto()
            {
                Id = branch.Id,
                Name = branch.Name,
                DockerCompose = (string.IsNullOrEmpty(branch.DockerCompose) ? null : JsonConvert.DeserializeObject<DockerCompose>(branch.DockerCompose)),
                IsBuildEnable = branch.IsBuildEnable,
            };
        }

        public static IEnumerable<BranchGetDto> ToDto(this IEnumerable<Branch> branches)
        {
            return branches.Select(x => x.ToDto());
        }

        public static Branch ToDataModel(this BranchAddDto branch)
        {
            return new Branch()
            {
                Name = branch.Name,
                DockerCompose = (branch.DockerCompose == null ? null : JsonConvert.SerializeObject(branch.DockerCompose)),
                IsBuildEnable = branch.IsBuildEnable,
            };
        }
    }
}
