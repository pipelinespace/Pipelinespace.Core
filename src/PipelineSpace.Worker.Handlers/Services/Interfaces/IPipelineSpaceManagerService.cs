using PipelineSpace.Worker.Handlers.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services.Interfaces
{
    public interface IPipelineSpaceManagerService
    {
        Task<string> CreateRepository(CreateRepositoryOptions @options);
        Task<string> GetReleaseDefinition(CreateRepositoryOptions @options, string buildDefinition);
        Task CreateOrganizationRepository(CreateOrganizationRepositoryOptions @options);
        Task<string> CreateBranch(CreateBranchOptions @options);
        Task DeleteBranch(DeleteBranchOptions @options);
        Task<GetQueueResult> GetQueue(GetQueueOptions @options);
        Task<int> CreateBuildDefinition(CreateBuildDefinitionOptions @options);
        Task DeleteBuildDefinition(DeleteBuildDefinitionOptions @options);
        Task<Guid> CreateServiceHook(CreateServiceHookOptions @options);
        Task DeleteServiceHook(DeleteServiceHookOptions @options);
        Task<CMSVSTSReleaseDefinitionReadModel> GetReleaseDefinition(ReadReleaseDefinitionOptions @options);
        Task<int?> CreateReleaseDefinition(CreateReleaseDefinitionOptions @options);
        Task<int?> CreateReleaseDefinitionFromBaseDefinition(CreateReleaseDefinitionOptions @options);
        Task DeleteReleaseDefinition(DeleteReleaseDefinitionOptions @options);
        Task UpdateReleaseDefinition(UpdateReleaseDefinitionOptions @options);
        Task QueueBuild(QueueBuildOptions @options);
        Task QueueRelease(QueueReleaseOptions @options);
    }
}
