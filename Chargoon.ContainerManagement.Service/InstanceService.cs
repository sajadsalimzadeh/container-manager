using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Service
{
    public class InstanceService : IInstanceService
    {
        private readonly IInstanceRepository instanceRepository;
        private readonly IDockerService dockerService;

        public InstanceService(IInstanceRepository instanceRepository, IDockerService dockerService)
        {
            this.instanceRepository = instanceRepository;
            this.dockerService = dockerService;
        }

        public List<InstanceGetDto> GetByUserId(int id)
        {
            return instanceRepository.GetAllByUserId(id).Select(x => x.ToDto()).ToList();
        }

        public InstanceGetDto Add(InstanceAddDto dto)
        {
            var instance = instanceRepository.GetByName(dto.Name);
            if (instance != null) throw new Exception("Instance Name Exists");

            return instanceRepository.Insert(new Domain.DataModels.Instance()
            {
                UserId = dto.UserId,
                Name = dto.Name,
            }).ToDto();
        }

        public bool StartContainer(int id)
        {
            dockerService.
        }

        public bool StopContainer(int id)
        {

        }

        public bool StartAppPools(int id)
        {

        }

        public bool StopAppPools(int id)
        {

        }

        public bool RestartAppPools(int id)
        {

        }

        public bool RunFileManager(int id)
        {

        }

        public bool UpdateDatabase(int id)
        {

        }

        public bool ChangeImage(int id, InstanceChangeImageDto dto)
        {

        }

        public bool ChangeType(int id, InstanceChangeTypeDto dto)
        {

        }
    }
}
