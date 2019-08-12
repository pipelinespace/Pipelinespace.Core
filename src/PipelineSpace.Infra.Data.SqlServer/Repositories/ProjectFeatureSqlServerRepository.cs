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
    public class ProjectFeatureSqlServerRepository : Repository<ProjectFeature>, IProjectFeatureRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public ProjectFeatureSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProjectFeature> GetProjectFeatureById(Guid organizationId, Guid projectId, Guid featureId)
        {
            return await _context.ProjectFeatures.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.ProjectFeatureId == featureId);
        }

        public async Task<List<ProjectFeatureService>> GetProjectFeatureServices(Guid organizationId, Guid projectId, Guid featureId)
        {
            return await _context.ProjectFeatureServices.IgnoreQueryFilters().Where(x => x.ProjectFeatureId == featureId && x.Status == EntityStatus.Active).ToListAsync();
        }
    }
}
