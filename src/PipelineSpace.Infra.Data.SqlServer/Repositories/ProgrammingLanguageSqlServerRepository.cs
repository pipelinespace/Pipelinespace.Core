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
    public class ProgrammingLanguageSqlServerRepository : Repository<ProgrammingLanguage>, IProgrammingLanguageRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public ProgrammingLanguageSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ProgrammingLanguage>> GetProgrammingLanguages()
        {
            return await _context.ProgrammingLanguages.Where(x=> x.Status == EntityStatus.Active).ToListAsync();
        }

        public async Task<ProgrammingLanguage> GetProgrammingLanguageById(Guid programmingLanguageId)
        {
            return await _context.ProgrammingLanguages.FirstOrDefaultAsync(x=> x.ProgrammingLanguageId == programmingLanguageId && x.Status == EntityStatus.Active);
        }
        
    }
}
