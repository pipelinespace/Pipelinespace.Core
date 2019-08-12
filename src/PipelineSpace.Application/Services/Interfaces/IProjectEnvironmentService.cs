using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectEnvironmentService
    {
        Task CreateProjectEnvironment(Guid organizationId, Guid projectId, ProjectEnvironmentPostRp resource);
        Task CreateProjectEnvironmentVariables(Guid organizationId, Guid projectId, Guid environmentId, ProjectEnvironmentVariablePostRp resource);
        Task DeleteProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId);
        Task ActivateProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId);
        Task InactivateProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId);
        Task CreateReleaseProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId);
        Task SortProjectEnvironments(Guid organizationId, Guid projectId, ProjectEnvironmentSortPostRp resource);
    }
}
