using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Chargoon.ContainerManagement.Domain.Dtos.Instances;
using Chargoon.ContainerManagement.Domain.Models;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service.Utilities;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chargoon.ContainerManagement.Service
{
    public class DockerService : IDockerService
    {
        private readonly ILoggerService logger;
        private readonly IMemoryCache memoryCache;
        private readonly AppSettings appSettings;
        private readonly IConfiguration configuration;
        private readonly DockerClient client;

        public DockerService(
            ILoggerService logger,
            IMemoryCache memoryCache,
            IOptions<AppSettings> appSettings,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.memoryCache = memoryCache;
            this.appSettings = appSettings.Value;
            this.configuration = configuration;
            var dockerSettings = configuration.GetSection("Docker");
            client = new DockerClientConfiguration(new Uri(dockerSettings.GetSection("Url").Value)).CreateClient();
        }

        private string GetContainerCommandsCacheName() => $"Container-Commands";

        private void ExecuteCommand(string command)
        {
            try
            {
                var psi = new ProcessStartInfo("cmd.exe")
                {
                    Arguments = $"/C {command}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                };
                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();
                    var output = process.StandardOutput.ReadToEnd();
                    logger.LogInformation($"Method Call: {nameof(ExecuteCommand)}", new { Command = command, Output = output });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
        }

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

        public IEnumerable<SwarmService> GetAllService()
        {
            if (!memoryCache.TryGetValue(nameof(GetAllService), out IEnumerable<SwarmService> result))
            {
                result = client.Swarm.ListServicesAsync(new ServicesListParameters()).Result;
                memoryCache.Set(nameof(GetAllService), result, TimeSpan.FromSeconds(appSettings.Docker.ServiceCacheTimeout));
            }
            return result;
        }

        public IEnumerable<SwarmService> GetAllServiceByStackName(string stackName)
        {
            var services = GetAllService();
            foreach (var service in services)
            {
                if (service.Spec.Name.StartsWith(stackName))
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
                var output = mstream.ReadOutputToEndAsync(default).Result;

                if (memoryCache.TryGetValue(GetContainerCommandsCacheName(), out List<DockerCommandCacheDto> commands))
                {
                    var commandDto = commands.FirstOrDefault(x => x.CommandId == result.ID);
                    if (commandDto != null) commandDto.Output = $"Command:{command}\nError:\n{output.stderr}\nOutput:\n{output.stdout}";
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

        public void SystemPrune()
        {
            ExecuteCommand($"docker system prune -f");
        }

        public void Deploy(string stackName, DockerCompose dc)
        {
            var dirPath = Path.Combine(Path.GetTempPath(), "ContainerManagement");
            var dockerComposeFilePath = Path.Combine(dirPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".yml");

            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            var yamlHelper = new YamlHelper(dc);
            File.WriteAllText(dockerComposeFilePath, yamlHelper.ToYaml());
            ExecuteCommand($"docker stack deploy -c \"{dockerComposeFilePath}\" {stackName}");
        }

        public void Undeploy(string stackName, string username)
        {
            var services = GetAllService();
            var searchValue = (stackName + "_" + username).ToLower();
            foreach (var service in services)
            {
                if (service.Spec.Name.ToLower().StartsWith(searchValue))
                {
                    client.Swarm.RemoveServiceAsync(service.ID);
                    logger.LogInformation($"Service {service.Spec.Name} Shutting down");
                }
            }
        }

    }
}
