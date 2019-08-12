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
    public class ProjectEnvironmentController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectEnvironmentService _projectEnvironmentService;
        readonly IProjectEnvironmentQueryService _projectEnvironmentQueryService;

        public ProjectEnvironmentController(IDomainManagerService domainManagerService,
                                            IProjectEnvironmentService projectEnvironmentService,
                                            IProjectEnvironmentQueryService projectEnvironmentQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectEnvironmentService = projectEnvironmentService;
            _projectEnvironmentQueryService = projectEnvironmentQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/environments")]
        public async Task<IActionResult> GetProjectEnvironments(Guid organizationId, Guid projectId)
        {
            var projectEnvironments = await _projectEnvironmentQueryService.GetProjectEnvironments(organizationId, projectId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projectEnvironments);
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/environments/{environmentId:guid}")]
        public async Task<IActionResult> GetProjectEnvironmentById(Guid organizationId, Guid projectId, Guid environmentId)
        {
            var environment = await _projectEnvironmentQueryService.GetProjectEnvironmentById(organizationId, projectId, environmentId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (environment == null)
                return this.NotFound();

            return this.Ok(environment);
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/environments")]
        public async Task<IActionResult> CreateProjectEnvironment(Guid organizationId, Guid projectId, [FromBody]ProjectEnvironmentPostRp projectEnvironmentRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectEnvironmentService.CreateProjectEnvironment(organizationId, projectId, projectEnvironmentRp);

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

            return this.Ok(new { environmentId = await _domainManagerService.GetResult<Guid>("EnvironmentId") });
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/environments/sort")]
        public async Task<IActionResult> SortProjectEnvironments(Guid organizationId, Guid projectId, [FromBody]ProjectEnvironmentSortPostRp projectEnvironmentSortRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectEnvironmentService.SortProjectEnvironments(organizationId, projectId, projectEnvironmentSortRp);

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

        [HttpDelete]
        [Route("{organizationId:guid}/projects/{projectId:guid}/environments/{environmentId:guid}")]
        public async Task<IActionResult> DeleteProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectEnvironmentService.DeleteProjectEnvironment(organizationId, projectId, environmentId);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/environments/{environmentId:guid}/activate")]
        public async Task<IActionResult> ActiveProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectEnvironmentService.ActivateProjectEnvironment(organizationId, projectId, environmentId);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/environments/{environmentId:guid}/inactivate")]
        public async Task<IActionResult> InactiveProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectEnvironmentService.InactivateProjectEnvironment(organizationId, projectId, environmentId);
            
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
        [Route("{organizationId:guid}/projects/{projectId:guid}/environments/{environmentId:guid}/releases")]
        public async Task<IActionResult> CreateReleaseProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectEnvironmentService.CreateReleaseProjectEnvironment(organizationId, projectId, environmentId);

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
