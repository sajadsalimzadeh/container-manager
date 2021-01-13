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
using YamlDotNet.Serialization;

namespace Chargoon.ContainerManagement.Service
{
	public class DockerService : IDockerService
	{
		private DockerClient _client;

		private readonly ILoggerService logger;
		private readonly IMemoryCache memoryCache;
		private readonly ICommandService commandService;
		private readonly IAuthenticationService authenticationService;
		private readonly AppSettings appSettings;
		private readonly IConfiguration configuration;

		public DockerService(
			ILoggerService logger,
			IMemoryCache memoryCache,
			IOptions<AppSettings> appSettings,
			ICommandService commandService,
			IAuthenticationService authenticationService,
			IConfiguration configuration)
		{
			this.logger = logger;
			this.memoryCache = memoryCache;
			this.commandService = commandService;
			this.authenticationService = authenticationService;
			this.appSettings = appSettings.Value;
			this.configuration = configuration;
		}

		private string GetContainerCommandsCacheName() => $"Container-Commands";

		private DockerClient GetClient()
		{
			if (_client != null) return _client;
			var user = authenticationService.GetUser();
			var url = user.Host;
			if (string.IsNullOrEmpty(url) && appSettings.Docker.SelfHostEnable) url = appSettings.Docker.Url;
			if (string.IsNullOrEmpty(url))
				throw new Exception("Please Enter Your Host First");
			if (url.IndexOf(":") < 0) url += ":2375";
			if (url.IndexOf("http") < 0) url = "http://" + url;
			return _client = new DockerClientConfiguration(new Uri(url)).CreateClient();

		}

		public IEnumerable<ImagesListResponse> GetAllImage()
		{
			if (!memoryCache.TryGetValue(nameof(GetAllImage), out IEnumerable<ImagesListResponse> result))
			{
				result = GetClient().Images.ListImagesAsync(new ImagesListParameters()).Result;
				memoryCache.Set(nameof(GetAllImage), result, TimeSpan.FromSeconds(appSettings.Docker.ImageCacheTimeout));
			}
			return result;
		}

		public IEnumerable<ContainerListResponse> GetAllContainer(bool all = false)
		{
			if (!memoryCache.TryGetValue(nameof(GetAllContainer) + (all ? "_All" : ""), out IEnumerable<ContainerListResponse> result))
			{
				result = GetClient().Containers.ListContainersAsync(new ContainersListParameters()
				{
					All = all
				}).Result;
				memoryCache.Set(nameof(GetAllContainer) + (all ? "_All" : ""), result, TimeSpan.FromSeconds(appSettings.Docker.ContainerCacheTimeout));
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
				result = GetClient().Swarm.ListServicesAsync(new ServicesListParameters()).Result;
				memoryCache.Set(nameof(GetAllService), result, TimeSpan.FromSeconds(appSettings.Docker.ServiceCacheTimeout));
			}
			return result;
		}

		public IEnumerable<NetworkResponse> GetAllNetwork()
		{
			if (!memoryCache.TryGetValue(nameof(GetAllService), out IEnumerable<NetworkResponse> result))
			{
				result = GetClient().Networks.ListNetworksAsync(new NetworksListParameters()).Result;
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
				result = GetClient().Swarm.ListNodesAsync().Result;
				memoryCache.Set(nameof(GetAllNode), result, TimeSpan.FromSeconds(appSettings.Docker.NodeCacheTimeout));
			}
			return result;
		}

		public IEnumerable<ContainerFileSystemChangeResponse> GetContainerChanges(string id)
		{
			return GetClient().Containers.InspectChangesAsync(id).Result;
		}

		public ContainerInspectResponse GetContainerInspect(string id)
		{
			return GetClient().Containers.InspectContainerAsync(id).Result;
		}

		public ContainerProcessesResponse GetContainerProcesses(string id)
		{
			return GetClient().Containers.ListProcessesAsync(id, new ContainerListProcessesParameters()).Result;
		}

		public string GetContainerLog(string id)
		{
			var mstream = GetClient().Containers.GetContainerLogsAsync(id, true, new ContainerLogsParameters()).Result;
			var result = mstream.ReadOutputToEndAsync(default).Result;
			return result.stderr ?? result.stdout;
		}

		public Stream GetContainerExport(string id)
		{
			return GetClient().Containers.ExportContainerAsync(id).Result;
		}

		public bool StartContainer(string id)
		{
			return GetClient().Containers.StartContainerAsync(id, new ContainerStartParameters()).Result;
		}

		public bool StopContainer(string id)
		{
			return GetClient().Containers.StopContainerAsync(id, new ContainerStopParameters()).Result;
		}

		public void KillContainer(string id)
		{
			GetClient().Containers.KillContainerAsync(id, new ContainerKillParameters());
		}

		public void RestartContainer(string id)
		{
			GetClient().Containers.RestartContainerAsync(id, new ContainerRestartParameters());
		}

		public ContainerUpdateResponse UpdateContainer(string id, ContainerUpdateParameters dto)
		{
			return GetClient().Containers.UpdateContainerAsync(id, dto).Result;
		}

		public void RenameContainer(string id, ContainerRenameParameters dto)
		{
			GetClient().Containers.RenameContainerAsync(id, dto, default);
		}

		public void PauseContainer(string id)
		{
			GetClient().Containers.PauseContainerAsync(id);
		}

		public void UnPauseContainer(string id)
		{
			GetClient().Containers.UnpauseContainerAsync(id);
		}

