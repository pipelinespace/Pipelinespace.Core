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
    public class ProjectEnvironmentVariableController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectEnvironmentService _projectEnvironmentService;
        readonly IProjectEnvironmentQueryService _projectEnvironmentQueryService;

        public ProjectEnvironmentVariableController(IDomainManagerService domainManagerService,
                                                    IProjectEnvironmentService projectEnvironmentService,
                                                    IProjectEnvironmentQueryService projectEnvironmentQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectEnvironmentService = projectEnvironmentService;
            _projectEnvironmentQueryService = projectEnvironmentQueryService;
        }


        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/environments/{environmentId:guid}/variables")]
        public async Task<IActionResult> GetProjectEnvironmentVariables(Guid organizationId, Guid projectId, Guid environmentId)
        {
            var environmentVariables = await _projectEnvironmentQueryService.GetProjectEnvironmentVariables(organizationId, projectId, environmentId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok(environmentVariables);
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/environments/{environmentId:guid}/variables")]
        public async Task<IActionResult> CreateProjectEnvironmentVariables(Guid organizationId, Guid projectId, Guid environmentId, [FromBody]ProjectEnvironmentVariablePostRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectEnvironmentService.CreateProjectEnvironmentVariables(organizationId, projectId, environmentId, resource);

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
