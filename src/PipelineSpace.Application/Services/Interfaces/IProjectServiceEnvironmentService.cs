using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectServiceEnvironmentService
    {
        Task CreateProjectServiceEnvironmentVariables(Guid organizationId, Guid projectId, Guid serviceId, Guid environmentId, ProjectServiceEnvironmentVariablePostRp resource);
        Task CreateProjectServiceEnvironmentRelease(Guid organizationId, Guid projectId, Guid serviceId, Guid environmentId);
    }
}
