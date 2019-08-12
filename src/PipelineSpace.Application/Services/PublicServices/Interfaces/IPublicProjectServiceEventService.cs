using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.PublicServices.Interfaces
{
    public interface IPublicProjectServiceEventService
    {
        Task CreateProjectServiceEvent(Guid organizationId, Guid projectId, Guid serviceId, ProjectServiceEventPostRp resource);
    }
}
