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
    [Route("{api}/projecttemplates")]
    public class ProjectTemplateController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectTemplateQueryService _projectTemplateQueryService;

        public ProjectTemplateController(IDomainManagerService domainManagerService,
                                         IProjectTemplateQueryService projectTemplateQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectTemplateQueryService = projectTemplateQueryService;
        }

        [HttpGet]
        [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
        public async Task<IActionResult> GetProjectTemplates([FromQuery(Name = "cloudProviderType")]CloudProviderService cloudProviderType)
        {
            var projectTemplates = await _projectTemplateQueryService.GetProjectTemplates(cloudProviderType);
            return this.Ok(projectTemplates);
        }
    }
}
