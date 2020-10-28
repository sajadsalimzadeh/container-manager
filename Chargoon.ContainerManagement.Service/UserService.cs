using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Chargoon.ContainerManagement.Service
{
    public class UserService : IUserService
    {
        private readonly IInstanceRepository instanceRepository;
        private readonly IServiceProvider services;
        private readonly IAuthenticationService authenticationService;
        private readonly IUserRepository userRepository;
        private readonly string teamSpace;

        public UserService(
            IInstanceRepository instanceRepository,
            IConfiguration configuration,
            IServiceProvider services,
            IAuthenticationService authenticationService,
            IUserRepository userRepository)
        {
            this.instanceRepository = instanceRepository;
            this.services = services;
            this.authenticationService = authenticationService;
            this.userRepository = userRepository;
            teamSpace = configuration.GetSection("Volumes").GetSection("TeamSpace").Value;
        }

        public IEnumerable<UserGetDto> GetAll()
        {
            var users = userRepository.GetAll();
            foreach (var user in users)
            {
                user.Instances = instanceRepository.GetAllByUserId(user.Id);
            }
            return users.ToDto();
        }

        public UserGetDto GetById(int id)
        {
            return userRepository.Get(id).ToDto();
        }

        public UserGetDto GetOwn()
        {
            var userId = authenticationService.UserId;
            return GetById(userId);
        }

        public UserGetDto Add(UserAddDto dto)
        {
            var instanceService = services.GetService<IInstanceService>();
            var user = userRepository.Insert(dto.ToDataModel()).ToDto();

            var instances = new List<InstanceGetDto>();
            var instanceNames = dto.Instances?.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var instanceName in instanceNames)
            {
                var instance = instanceService.Add(new InstanceAddDto()
                {
                    UserId = user.Id,
                    Name = instanceName
                });

                instances.Add(instance);
            }

            user.Instances = instances;

            return user;
        }

        public UserGetDto ChangePassword(int id, UserChangePasswordDto dto)
        {
            var user = userRepository.Get(id);
            if (user.Password != dto.CurrentPassword) throw new Exception("Current Password is not valid");

            user.Password = dto.newPassword;

            user = userRepository.Update(user);

            return user.ToDto();
        }

        public UserGetDto ChangeOwnPassword(UserChangePasswordDto dto)
        {
            var userId = authenticationService.UserId;
            return ChangePassword(userId, dto);
        }
    }
}
