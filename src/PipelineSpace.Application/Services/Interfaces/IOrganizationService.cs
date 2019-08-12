using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task CreateOrganization(OrganizationPostRp resource);
        Task UpdateOrganization(Guid id, OrganizationPutRp resource);
        Task DeleteOrganization(Guid organizationId);
    }
}
