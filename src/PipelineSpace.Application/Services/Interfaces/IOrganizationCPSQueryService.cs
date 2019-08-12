using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IOrganizationCPSQueryService
    {
        Task<OrganizationCPSListRp> GetOrganizationCloudProviderServices(Guid organizationId);
        Task<OrganizationCPSGetRp> GetOrganizationCloudProviderServiceById(Guid organizationId, Guid organizationCPSId);
    }
}
