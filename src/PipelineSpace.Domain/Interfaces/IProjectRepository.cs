using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> GetProjectById(Guid organizationId, Guid projectId);
        Task<Project> GetProjectWithOrgAndAccountOwnerByProjectId(Guid organizationId, Guid projectId);
        Task<List<ProjectService>> GetProjectServices(Guid organizationId, Guid projectId);
        Task<List<ProjectFeature>> GetProjectFeatures(Guid organizationId, Guid projectId);
        Task<List<ProjectEnvironment>> GetProjectEnvironments(Guid organizationId, Guid projectId);
    }
}
