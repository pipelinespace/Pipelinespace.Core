using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectServiceActivityQueryService
    {
        Task<ProjectServiceActivityListRp> GetProjectServiceActivities(Guid organizationId, Guid projectId, Guid serviceId);
    }
}
