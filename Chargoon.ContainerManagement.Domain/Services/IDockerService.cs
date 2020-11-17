using Chargoon.ContainerManagement.Domain.Dtos;
using Chargoon.ContainerManagement.Domain.Dtos.Dockers;
using Docker.DotNet.Models;
using System.Collections.Generic;
using System.IO;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface IDockerService
    {
        GetArchiveFromContainerResponse ArchiveContainer(string id, GetArchiveFromContainerParameters dto);
        void Deploy(string stackName, string dockerCompose);
        void ExtractArchiveToContainerAsync(string id, ContainerPathStatParameters dto, Stream stream);
        IEnumerable<ContainerListResponse> GetAllContainer();
        IEnumerable<ImagesListResponse> GetAllImage();
        IEnumerable<NodeListResponse> GetAllNode();
        IEnumerable<SwarmService> GetAllService();
        IEnumerable<SwarmService> GetAllServiceByPrefix(string stackName);
        IEnumerable<ContainerFileSystemChangeResponse> GetContainerChanges(string id);
        Stream GetContainerExport(string id);
        ContainerInspectResponse GetContainerInspect(string id);
        string GetContainerLog(string id);
        ContainerProcessesResponse GetContainerProcesses(string id);
        void KillContainer(string id);
        void PauseContainer(string id);
        void RemoveContainer(string id);
        void RenameContainer(string id, ContainerRenameParameters dto);
        void RestartContainer(string id);
        bool StartContainer(string id);
        bool StopContainer(string id);
        void SystemPrune(bool all = false);
        void Undeploy(string stackName, string prefix);
        void UnPauseContainer(string id);
        string ExecCommandContainer(string id, string command);
        ContainerUpdateResponse UpdateContainer(string id, ContainerUpdateParameters dto);
        IEnumerable<ContainerExecInspectResponse> GetAllContainerCommand(string id);
        ContainerExecInspectResponse GetContainerExecInspect(string id);
        string GetExecCommandContainerLog(string id);
        void ClearExitedCommands();
        void Build(string dockerCompose);
        IEnumerable<ContainerListResponse> GetAllContainerByPrefix(string prefix);
		void ContainerPrune(bool all = false);
		IEnumerable<NetworkResponse> GetAllNetwork();
	}
}
