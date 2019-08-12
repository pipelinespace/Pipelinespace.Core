using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IProgrammingLanguageRepository : IRepository<ProgrammingLanguage>
    {
        Task<List<ProgrammingLanguage>> GetProgrammingLanguages();
        Task<ProgrammingLanguage> GetProgrammingLanguageById(Guid programmingLanguageId);
    }
}
