using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectFeatureServiceQueryService
    {
        Task<ProjectFeatureServiceListRp> GetProjectFeatureServices(Guid organizationId, Guid projectId, Guid featureId);
        Task<ProjectFeatureAllServiceListRp> GetProjectFeatureAllServices(Guid organizationId, Guid projectId, Guid featureId);
        Task<ProjectFeatureServiceSummaryGetRp> GetProjectFeatureServiceSummaryById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId);
        Task<ProjectFeatureServicePipelineGetRp> GetProjectFeatureServicePipelineById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId);
        Task<ProjectFeatureServiceExternalGetRp> GetProjectFeatureServiceExternalById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId);

    }
}
