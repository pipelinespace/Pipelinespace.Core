using PipelineSpace.Application.Models;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectServiceEventQueryService
    {
        Task<ProjectServiceEventListRp> GetProjectServiceEvents(Guid organizationId, Guid projectId, Guid serviceId, BaseEventType baseEventType);
    }
}
