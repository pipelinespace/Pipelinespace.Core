using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IOrganizationProjectServiceTemplateService
    {
        Task CreateOrganizationProjectServiceTemplate(Guid organizationId, OrganizationProjectServiceTemplatePostRp resource);
        Task UpdateOrganizationProjectServiceTemplate(Guid organizationId, Guid projectServiceTemplateId, OrganizationProjectServiceTemplatePutRp resource);
        Task DeleteOrganizationProjectServiceTemplate(Guid organizationId, Guid projectServiceTemplateId);
    }
}
