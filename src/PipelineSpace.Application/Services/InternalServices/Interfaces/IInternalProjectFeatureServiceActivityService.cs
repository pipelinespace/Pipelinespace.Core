using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices.Interfaces
{
    public interface IInternalProjectFeatureServiceActivityService
    {
        Task UpdateProjectFeatureServiceActivity(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, string code, ProjectFeatureServiceActivityPutRp resource);
    }
}
