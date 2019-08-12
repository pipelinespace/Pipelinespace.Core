using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectQueryService
    {
        Task<ProjectListRp> GetProjects(Guid organizationId);
        Task<ProjectWithServiceListRp> GetProjectsWithServices(Guid organizationId);
        Task<ProjectGetRp> GetProjectById(Guid organizationId, Guid projectId);
        Task<ProjectExternalGetRp> GetProjectExternalById(Guid organizationId, Guid projectId);
        Task<ProjectEnvironmentSummaryGetRp> GetProjectEnvironmentSummayById(Guid organizationId, Guid projectId, Guid? environmentId);
    }
}
