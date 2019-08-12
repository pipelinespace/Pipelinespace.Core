using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IOrganizationCMSRepository : IRepository<OrganizationCMS>
    {
        Task<OrganizationCMS> FindOrganizationCMSById(Guid organizationCMSId);
        Task<OrganizationCMS> FindOrganizationCMSByTypeAndAccountName(ConfigurationManagementService type, string accountName);
    }
}
