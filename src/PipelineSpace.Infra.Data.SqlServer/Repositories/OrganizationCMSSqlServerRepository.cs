using Microsoft.EntityFrameworkCore;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Data.SqlServer.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.SqlServer.Repositories
{
    public class OrganizationCMSSqlServerRepository : Repository<OrganizationCMS>, IOrganizationCMSRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public OrganizationCMSSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<OrganizationCMS> FindOrganizationCMSByTypeAndAccountName(ConfigurationManagementService type, string accountName)
        {
            return _context.OrganizationCMSs.FirstOrDefaultAsync(x => x.Type == type && 
                                                                 x.AccountName.Equals(accountName, StringComparison.InvariantCultureIgnoreCase) && 
                                                                 x.Status == EntityStatus.Active &&
                                                                 x.Organization.Status == EntityStatus.Active);
        }

        public Task<OrganizationCMS> FindOrganizationCMSById(Guid organizationCMSId)
        {
            return _context.OrganizationCMSs.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.OrganizationCMSId == organizationCMSId);
        }
    }
}
