using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IProjectFeatureRepository : IRepository<ProjectFeature>
    {
        Task<ProjectFeature> GetProjectFeatureById(Guid organizationId, Guid projectId, Guid featureId);
        Task<List<ProjectFeatureService>> GetProjectFeatureServices(Guid organizationId, Guid projectId, Guid featureId);
    }
}
