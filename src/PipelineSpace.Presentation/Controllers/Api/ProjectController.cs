using IdentityServer4.AccessTokenValidation;
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
    public class ProjectController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectService _projectService;
        readonly IProjectQueryService _projectQueryService;

        public ProjectController(IDomainManagerService domainManagerService,
                                 IProjectService projectService,
                                 IProjectQueryService projectQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectService = projectService;
            _projectQueryService = projectQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects")]
        public async Task<IActionResult> GetProjects(Guid organizationId)
        {
            var projects = await _projectQueryService.GetProjects(organizationId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projects);
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}")]
        public async Task<IActionResult> GetProjectById(Guid organizationId, Guid projectId)
        {
            var project = await _projectQueryService.GetProjectById(organizationId, projectId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (project == null)
                return this.NotFound();

            return this.Ok(project);
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/environmentsummary")]
        public async Task<IActionResult> GetProjectEnvironmentSummaryById(Guid organizationId, Guid projectId, [FromQuery(Name = "environmentId")]Guid? environmentId)
        {
            var projectEnvironmentSummary = await _projectQueryService.GetProjectEnvironmentSummayById(organizationId, projectId, environmentId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (projectEnvironmentSummary == null)
                return this.NotFound();

            return this.Ok(projectEnvironmentSummary);
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects")]
        public async Task<IActionResult> CreateProject(Guid organizationId, [FromBody]ProjectPostRp projectRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectService.CreateProject(organizationId, projectRp);

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

            return this.Ok(new { ProjectId = await _domainManagerService.GetResult<Guid>("ProjectId") });
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/imports")]
        public async Task<IActionResult> ImportProject(Guid organizationId, [FromBody]ProjectImportPostRp projectRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);
            
            await _projectService.ImportProject(organizationId, projectRp);

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

            return this.Ok(new { ProjectId = await _domainManagerService.GetResult<Guid>("ProjectId") });
        }

        [HttpPut]
        [Route("{organizationId:guid}/projects/{projectId:guid}/basicinformation")]
        public async Task<IActionResult> UpdateProjectBasicInformation(Guid organizationId, Guid projectId, [FromBody]ProjectPutRp projectRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectService.UpdateProjectBasicInformation(organizationId, projectId, projectRp);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}")]
        public async Task<IActionResult> DeleteProject(Guid organizationId, Guid projectId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectService.DeleteProject(organizationId, projectId);

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
