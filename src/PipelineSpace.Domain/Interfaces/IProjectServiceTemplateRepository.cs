using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IProjectServiceTemplateRepository : IRepository<ProjectServiceTemplate>
    {
        Task<ProjectServiceTemplate> GetPendingProjectServiceTemplateById(Guid projectServiceTemplateId);
        Task<ProjectServiceTemplate> GetProjectServiceTemplateById(Guid projectServiceTemplateId);
        Task<List<ProjectServiceTemplate>> GetProjectServiceTemplates(ConfigurationManagementService gitProviderType, CloudProviderService cloudProviderType, PipeType pipeType);
        Task<ProjectServiceTemplate> GetProjectServiceTemplateInternalById(Guid projectServiceTemplateId);
        Task<List<ProjectServiceTemplate>> GetProjectServiceTemplateInternals(Guid programmingLanguageId, CloudProviderService cloudProviderType);
    }
}
