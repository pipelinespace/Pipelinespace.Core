using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectServiceEnvironmentQueryService
    {
        Task<ProjectServiceEnvironmentListRp> GetProjectServiceEnvironments(Guid organizationId, Guid projectId, Guid serviceId);
    }
}
