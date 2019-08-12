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
    public class ProjectServiceTemplateSqlServerRepository : Repository<ProjectServiceTemplate>, IProjectServiceTemplateRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public ProjectServiceTemplateSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProjectServiceTemplate> GetPendingProjectServiceTemplateById(Guid projectServiceTemplateId)
        {
            return await _context.ProjectServiceTemplates.FirstOrDefaultAsync(x => x.ProjectServiceTemplateId == projectServiceTemplateId &&
                                                                              x.TemplateType == Domain.Models.Enums.TemplateType.Standard &&
                                                                              x.Status == EntityStatus.Preparing);
        }

        public async Task<ProjectServiceTemplate> GetProjectServiceTemplateById(Guid projectServiceTemplateId)
        {
            return await _context.ProjectServiceTemplates.FirstOrDefaultAsync(x => x.ProjectServiceTemplateId == projectServiceTemplateId && 
                                                                              x.TemplateType == Domain.Models.Enums.TemplateType.Standard &&
                                                                              x.Status == EntityStatus.Active);
        }

        public async Task<ProjectServiceTemplate> GetProjectServiceTemplateInternalById(Guid projectServiceTemplateId)
        {
            return await _context.ProjectServiceTemplates.FirstOrDefaultAsync(x => x.ProjectServiceTemplateId == projectServiceTemplateId && 
                                                                              x.TemplateType == Domain.Models.Enums.TemplateType.Internal && 
                                                                              x.Status == EntityStatus.Active);
        }

        public async Task<List<ProjectServiceTemplate>> GetProjectServiceTemplates(ConfigurationManagementService gitProviderType, CloudProviderService cloudProviderType, PipeType pipeType)
        {
            return await _context.ProjectServiceTemplates.Where(x=> x.ServiceCPSType == cloudProviderType &&
                                                                    x.PipeType == pipeType &&
                                                                    x.TemplateType == Domain.Models.Enums.TemplateType.Standard &&
                                                                    x.TemplateAccess == Domain.Models.Enums.TemplateAccess.System &&
                                                                    x.Status == EntityStatus.Active).ToListAsync();
        }
        
        public async Task<List<ProjectServiceTemplate>> GetProjectServiceTemplateInternals(Guid programmingLanguageId, CloudProviderService cloudProviderType)
        {
            return await _context.ProjectServiceTemplates.Where(x => x.ProgrammingLanguageId == programmingLanguageId &&
                                                                    x.ServiceCPSType == cloudProviderType &&
                                                                    x.TemplateType == Domain.Models.Enums.TemplateType.Internal &&
                                                                    x.Status == EntityStatus.Active).ToListAsync();
        }
    }
}
