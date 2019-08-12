using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices.Interfaces
{
    public interface IInternalProjectActivityService
    {
        Task UpdateProjectActivity(Guid organizationId, Guid projectId, string code, ProjectActivityPutRp resource);
    }
}
