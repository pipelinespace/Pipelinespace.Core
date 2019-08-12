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
    public class ProjectActivitySqlServerRepository : Repository<ProjectActivity>, IProjectActivityRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public ProjectActivitySqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProjectActivity> GetProjectActivityById(Guid organizationId, Guid projectId, string code)
        {
            return await _context.ProjectActivities.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase) && x.ActivityStatus != Domain.Models.Enums.ActivityStatus.Succeed);
        }
    }
}
