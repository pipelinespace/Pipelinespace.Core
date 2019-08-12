using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.PublicServices.Interfaces
{
    public interface IPublicProjectFeatureServiceEventService
    {
        Task CreateProjectFeatureServiceEvent(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, ProjectFeatureServiceEventPostRp resource);
    }
}