		public void RemoveContainer(string id)
		{
			GetClient().Containers.RemoveContainerAsync(id, new ContainerRemoveParameters());
		}

		public void RemoveImage(string name)
		{
			GetClient().Images.DeleteImageAsync(name, new ImageDeleteParameters());
		}

		public GetArchiveFromContainerResponse ArchiveContainer(string id, GetArchiveFromContainerParameters dto)
		{
			return GetClient().Containers.GetArchiveFromContainerAsync(id, dto, false).Result;
		}

		public void ExtractArchiveToContainerAsync(string id, ContainerPathStatParameters dto, Stream stream)
		{
			GetClient().Containers.ExtractArchiveToContainerAsync(id, dto, stream);
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
							result.Add(GetClient().Exec.InspectContainerExecAsync(command.CommandId).Result);
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
			var strStack = new Stack<char>();
			var temp = string.Empty;
			command = command.Trim();
			for (int i = 0; i < command.Length; i++)
			{
				var ch = command[i];
				if (ch == '"' || ch == '\'')
				{
					if (strStack.Count > 0)
					{
						if (strStack.Peek() == ch)
						{
							strStack.Pop();
						}
						else
						{
							strStack.Push(ch);
						}
					}
					else strStack.Push(ch);
				}
				else
				{
					if (ch == ' ')
					{
						if (strStack.Count > 0) temp += ch;
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
			var result = GetClient().Exec.ExecCreateContainerAsync(id, dto).Result;
			var mstream = GetClient().Exec.StartAndAttachContainerExecAsync(result.ID, true).Result;
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
				return GetClient().Exec.InspectContainerExecAsync(id).Result;
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

		public void Deploy(string stackName, string dockerComposeString)
		{
			var client = GetClient();
			var deserializer = new DeserializerBuilder().Build();
			var dockerCompose = deserializer.Deserialize<DockerCompose>(dockerComposeString);
			var containers = GetAllContainer(true);
			var network = client.Networks.ListNetworksAsync(new NetworksListParameters()).Result.FirstOrDefault(x => x.Name == stackName);
			var networkID = network?.ID;
			if (network == null)
			{
				var networkCreateResult = client.Networks.CreateNetworkAsync(new NetworksCreateParameters()
				{
					Name = stackName,
					Driver = "nat",
					Scope = "local",
					Labels = new Dictionary<string, string>()
					{
						{ "com.docker.stack.namespace", stackName }
					},
				}).Result;
				networkID = networkCreateResult.ID;
			}
			foreach (var item in dockerCompose.services)
			{
				var container = containers.FirstOrDefault(x => x.Names.Any(y => y.IndexOf(stackName + "_" + item.Key) > -1));
				if (container != null)
				{
					if (container.State.ToLower().IndexOf("exited") > -1)
					{
						client.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters()).Wait();
					}
					if (container.State.ToLower().IndexOf("stop") > -1)
					{
						client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters()).Wait();
						continue;
					}
				}
				var containerParameter = new CreateContainerParameters()
				{
					Name = stackName + "_" + item.Key,
					Image = item.Value.image,
					Env = item.Value.environment.Select(x => x.Replace(":", "=")).ToList(),
					Labels = new Dictionary<string, string>()
					{
						{ "com.docker.compose.project", stackName },
						{ "com.docker.compose.service", item.Key },
					},
					ExposedPorts = item.Value.ports.ToDictionary(x =>
					{
						var split = x.Split(':');
						return split[1] + "/tcp";
					}, x => new EmptyStruct()),
					HostConfig = new HostConfig()
					{
						NetworkMode = stackName,
						Binds = item.Value.volumes?.Select(x => x).ToList(),
						PortBindings = item.Value.ports.ToDictionary(x =>
						{
							var split = x.Split(':');
							return split[1] + "/tcp";
						}, x =>
						{
							var split = x.Split(':');
							return new List<PortBinding> {
								new PortBinding
								{
									HostIP = "",
									HostPort = split[0]
								}
							} as IList<PortBinding>;
						})
					},
					NetworkingConfig = new NetworkingConfig()
					{
						EndpointsConfig = new Dictionary<string, EndpointSettings>
						{
							{
								stackName, new EndpointSettings()
								{
									NetworkID = networkID,
									Aliases = new List<string>() { item.Key },
									Links = dockerCompose.services.Select(x => x.Key).ToList()
								}
							}
						}
					}
				};

				var containerCreateResult = client.Containers.CreateContainerAsync(containerParameter).Result;
				var containerStartParameter = new ContainerStartParameters() { };
				var startContainerResult = client.Containers.StartContainerAsync(containerCreateResult.ID, containerStartParameter).Result;
			}
		}

		public void Undeploy(string stackName, string username)
		{
			var client = GetClient();
			var containers = GetAllContainer();
			var searchValue = (stackName + "_" + username + "_").ToLower();
			foreach (var container in containers)
			{
				if (container.Names.Any(x => x.ToLower().IndexOf(searchValue) > -1))
				{
					var stopContainerResult = client.Containers.StopContainerAsync(container.ID, new ContainerStopParameters()).Result;
					client.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters()).Wait();
					logger.LogInformation($"Container {string.Join(",", container.Names)} Shutting down");
				}
			}
			var networks = GetAllNetwork();
			foreach (var network in networks)
			{
				if (network.Name.ToLower().StartsWith(searchValue))
				{
					try
					{
						GetClient().Networks.DeleteNetworkAsync(network.ID).Wait();
						logger.LogInformation($"Network {network.Name} Deleted");
					}
					catch
					{

					}
				}
			}
		}
	}
}
