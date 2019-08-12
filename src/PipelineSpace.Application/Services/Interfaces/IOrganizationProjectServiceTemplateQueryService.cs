using PipelineSpace.Application.Models;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IOrganizationProjectServiceTemplateQueryService
    {
        Task<OrganizationProjectServiceTemplateListRp> GetAllOrganizationProjectServiceTemplates(Guid organizationId);
        Task<OrganizationProjectServiceTemplateListRp> GetOrganizationProjectServiceTemplates(Guid organizationId, CloudProviderService cloudProviderType, PipeType pipeType);
    }
}
