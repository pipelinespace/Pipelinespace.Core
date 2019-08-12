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
    public class ProjectTemplateSqlServerRepository : Repository<ProjectTemplate>, IProjectTemplateRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public ProjectTemplateSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProjectTemplate> GetProjectTemplateById(Guid projectTemplateId)
        {
            return await _context.ProjectTemplates.FirstOrDefaultAsync(x => x.ProjectTemplateId == projectTemplateId && x.Status == EntityStatus.Active);
        }

        public async Task<List<ProjectTemplate>> GetProjectTemplates(CloudProviderService cloudProviderType)
        {
            return await _context.ProjectTemplates.Where(x=> x.CloudProviderType == cloudProviderType &&
                                                             x.Status == EntityStatus.Active).ToListAsync();
        }
    }
}
