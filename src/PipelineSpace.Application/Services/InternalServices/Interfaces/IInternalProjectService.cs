using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices.Interfaces
{
    public interface IInternalProjectService
    {
        Task PatchProject(Guid organizationId, Guid projectId, ProjectPatchRp resource);
        Task ActivateProject(Guid organizationId, Guid projectId);
        Task CreateVSTSFakeProject(Guid organizationId, Guid projectId);
    }
}
