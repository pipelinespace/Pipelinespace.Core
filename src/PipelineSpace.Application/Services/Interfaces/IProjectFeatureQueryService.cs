using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectFeatureQueryService
    {
        Task<ProjectFeatureListRp> GetProjectFeatures(Guid organizationId, Guid projectId);
        Task<ProjectFeatureGetRp> GetProjectFeatureById(Guid organizationId, Guid projectId, Guid featureId);
    }
}
