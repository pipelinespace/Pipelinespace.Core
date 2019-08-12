using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectFeatureServiceService
    {
        Task CreateProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, ProjectFeatureServicePostRp resource);
        Task DeleteProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId);
        Task CreateBuildProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId);
        Task CreateReleaseProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId);
    }
}
