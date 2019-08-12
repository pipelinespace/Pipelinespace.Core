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
    public class ProjectFeatureServiceActivitySqlServerRepository : Repository<ProjectFeatureServiceActivity>, IProjectFeatureServiceActivityRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public ProjectFeatureServiceActivitySqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProjectFeatureServiceActivity> GetProjectFeatureServiceActivityById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId,string code)
        {
            return await _context.ProjectFeatureServiceActivities.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.ProjectFeatureId == featureId && x.ProjectServiceId == serviceId && x.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase) && x.ActivityStatus != Domain.Models.Enums.ActivityStatus.Succeed);
        }
    }
}
