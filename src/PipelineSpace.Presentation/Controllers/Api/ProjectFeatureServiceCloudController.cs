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
    public class ProjectFeatureServiceCloudController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectFeatureServiceCloudQueryService _projectServiceCloudQueryService;

        public ProjectFeatureServiceCloudController(IDomainManagerService domainManagerService,
                                                    IProjectFeatureServiceCloudQueryService projectFeatureServiceCloudQueryService): base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectServiceCloudQueryService = projectFeatureServiceCloudQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/cloudsummary")]
        public async Task<IActionResult> GetProjectServiceEvents(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            var summary = await _projectServiceCloudQueryService.GetProjectFeatureServiceCloudSummary(organizationId, projectId, featureId, serviceId);

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
