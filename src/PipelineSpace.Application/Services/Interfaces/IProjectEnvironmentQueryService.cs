using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectEnvironmentQueryService
    {
        Task<ProjectEnvironmentListRp> GetProjectEnvironments(Guid organizationId, Guid projectId);
        Task<ProjectEnvironmentGetRp> GetProjectEnvironmentById(Guid organizationId, Guid projectId, Guid environmentId);
        Task<ProjectEnvironmentVariableListRp> GetProjectEnvironmentVariables(Guid organizationId, Guid projectId, Guid environmentId);
    }
}
