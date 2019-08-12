using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class ProjectServiceCloudController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectServiceCloudQueryService _projectServiceCloudQueryService;

        public ProjectServiceCloudController(IDomainManagerService domainManagerService, 
                                             IProjectServiceCloudQueryService projectServiceCloudQueryService): base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectServiceCloudQueryService = projectServiceCloudQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/cloudsummary")]
        public async Task<IActionResult> GetProjectServiceEvents(Guid organizationId, Guid projectId, Guid serviceId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            var summary = await _projectServiceCloudQueryService.GetProjectServiceCloudSummary(organizationId, projectId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok(summary);
        }
    }
}
