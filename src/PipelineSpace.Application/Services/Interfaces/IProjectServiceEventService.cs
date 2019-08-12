using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectServiceEventService
    {
        Task CreateProjectServiceEvent(Guid organizationId, Guid projectId, Guid serviceId, ProjectServiceEventPostRp resource);
    }
}
