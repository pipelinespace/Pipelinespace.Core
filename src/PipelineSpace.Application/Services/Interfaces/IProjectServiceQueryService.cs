using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectServiceQueryService
    {
        Task<ProjectServiceListRp> GetProjectServices(Guid organizationId, Guid projectId);
        Task<ProjectServiceGetRp> GetProjectServiceById(Guid organizationId, Guid projectId, Guid serviceId);
        Task<ProjectServiceExternalGetRp> GetProjectServiceExternalById(Guid organizationId, Guid projectId, Guid serviceId);

        Task<ProjectServiceSummaryGetRp> GetProjectServiceSummaryById(Guid organizationId, Guid projectId, Guid serviceId);
        Task<ProjectServicePipelineGetRp> GetProjectServicePipelineById(Guid organizationId, Guid projectId, Guid serviceId);
        Task<ProjectServiceFeatureListRp> GetProjectServiceFeaturesById(Guid organizationId, Guid projectId, Guid serviceId);
    }
}
