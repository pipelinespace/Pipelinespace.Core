using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectServiceService
    {
        Task CreateProjectService(Guid organizationId, Guid projectId, ProjectServicePostRp resource, string userId = null);
        Task ImportProjectService(Guid organizationId, Guid projectId, ProjectServiceImportPostRp resource, string userId = null);
        Task UpdateProjectService(Guid organizationId, Guid projectId, Guid serviceId, ProjectServicePutRp resource);
        Task DeleteProjectService(Guid organizationId, Guid projectId, Guid serviceId);
        Task CreateBuildProjectService(Guid organizationId, Guid projectId, Guid serviceId);
        Task CreateReleaseProjectService(Guid organizationId, Guid projectId, Guid serviceId);
        Task CompleteApprovalProjectService(Guid organizationId, Guid projectId, Guid serviceId, int approvalId, ProjectServiceApprovalPutRp resource);
    }
}
