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
    public class ProjectServiceEnvironmentController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectServiceEnvironmentService _projectServiceEnvironmentService;
        readonly IProjectServiceEnvironmentQueryService _projectServiceEnvironmentQueryService;

        public ProjectServiceEnvironmentController(IDomainManagerService domainManagerService,
                                                   IProjectServiceEnvironmentService projectServiceEnvironmentService,
                                                   IProjectServiceEnvironmentQueryService projectServiceEnvironmentQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectServiceEnvironmentService = projectServiceEnvironmentService;
            _projectServiceEnvironmentQueryService = projectServiceEnvironmentQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/environments")]
        public async Task<IActionResult> GetProjectServiceEnvironments(Guid organizationId, Guid projectId, Guid serviceId)
        {
            var projectServiceEnvironments = await _projectServiceEnvironmentQueryService.GetProjectServiceEnvironments(organizationId, projectId, serviceId);
            
            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projectServiceEnvironments);
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/environments/{environmentId:guid}/variables")]
        public async Task<IActionResult> CreateProjectServiceEnvironmentVariables(Guid organizationId, Guid projectId, Guid serviceId, Guid environmentId, [FromBody]ProjectServiceEnvironmentVariablePostRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectServiceEnvironmentService.CreateProjectServiceEnvironmentVariables(organizationId, projectId, serviceId, environmentId, resource);

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

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/environments/{environmentId:guid}/releases")]
        public async Task<IActionResult> CreateProjectServiceEnvironmentRelease(Guid organizationId, Guid projectId, Guid serviceId, Guid environmentId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectServiceEnvironmentService.CreateProjectServiceEnvironmentRelease(organizationId, projectId, serviceId, environmentId);

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
