using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Dtos.TemplateCommands;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service.Mappings;
using Docker.DotNet.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chargoon.ContainerManagement.Service
{
    public class InstanceService : IInstanceService
	{
		private readonly IInstanceRepository instanceRepository;
		private readonly IDockerService dockerService;
		private readonly IUserRepository userRepository;
		private readonly IMemoryCache memoryCache;
		private readonly ICommandService commandService;
		private readonly ITemplateRepository templateRepository;
		private readonly ITemplateCommandRepository templateCommandRepository;
		private readonly IAuthenticationService authenticationService;

		public InstanceService(
			IInstanceRepository instanceRepository,
			IDockerService dockerService,
			IUserRepository userRepository,
			IMemoryCache memoryCache,
			ICommandService commandService,
			ITemplateRepository templateRepository,
			ITemplateCommandRepository templateCommandRepository,
			IAuthenticationService authenticationService
			)
		{
			this.instanceRepository = instanceRepository;
			this.dockerService = dockerService;
			this.userRepository = userRepository;
			this.memoryCache = memoryCache;
			this.commandService = commandService;
			this.templateRepository = templateRepository;
			this.templateCommandRepository = templateCommandRepository;
			this.authenticationService = authenticationService;
		}

		private string GetTemplateCommandExecsCacheName() => $"TemplateCommand-Execs";

		public IEnumerable<InstanceGetDto> GetByUserId(int id)
		{
			return instanceRepository.GetAllByUserId(id).Select(x => x.ToDto()).ToList();
		}

		private Instance Load(Instance instance)
		{
			instance.User = userRepository.Get(instance.UserId);
			if (instance.TemplateId.HasValue)
			{
				instance.Template = templateRepository.Get(instance.TemplateId.Value);
				if (instance.Template != null)
				{
					instance.Template.Commands = templateCommandRepository.GetAllByTemplateId(instance.TemplateId.Value);
				}
			}
			return instance;
		}

		public IEnumerable<InstanceGetDto> GetAll()
		{
			var instances = instanceRepository.GetAll();
			foreach (var instance in instances)
			{
				Load(instance);
			}
			return instances.ToDto();
		}

		public IEnumerable<InstanceGetDto> GetAllByUserId(int userId)
		{
			var instances = instanceRepository.GetAllByUserId(userId);
			foreach (var instance in instances)
			{
				Load(instance);
			}
			return instances.ToDto();
		}

		public InstanceGetDto Get(int id)
		{
			var instance = instanceRepository.Get(id);
			if (instance == null) throw new NullReferenceException(nameof(instance));
			return instance.ToDto();
		}

		public IEnumerable<InstanceGetDto> GetAllOwn()
		{
			var userId = authenticationService.UserId;
			return GetAllByUserId(userId);
		}

		public InstanceGetDto GetOwn(int id)
		{
			var userId = authenticationService.UserId;
			var instance = instanceRepository.Get(id);
			if (instance == null) throw new NullReferenceException(nameof(instance));
			if (instance.UserId != userId) throw new Exception("this instance is not yours");
			return instance.ToDto();
		}

		public IEnumerable<SwarmService> GetAllOwnService(int id)
		{
			var instance = GetOwn(id);
			return dockerService.GetAllServiceByPrefix(instance.GetStackName());
		}

		public IEnumerable<ContainerListResponse> GetAllOwnContainer(int id)
		{
			var instance = GetOwn(id);
			return dockerService.GetAllContainerByPrefix(instance.GetStackName());
		}

		public IEnumerable<TemplateCommandExecDto> GetAllOwnCommands(int id)
		{
			var result = new List<TemplateCommandExecDto>();
			if (memoryCache.TryGetValue(GetTemplateCommandExecsCacheName(), out List<TemplateCommandExecDto> tces))
			{
				foreach (var tce in tces.Where(x => x.InstanceId == id).ToList())
				{
					try
					{
						tce.Inspect = dockerService.GetContainerExecInspect(tce.CommandId);
						result.Add(tce);
					}
					catch
					{

					}
				}
			}
			return result;
		}

		public InstanceGetDto Add(InstanceAddDto dto)
		{
			var instances = instanceRepository.GetAllByUserId(dto.UserId);
			if (instances.Any(x => x.Name == dto.Name)) throw new Exception("Instance Name Exists");

			if (instances.Count() > 8) throw new Exception("Max instance count per user is 10");

			instances.OrderBy(x => x.Code);

			var code = 0;
			for (int i = 0, length = instances.Count(); i < length; i++, code++)
			{
				var instance = instances.ElementAt(i);
				if (instance.Code != i)
				{
					code = i;
					break;
				}
			}

			return instanceRepository.Insert(new Domain.DataModels.Instance()
			{
				Code = code,
				UserId = dto.UserId,
				Name = dto.Name,
			}).ToDto();
		}

		private InstanceGetDto ChangeTemplate(Instance instance, InstanceChangeTemplateDto dto)
		{
			instance.TemplateId = dto.TemplateId;
			instanceRepository.Update(instance);
			return instance.ToDto();
		}

		public InstanceGetDto ChangeTemplate(int id, InstanceChangeTemplateDto dto)
		{
			var instance = instanceRepository.Get(id);
			return ChangeTemplate(instance, dto);
		}

		public InstanceGetDto ChangeOwnTemplate(int id, InstanceChangeTemplateDto dto)
		{
			var instance = instanceRepository.Get(id);
			var userId = authenticationService.UserId;
			if (instance.UserId != userId) throw new Exception("this instance is not yours");
			return ChangeTemplate(instance, dto);
		}

		private InstanceGetDto Start(Instance instance)
		{
			if (instance == null) throw new ArgumentNullException(nameof(instance));
			var dto = instance.ToDto();
			if (instance.Template == null) throw new Exception("Instance does not have any template");
			if (dto.Template.DockerCompose == null) throw new Exception("Docker Compose Failed to Read");
			var dockerCompose = instance.Template.DockerCompose;
			foreach (var item in dto.Environments)
			{
				dockerCompose = dockerCompose.Replace("{" + item.Key + "}", item.Value);
			}
			if(!string.IsNullOrEmpty(instance.Template.BeforeStartCommand))
			{
				var command = instance.Template.BeforeStartCommand;
				foreach (var item in dto.Environments)
				{
					command = command.Replace("{" + item.Key + "}", item.Value);
				}
				commandService.Execute(command);
			}
			dockerService.Deploy(instance.User.Username + "_" + instance.Name, dockerCompose);
			if (!string.IsNullOrEmpty(instance.Template.AfterStartCommand))
			{
				var command = instance.Template.AfterStartCommand;
				foreach (var item in dto.Environments)
				{
					command = command.Replace("{" + item.Key + "}", item.Value);
				}
				commandService.Execute(command);
			}
			return instance.ToDto();
		}

		private InstanceGetDto Stop(Instance instance)
		{
			if (instance == null) throw new ArgumentNullException(nameof(instance));
			var dto = instance.ToDto();
			if (!string.IsNullOrEmpty(instance.Template.BeforeStopCommand))
			{
				var command = instance.Template.BeforeStopCommand;
				foreach (var item in dto.Environments)
				{
					command = command.Replace("{" + item.Key + "}", item.Value);
				}
				commandService.Execute(command);
			}
			dockerService.Undeploy(instance.User.Username, instance.Name);
			if (!string.IsNullOrEmpty(instance.Template.AfterStopCommand))
			{
				var command = instance.Template.AfterStopCommand;
				foreach (var item in dto.Environments)
				{
					command = command.Replace("{" + item.Key + "}", item.Value);
				}
				commandService.Execute(command);
			}
			memoryCache.TryGetValue(GetTemplateCommandExecsCacheName(), out List<TemplateCommandExecDto> tces);

			if (tces != null)
			{
				memoryCache.Set(GetTemplateCommandExecsCacheName(), tces.Where(x => x.InstanceId != instance.Id).ToList(), TimeSpan.FromHours(1));
			}
			return instance.ToDto();
		}

		public InstanceGetDto Start(int id)
		{
			var instance = instanceRepository.Get(id);
			return Start(instance);
		}

		public InstanceGetDto Stop(int id)
		{
			var instance = instanceRepository.Get(id);
			return Stop(instance);
		}

		public InstanceGetDto StartOwn(int id)
		{
			var instance = instanceRepository.Get(id);
			var userId = authenticationService.UserId;
			if (instance.UserId != userId) throw new Exception("this instance is not yours");
			return Start(instance);
		}

		public InstanceGetDto StopOwn(int id)
		{
			var instance = instanceRepository.Get(id);
			var userId = authenticationService.UserId;
			if (instance.UserId != userId) throw new Exception("this instance is not yours");
			return Stop(instance);
		}

		public InstanceGetDto RunOwnCommand(int id, int templateCommandId)
		{
			memoryCache.TryGetValue(GetTemplateCommandExecsCacheName(), out List<TemplateCommandExecDto> tces);

			if (tces != null)
			{
				foreach (var templateCommandExecDto in tces.Where(x => x.TemplateCommandId == templateCommandId).ToList())
				{
					var execInspect = dockerService.GetContainerExecInspect(templateCommandExecDto.CommandId);
					if (execInspect != null && execInspect.Running) throw new Exception("Command is running inside container");
				}
			}

			var userId = authenticationService.UserId;
			var instance = instanceRepository.Get(id);
			if (instance == null) throw new NullReferenceException(nameof(instance));
			if (instance.UserId != userId) throw new Exception("this instance is not yours");
			if (instance?.Template?.Commands == null) throw new NullReferenceException(nameof(instance.Template.Commands));
			var tc = instance.Template.Commands.FirstOrDefault(x => x.Id == templateCommandId);
			if (tc == null) throw new NullReferenceException("Template Command not found");

			var dto = instance.ToDto();
			var containers = dockerService.GetAllContainer();
			var stackName = dto.GetStackName();
			var container = containers.FirstOrDefault(x =>
			{
				return x.Names.Any(y =>
				{
					return y.Trim().ToLower().Contains((stackName + "_" + tc.ServiceName).Trim().ToLower());
				});
			});
			if (container == null) throw new NullReferenceException("Container not found");

			var command = tc.Command;
			foreach (var env in dto.Environments)
			{
				command = command.Replace("{" + env.Key + "}", env.Value);
			}

			var commandId = dockerService.ExecCommandContainer(container.ID, command);

			var item = new TemplateCommandExecDto()
			{
				InstanceId = instance.Id,
				TemplateId = tc.TemplateId,
				TemplateCommandId = templateCommandId,
				CommandId = commandId
			};
			if (tces != null)
			{
				tces.Add(item);
			}
			else
			{
				tces = new List<TemplateCommandExecDto> { item };
			}
			memoryCache.Set(GetTemplateCommandExecsCacheName(), tces, TimeSpan.FromHours(1));
			return dto;
		}


		public bool Remove(int id)
		{
			var instance = instanceRepository.Get(id);
			if (instance == null) throw new NullReferenceException(nameof(instance));

			Stop(instance);

			instanceRepository.Delete(instance);

			return true;
		}
	}
}
