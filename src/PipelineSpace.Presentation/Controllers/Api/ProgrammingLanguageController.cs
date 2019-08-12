using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Authorize(Roles = "globaladmin")]
    [Route("{api}/programminglanguages")]
    public class ProgrammingLanguageController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProgrammingLanguageQueryService _programmingLanguageQueryService;

        public ProgrammingLanguageController(IDomainManagerService domainManagerService,
                                             IProgrammingLanguageQueryService programmingLanguageQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _programmingLanguageQueryService = programmingLanguageQueryService;
        }

        [HttpGet]
        [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
        public async Task<IActionResult> GetProjectServiceTemplates()
        {
            var programmingLanguages = await _programmingLanguageQueryService.GetProgrammingLanguages();
            return this.Ok(programmingLanguages);
        }
    }
}
