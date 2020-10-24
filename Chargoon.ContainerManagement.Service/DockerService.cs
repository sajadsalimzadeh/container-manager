using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Chargoon.ContainerManagement.Domain.Services;
using Chargoon.ContainerManagement.Service.Mappings;
using Chargoon.ContainerManagement.Service.Utilities;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Service
{
    public class DockerService : IDockerService
    {
        private readonly IMemoryCache memoryCache;
        private readonly IConfiguration configuration;
        private readonly DockerClient client;

        public DockerService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            this.memoryCache = memoryCache;
            this.configuration = configuration;
            var dockerSettings = configuration.GetSection("Docker");
            client = new DockerClientConfiguration(new Uri(dockerSettings.GetSection("Url").Value)).CreateClient();
        }

        public List<ImagesListResponse> GetAllImages()
        {
            List<ImagesListResponse> result;
            if (!memoryCache.TryGetValue(nameof(GetAllImages), out result))
            {
                result = client.Images.ListImagesAsync(new ImagesListParameters()).Result.ToList();
                memoryCache.Set(nameof(GetAllImages), result, TimeSpan.FromSeconds(10));
            }
            return result;
        }

        public List<ContainerListResponse> GetAllContainers()
        {
            List<ContainerListResponse> result;
            if (!memoryCache.TryGetValue(nameof(GetAllContainers), out result))
            {
                result = client.Containers.ListContainersAsync(new ContainersListParameters()).Result.ToList();
                memoryCache.Set(nameof(GetAllContainers), result, TimeSpan.FromSeconds(10));
            }
            return result;
        }

        public List<ContainerFileSystemChangeResponse> GetContainerChanges(string id)
        {
            return client.Containers.InspectChangesAsync(id).Result.ToList();
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

        public void RunCommandContainer(string id, ContainerExecCreateParameters dto)
        {
            var result = client.Exec.ExecCreateContainerAsync(id, dto).Result;
            client.Exec.StartContainerExecAsync(result.ID);
        }

        public void Deploy(DockerCompose compose)
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), "ContainerManagement", Guid.NewGuid().ToString());
            
            File.WriteAllText()
        }
    }
}
