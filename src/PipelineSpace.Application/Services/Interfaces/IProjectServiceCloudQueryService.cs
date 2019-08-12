using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectServiceCloudQueryService
    {
        Task<CPSCloudResourceSummaryModel> GetProjectServiceCloudSummary(Guid organizationId, Guid projectId, Guid serviceId);
    }
}
