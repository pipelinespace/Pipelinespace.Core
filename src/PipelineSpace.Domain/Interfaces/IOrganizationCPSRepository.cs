using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IOrganizationCPSRepository : IRepository<OrganizationCPS>
    {
        Task<OrganizationCPS> FindOrganizationCPSById(Guid organizationCPSId);
    }
}
