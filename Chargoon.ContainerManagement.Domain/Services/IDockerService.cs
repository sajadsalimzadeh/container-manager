using Chargoon.ContainerManagement.Domain.Dtos;
using Docker.DotNet.Models;
using System.Collections.Generic;
using System.IO;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface IDockerService
    {
        GetArchiveFromContainerResponse ArchiveContainer(string id, GetArchiveFromContainerParameters dto);
        void ExtractArchiveToContainerAsync(string id, ContainerPathStatParameters dto, Stream stream);
        List<ContainerListResponse> GetAllContainers();
        List<ImagesListResponse> GetAllImages();
        List<ContainerFileSystemChangeResponse> GetContainerChanges(string id);
        Stream GetContainerExport(string id);
        ContainerInspectResponse GetContainerInspect(string id);
        string GetContainerLog(string id);
        ContainerProcessesResponse GetContainerProcesses(string id);
        void KillContainer(string id);
        void PauseContainer(string id);
        void RemoveContainer(string id);
        void RenameContainer(string id, ContainerRenameParameters dto);
        void RestartContainer(string id);
        void RunCommandContainer(string id, ContainerExecCreateParameters dto);
        bool StartContainer(string id);
        bool StopContainer(string id);
        void UnPauseContainer(string id);
        ContainerUpdateResponse UpdateContainer(string id, ContainerUpdateParameters dto);
    }
}
