using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.Dtos.Branches;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service.Mappings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Chargoon.ContainerManagement.Service
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository branchRepository;
        private readonly IDockerService dockerService;
        private readonly ILoggerService logger;

        public BranchService(
            IBranchRepository branchRepository, 
            IDockerService dockerService,
            ILoggerService logger)
        {
            this.branchRepository = branchRepository;
            this.dockerService = dockerService;
            this.logger = logger;
        }

        public IEnumerable<BranchGetDto> GetAll()
        {
            return branchRepository.GetAll().ToDto();
        }

        public BranchGetDto Get(int id)
        {
            return branchRepository.Get(id).ToDto();
        }

        public BranchGetDto Add(BranchAddDto dto)
        {
            return branchRepository.Insert(dto.ToDataModel()).ToDto();
        }

        public BranchGetDto Change(int id, BranchChangeDto dto)
        {
            var branch = branchRepository.Get(id);
            if (branch == null) throw new NullReferenceException("branch not found");

            branch.Name = dto.Name;
            branch.IsBuildEnable = dto.IsBuildEnable;
            if (dto.DockerCompose != null) branch.DockerCompose = JsonConvert.SerializeObject(dto.DockerCompose);

            branchRepository.Update(branch);

            return branch.ToDto();
        }

        public void Build(int branchId)
        {
            try
            {
                var branch = branchRepository.Get(branchId);
                var dto = branch.ToDto();
                dockerService.Build(dto.DockerCompose);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
        }

        public Task BuidAsync(int branchId)
        {
            return Task.Run(() =>
            {
                Build(branchId);
            });
        }
    }
}
