using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IProjectFeatureServiceActivityRepository : IRepository<ProjectFeatureServiceActivity>
    {
        Task<ProjectFeatureServiceActivity> GetProjectFeatureServiceActivityById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, string code);
    }
}
