using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Chargoon.ContainerManagement.Domain.Models;
using Chargoon.ContainerManagement.Domain.Services;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Chargoon.ContainerManagement.Service
{
	public class DockerService : IDockerService
	{
		private readonly ILoggerService logger;
		private readonly IMemoryCache memoryCache;
		private readonly ICommandService commandService;
		private readonly AppSettings appSettings;
		private readonly IConfiguration configuration;
		private readonly DockerClient client;

		public DockerService(
			ILoggerService logger,
			IMemoryCache memoryCache,
			IOptions<AppSettings> appSettings,
			ICommandService commandService,
			IConfiguration configuration)
		{
			this.logger = logger;
			this.memoryCache = memoryCache;
			this.commandService = commandService;
			this.appSettings = appSettings.Value;
			this.configuration = configuration;
			var dockerSettings = configuration.GetSection("Docker");
			client = new DockerClientConfiguration(new Uri(dockerSettings.GetSection("Url").Value)).CreateClient();
		}

		private string GetContainerCommandsCacheName() => $"Container-Commands";

		public IEnumerable<ImagesListResponse> GetAllImage()
		{
			if (!memoryCache.TryGetValue(nameof(GetAllImage), out IEnumerable<ImagesListResponse> result))
			{
				result = client.Images.ListImagesAsync(new ImagesListParameters()).Result;
				memoryCache.Set(nameof(GetAllImage), result, TimeSpan.FromSeconds(appSettings.Docker.ImageCacheTimeout));
			}
			return result;
		}

		public IEnumerable<ContainerListResponse> GetAllContainer()
		{
			if (!memoryCache.TryGetValue(nameof(GetAllContainer), out IEnumerable<ContainerListResponse> result))
			{
				result = client.Containers.ListContainersAsync(new ContainersListParameters()).Result;
				memoryCache.Set(nameof(GetAllContainer), result, TimeSpan.FromSeconds(appSettings.Docker.ContainerCacheTimeout));
			}
			return result;
		}

		public IEnumerable<ContainerListResponse> GetAllContainerByPrefix(string prefix)
		{
			var containers = GetAllContainer();
			foreach (var container in containers)
			{
				if (container.Names.Any(x => x.IndexOf(prefix) > -1 && x.IndexOf(prefix) < 3))
				{
					yield return container;
				}
			}
		}

		public IEnumerable<SwarmService> GetAllService()
		{
			if (!memoryCache.TryGetValue(nameof(GetAllService), out IEnumerable<SwarmService> result))
			{
				result = client.Swarm.ListServicesAsync(new ServicesListParameters()).Result;
				memoryCache.Set(nameof(GetAllService), result, TimeSpan.FromSeconds(appSettings.Docker.ServiceCacheTimeout));
			}
			return result;
		}

		public IEnumerable<NetworkResponse> GetAllNetwork()
		{
			if (!memoryCache.TryGetValue(nameof(GetAllService), out IEnumerable<NetworkResponse> result))
			{
				result = client.Networks.ListNetworksAsync(new NetworksListParameters()).Result;
				memoryCache.Set(nameof(GetAllService), result, TimeSpan.FromSeconds(appSettings.Docker.ServiceCacheTimeout));
			}
			return result;
		}

		public IEnumerable<SwarmService> GetAllServiceByPrefix(string prefix)
		{
			var services = GetAllService();
			foreach (var service in services)
			{
				if (service.Spec.Name.StartsWith(prefix))
				{
					yield return service;
				}
			}
		}

		public IEnumerable<NodeListResponse> GetAllNode()
		{
			if (!memoryCache.TryGetValue(nameof(GetAllNode), out IEnumerable<NodeListResponse> result))
			{
				result = client.Swarm.ListNodesAsync().Result;
				memoryCache.Set(nameof(GetAllNode), result, TimeSpan.FromSeconds(appSettings.Docker.NodeCacheTimeout));
			}
			return result;
		}

		public IEnumerable<ContainerFileSystemChangeResponse> GetContainerChanges(string id)
		{
			return client.Containers.InspectChangesAsync(id).Result;
		}

		public ContainerInspectResponse GetContainerInspect(string id)
		{
			return client.Containers.InspectContainerAsync(id).Result;
		}

		public ContainerProcessesResponse GetContainerProcesses(string id)
		{
			return client.Containers.ListProcessesAsync(id, new ContainerListProcessesParameters()).Result;
		}

		public string GetContainerLog(string id)
		{
			var mstream = client.Containers.GetContainerLogsAsync(id, true, new ContainerLogsParameters()).Result;
			var result = mstream.ReadOutputToEndAsync(default).Result;
			return result.stderr ?? result.stdout;
		}

		public Stream GetContainerExport(string id)
		{
			return client.Containers.ExportContainerAsync(id).Result;
		}

		public bool StartContainer(string id)
		{
			return client.Containers.StartContainerAsync(id, new ContainerStartParameters()).Result;
		}

		public bool StopContainer(string id)
		{
			return client.Containers.StopContainerAsync(id, new ContainerStopParameters()).Result;
		}

		public void KillContainer(string id)
		{
			client.Containers.KillContainerAsync(id, new ContainerKillParameters());
		}

		public void RestartContainer(string id)
		{
			client.Containers.RestartContainerAsync(id, new ContainerRestartParameters());
		}

		public ContainerUpdateResponse UpdateContainer(string id, ContainerUpdateParameters dto)
		{
			return client.Containers.UpdateContainerAsync(id, dto).Result;
		}

		public void RenameContainer(string id, ContainerRenameParameters dto)
		{
			client.Containers.RenameContainerAsync(id, dto, default);
		}

		public void PauseContainer(string id)
		{
			client.Containers.PauseContainerAsync(id);
		}

		public void UnPauseContainer(string id)
		{
			client.Containers.UnpauseContainerAsync(id);
		}

		public void RemoveContainer(string id)
		{
			client.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters());
		}

		public void RemoveImage(string name)
		{
			client.Images.DeleteImageAsync(name, new ImageDeleteParameters());
		}

		public GetArchiveFromContainerResponse ArchiveContainer(string id, GetArchiveFromContainerParameters dto)
		{
			return client.Containers.GetArchiveFromContainerAsync(id, dto, false).Result;
		}

		public void ExtractArchiveToContainerAsync(string id, ContainerPathStatParameters dto, Stream stream)
		{
			client.Containers.ExtractArchiveToContainerAsync(id, dto, stream);
		}

		public IEnumerable<ContainerExecInspectResponse> GetAllContainerCommand(string id)
		{
			var result = new List<ContainerExecInspectResponse>();
			if (memoryCache.TryGetValue(GetContainerCommandsCacheName(), out List<DockerCommandCacheDto> commands))
			{
				lock (commands)
				{
					foreach (var command in commands.Where(x => x.ContainerId == id).ToList())
					{
						try
						{
							result.Add(client.Exec.InspectContainerExecAsync(command.CommandId).Result);
						}
						catch { }
					}
				}
			}
			return result;
		}

		private List<string> ParseCommand(string command)
		{
			//========== Parse CMD ==========\\
			var cmds = new List<string>();
			var inQuote = false;
			var inDoubleQuote = false;
			var temp = string.Empty;
			command = command.Trim();
			for (int i = 0; i < command.Length; i++)
			{
				var ch = command[i];
				if (ch == '"') inDoubleQuote = !inDoubleQuote;
				else if (ch == '\'') inQuote = !inQuote;
				else
				{
					if (ch == ' ')
					{
						if (inQuote || inDoubleQuote) temp += ch;
						else
						{
							cmds.Add(temp);
							temp = "";
						}
					}
					else temp += ch;
				}
			}
			cmds.Add(temp);
			return cmds;
		}

		public string ExecCommandContainer(string id, string command)
		{
			var dto = new ContainerExecCreateParameters()
			{
				AttachStderr = true,
				AttachStdin = true,
				AttachStdout = true,
				Tty = true,
				WorkingDir = "/",
				Cmd = ParseCommand(command)
			};
			var result = client.Exec.ExecCreateContainerAsync(id, dto).Result;
			var mstream = client.Exec.StartAndAttachContainerExecAsync(result.ID, true).Result;
			Task.Run(() =>
			{
				if (memoryCache.TryGetValue(GetContainerCommandsCacheName(), out List<DockerCommandCacheDto> commands))
				{
					var commandDto = commands.FirstOrDefault(x => x.CommandId == result.ID);
					if (commandDto != null)
					{
						commandDto.Output = $"Command:\n{command}\n";
						var output = mstream.ReadOutputToEndAsync(default).Result;
						commandDto.Output += $"Error:\n{output.stderr}\nOutput:\n{output.stdout}";
					}
				}
				memoryCache.Set(GetContainerCommandsCacheName(), commands, TimeSpan.FromHours(1));
			});

			var item = new DockerCommandCacheDto() { ContainerId = id, CommandId = result.ID };
			if (memoryCache.TryGetValue(GetContainerCommandsCacheName(), out List<DockerCommandCacheDto> commands))
			{
				lock (commands)
				{
					commands.Add(item);
				}
			}
			else
			{
				commands = new List<DockerCommandCacheDto> { item };
			}
			memoryCache.Set(GetContainerCommandsCacheName(), commands, TimeSpan.FromHours(1));
			return result.ID;
		}

		public string GetExecCommandContainerLog(string commandId)
		{
			if (memoryCache.TryGetValue(GetContainerCommandsCacheName(), out List<DockerCommandCacheDto> commands))
			{
				var command = commands.FirstOrDefault(x => x.CommandId == commandId);
				return command != null ? command.Output : null;
			}
			return null;
		}

		public void ClearExitedCommands()
		{
			if (memoryCache.TryGetValue(GetContainerCommandsCacheName(), out List<DockerCommandCacheDto> commands))
			{
				foreach (var command in commands)
				{
					var commandInspect = GetContainerExecInspect(command.CommandId);
					if (!commandInspect.Running)
					{
						commands.Remove(command);
					}
				}
			}
			memoryCache.Set(GetContainerCommandsCacheName(), commands, TimeSpan.FromHours(1));
		}

		public ContainerExecInspectResponse GetContainerExecInspect(string id)
		{
			try
			{
				return client.Exec.InspectContainerExecAsync(id).Result;
			}
			catch
			{
				return null;
			}
		}

		public void SystemPrune(bool all = false)
		{
			commandService.Execute($"docker system prune {(all ? "-a" : "")} -f");
		}

		public void ContainerPrune(bool all = false)
		{
			commandService.Execute($"docker container prune {(all ? "-a" : "")} -f");
		}

		private string CreateDockerComposeFile(string dockerCompose)
		{
			var dirPath = Path.Combine(Path.GetTempPath(), "ContainerManagement");
			var dockerComposeFilePath = Path.Combine(dirPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".yml");
			if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
			File.WriteAllText(dockerComposeFilePath, dockerCompose);
			return dockerComposeFilePath;
		}

		public void Build(string dockerCompose)
		{
			var dockerComposeFilePath = CreateDockerComposeFile(dockerCompose);
			commandService.Execute($"docker-compose build \"{dockerComposeFilePath}\"");
		}

		public void Deploy(string stackName, string dockerCompose)
		{
			var dockerComposeFilePath = CreateDockerComposeFile(dockerCompose);
			commandService.Execute($"docker stack deploy -c \"{dockerComposeFilePath}\" {stackName}");
		}

		public void Undeploy(string stackName, string username)
		{
			var services = GetAllService();
			var searchValue = (stackName + "_" + username).ToLower();
			foreach (var service in services)
			{
				if (service.Spec.Name.ToLower().StartsWith(searchValue))
				{
					client.Swarm.RemoveServiceAsync(service.ID).Wait();
					logger.LogInformation($"Service {service.Spec.Name} Shutting down");
				}
			}
			var networks = GetAllNetwork();
			foreach (var network in networks)
			{
				if (network.Name.ToLower().StartsWith(searchValue))
				{
					client.Networks.DeleteNetworkAsync(network.ID).Wait();
					logger.LogInformation($"Network {network.Name} Deleted");
				}
			}
		}

	}
}
