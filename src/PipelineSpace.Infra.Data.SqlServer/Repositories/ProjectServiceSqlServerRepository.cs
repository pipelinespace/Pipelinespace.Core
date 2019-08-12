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
    public class ProjectServiceSqlServerRepository : Repository<ProjectService>, IProjectServiceRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public ProjectServiceSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProjectService> GetProjectServiceById(Guid organizationId, Guid projectId, Guid serviceId)
        {
            return await _context.ProjectServices.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.ProjectServiceId == serviceId);
        }
    }
}
