using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using Chargoon.ContainerManagement.Domain.Models;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service.Mappings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Chargoon.ContainerManagement.Service
{
    public class UserService : IUserService
    {
        private readonly AppSettings appSettings;
        private readonly IMemoryCache memoryCache;
        private readonly IConfiguration configuration;
        private readonly IUserRepository userRepository;
        private readonly string teamSpace;

        public UserService(
            IOptions<AppSettings> appSettings,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            IUserRepository userRepository)
        {
            this.memoryCache = memoryCache;
            this.configuration = configuration;
            this.userRepository = userRepository;
            this.appSettings = appSettings.Value;
            teamSpace = configuration.GetSection("Volumes").GetSection("TeamSpace").Value;
        }

        public List<UserGetDto> GetAll()
        {
            return userRepository.GetAll().ToDto().ToList();
        }

        public UserGetDto GetById(int id)
        {
            return userRepository.Get(id).ToDto();
        }
    }
}
