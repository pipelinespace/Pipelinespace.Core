using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectFeatureServiceDeliveryQueryService
    {
        Task<ProjectFeatureServiceDeliveryListRp> GetProjectFeatureServiceDeliveries(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId);
    }
}
