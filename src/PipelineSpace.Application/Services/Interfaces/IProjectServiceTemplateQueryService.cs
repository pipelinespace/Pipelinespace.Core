using PipelineSpace.Application.Models;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectServiceTemplateQueryService
    {
        Task<ProjectServiceTemplateListRp> GetProjectServiceTemplates(ConfigurationManagementService gitProviderType, CloudProviderService cloudProviderType, PipeType pipeType);
        Task<ProjectServiceTemplateListRp> GetProjectServiceTemplateInternals(Guid programmingLanguageId, CloudProviderService cloudProviderType);
        Task<ProjectServiceTemplateDefinitionGetRp> GetProjectServiceTemplateDefinition(Guid projectServiceTemplateId);

    }
}
