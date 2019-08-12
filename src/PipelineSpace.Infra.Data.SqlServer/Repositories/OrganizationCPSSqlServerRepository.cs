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
    public class OrganizationCPSSqlServerRepository : Repository<OrganizationCPS>, IOrganizationCPSRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public OrganizationCPSSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<OrganizationCPS> FindOrganizationCPSById(Guid organizationCPSId)
        {
            return _context.OrganizationCPSs.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.OrganizationCPSId == organizationCPSId);
        }
    }
}
