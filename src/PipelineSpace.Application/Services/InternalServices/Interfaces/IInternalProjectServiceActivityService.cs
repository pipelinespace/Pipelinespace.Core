using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices.Interfaces
{
    public interface IInternalProjectServiceActivityService
    {
        Task UpdateProjectServiceActivity(Guid organizationId, Guid projectId, Guid serviceId, string code, ProjectServiceActivityPutRp resource);
    }
}
