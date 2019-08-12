using Microsoft.EntityFrameworkCore;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Data.SqlServer.Contexts;
using System;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.SqlServer.Repositories
{
    public class ProjectServiceActivitySqlServerRepository : Repository<ProjectServiceActivity>, IProjectServiceActivityRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public ProjectServiceActivitySqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProjectServiceActivity> GetProjectServiceActivityById(Guid organizationId, Guid projectId, Guid serviceId,string code)
        {
            return await _context.ProjectServiceActivities.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.ProjectServiceId == serviceId && x.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase) && x.ActivityStatus != Domain.Models.Enums.ActivityStatus.Succeed);
        }
    }
}
