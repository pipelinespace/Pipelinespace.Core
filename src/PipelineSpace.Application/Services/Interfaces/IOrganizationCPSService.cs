using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IOrganizationCPSService
    {
        Task CreateCloudProviderService(Guid organizationId, OrganizationCPSPostRp resource);
        Task UpdateCloudProviderService(Guid organizationId, Guid organizationCPSId, OrganizationCPSPutRp resource);
        Task DeleteCloudProviderService(Guid organizationId, Guid organizationCPSId);
    }
}
