using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
    [Route("{api}/organizations")]
    public class ProjectActivityController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectActivityQueryService _projectActivityQueryService;

        public ProjectActivityController(IDomainManagerService domainManagerService,
                                         IProjectActivityQueryService projectActivityQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectActivityQueryService = projectActivityQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/activities")]
        public async Task<IActionResult> GetProjectActivities(Guid organizationId, Guid projectId)
        {
            var projectActivities = await _projectActivityQueryService.GetProjectActivities(organizationId, projectId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projectActivities);
        }
    }
}
