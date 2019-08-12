using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IOrganizationCMSService
    {
        Task CreateConfigurationManagementService(Guid organizationId, OrganizationCMSPostRp resource);
        Task UpdateConfigurationManagementService(Guid organizationId, Guid organizationCMSId, OrganizationCMSPutRp resource);
        Task DeleteConfigurationManagementService(Guid organizationId, Guid organizationCMSId);
    }
}
