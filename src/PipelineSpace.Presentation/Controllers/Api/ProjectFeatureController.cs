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
    public class ProjectFeatureController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectFeatureService _projectFeatureService;
        readonly IProjectFeatureQueryService _projectFeatureQueryService;

        public ProjectFeatureController(IDomainManagerService domainManagerService,
                                        IProjectFeatureService projectFeatureService,
                                        IProjectFeatureQueryService projectFeatureQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectFeatureService = projectFeatureService;
            _projectFeatureQueryService = projectFeatureQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features")]
        public async Task<IActionResult> GetProjectFeatures(Guid organizationId, Guid projectId)
        {
            var projectFeatures = await _projectFeatureQueryService.GetProjectFeatures(organizationId, projectId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projectFeatures);
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}")]
        public async Task<IActionResult> GetProjectFeatureById(Guid organizationId, Guid projectId, Guid featureId)
        {
            var project = await _projectFeatureQueryService.GetProjectFeatureById(organizationId, projectId, featureId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (project == null)
                return this.NotFound();

            return this.Ok(project);
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features")]
        public async Task<IActionResult> CreateProjectFeature(Guid organizationId, Guid projectId, [FromBody]ProjectFeaturePostRp projectFeatureRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectFeatureService.CreateProjectFeature(organizationId, projectId, projectFeatureRp);

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

            return this.Ok(new { FeatureId = await _domainManagerService.GetResult<Guid>("FeatureId") });
        }

        [HttpDelete]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}")]
        public async Task<IActionResult> DeleteProjectFeature(Guid organizationId, Guid projectId, Guid featureId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectFeatureService.DeleteProjectFeature(organizationId, projectId, featureId);

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

        [HttpPatch]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/complete")]
        public async Task<IActionResult> CompleteProjectFeature(Guid organizationId, Guid projectId, Guid featureId, [FromQuery(Name = "deleteInfrastructure")]bool deleteInfrastructure)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectFeatureService.CompleteProjectFeature(organizationId, projectId, featureId, deleteInfrastructure);

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
