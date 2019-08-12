using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectFeatureService
    {
        Task CreateProjectFeature(Guid organizationId, Guid projectId, ProjectFeaturePostRp resource);
        Task DeleteProjectFeature(Guid organizationId, Guid projectId, Guid featureId);
        Task CompleteProjectFeature(Guid organizationId, Guid projectId, Guid featureId, bool deleteInfrastructure);
    }
}
