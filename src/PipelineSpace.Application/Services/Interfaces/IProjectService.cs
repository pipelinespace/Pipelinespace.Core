using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectService
    {
        Task CreateProject(Guid organizationId, ProjectPostRp resource);
        Task ImportProject(Guid organizationId, ProjectImportPostRp resource);
        Task UpdateProjectBasicInformation(Guid organizationId, Guid projectId, ProjectPutRp resource);
        Task DeleteProject(Guid organizationId, Guid projectId);
    }
}
