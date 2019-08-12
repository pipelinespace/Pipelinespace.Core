using Microsoft.EntityFrameworkCore;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Data.SqlServer.Contexts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.SqlServer.Repositories
{
    public class ProjectSqlServerRepository : Repository<Project>, IProjectRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public ProjectSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Project> GetProjectById(Guid organizationId, Guid projectId)
        {
            return await _context.Projects.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.ProjectId == projectId);
        }

        public async Task<List<ProjectEnvironment>> GetProjectEnvironments(Guid organizationId, Guid projectId)
        {
            return await _context.ProjectEnvironments.IgnoreQueryFilters().Where(x => x.ProjectId == projectId && x.Status == EntityStatus.Active).ToListAsync();
        }

        public async Task<List<ProjectService>> GetProjectServices(Guid organizationId, Guid projectId)
        {
            return await _context.ProjectServices.IgnoreQueryFilters().Where(x => x.ProjectId == projectId && x.Status == EntityStatus.Active).ToListAsync();
        }

        public async Task<List<ProjectFeature>> GetProjectFeatures(Guid organizationId, Guid projectId)
        {
            return await _context.ProjectFeatures.IgnoreQueryFilters().Where(x => x.ProjectId == projectId && x.Status == EntityStatus.Active).ToListAsync();
        }

        public async Task<Project> GetProjectWithOrgAndAccountOwnerByProjectId(Guid organizationId, Guid projectId)
        {
            return await _context.Projects
                            .Include(x=> x.Organization)
                            .Include(x=> x.Owner).FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.ProjectId == projectId);
        }
        
    }
}
