using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class ProgrammingLanguageQueryService : IProgrammingLanguageQueryService
    {
        readonly IProgrammingLanguageRepository _programmingLanguageRepository;

        public ProgrammingLanguageQueryService(IProgrammingLanguageRepository programmingLanguageRepository)
        {
            _programmingLanguageRepository = programmingLanguageRepository;
        }

        public async Task<ProgrammingLanguageListRp> GetProgrammingLanguages()
        {
            var programmingLanguages = await _programmingLanguageRepository.GetProgrammingLanguages();

            ProgrammingLanguageListRp list = new ProgrammingLanguageListRp
            {
                Items = programmingLanguages.Select(x => new ProgrammingLanguageListItemRp()
                {
                    ProgrammingLanguageId = x.ProgrammingLanguageId,
                    Name = x.Name,
                    Description = x.Description
                }).ToList()
            };

            return list;
        }
    }
}
