using Microsoft.EntityFrameworkCore;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Data.SqlServer.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace PipelineSpace.Infra.Data.SqlServer.Repositories
{
    public class OrganizationSqlServerRepository : Repository<Organization>, IOrganizationRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public OrganizationSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<Organization> GetOrganizationById(Guid organizationId)
        {
            return await _context.Organizations.FirstOrDefaultAsync(x => x.OrganizationId == organizationId);
        }

        public async Task<List<Project>> GetProjects(Guid organizationId)
        {
            return await _context.Projects.IgnoreQueryFilters().Where(x => x.OrganizationId == organizationId && x.Status == EntityStatus.Active).ToListAsync();
        }
    }
}
