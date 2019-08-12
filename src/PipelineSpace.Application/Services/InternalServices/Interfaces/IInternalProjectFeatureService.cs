using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices.Interfaces
{
    public interface IInternalProjectFeatureService
    {
        Task ActivateProjectFeature(Guid organizationId, Guid projectId, Guid featureId);
        Task PatchProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, ProjectFeatureServicePatchtRp resource);
    }
}
