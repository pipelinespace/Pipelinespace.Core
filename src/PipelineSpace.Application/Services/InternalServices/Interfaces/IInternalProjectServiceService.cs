using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices.Interfaces
{
    public interface IInternalProjectServiceService
    {
        Task ActivateProjectService(Guid organizationId, Guid projectId, Guid serviceId);
        Task PatchProjectService(Guid organizationId, Guid projectId, Guid serviceId, ProjectServicePatchtRp resource);
    }
}
