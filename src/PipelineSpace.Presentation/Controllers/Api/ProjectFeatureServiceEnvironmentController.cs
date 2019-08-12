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
    public class ProjectFeatureServiceEnvironmentController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectFeatureServiceEnvironmentService _projectFeatureServiceEnvironmentService;
        readonly IProjectFeatureServiceEnvironmentQueryService _projectFeatureServiceEnvironmentQueryService;

        public ProjectFeatureServiceEnvironmentController(IDomainManagerService domainManagerService,
                                                          IProjectFeatureServiceEnvironmentService projectFeatureServiceEnvironmentService,
                                                          IProjectFeatureServiceEnvironmentQueryService projectFeatureServiceEnvironmentQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectFeatureServiceEnvironmentService = projectFeatureServiceEnvironmentService;
            _projectFeatureServiceEnvironmentQueryService = projectFeatureServiceEnvironmentQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/environments")]
        public async Task<IActionResult> GetProjectFeatureServiceEnvironments(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            var projectFeatureServiceEnvironments = await _projectFeatureServiceEnvironmentQueryService.GetFeatureProjectServiceEnvironments(organizationId, projectId, featureId, serviceId);
            
            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projectFeatureServiceEnvironments);
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/environments/{environmentId:guid}/variables")]
        public async Task<IActionResult> CreateProjectFeatureServiceEnvironmentVariables(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, Guid environmentId, [FromBody]ProjectFeatureServiceEnvironmentVariablePostRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectFeatureServiceEnvironmentService.CreateProjectFeatureServiceEnvironmentVariables(organizationId, projectId, featureId, serviceId, environmentId, resource);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasForbidden())
            {
                return this.Forbidden(_domainManagerService.GetForbidden());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok();
        }

    }
}
