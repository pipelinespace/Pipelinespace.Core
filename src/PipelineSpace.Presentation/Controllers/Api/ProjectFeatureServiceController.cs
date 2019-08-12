using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Presentation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
    [Route("{api}/organizations")]
    public class ProjectFeatureServiceController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectFeatureServiceService _projectFeatureServiceService;
        readonly IProjectFeatureServiceQueryService _projectFeatureServiceQueryService;

        public ProjectFeatureServiceController(IDomainManagerService domainManagerService, 
                                               IProjectFeatureServiceService projectFeatureServiceService, 
                                               IProjectFeatureServiceQueryService projectFeatureServiceQueryService): base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectFeatureServiceService = projectFeatureServiceService;
            _projectFeatureServiceQueryService = projectFeatureServiceQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services")]
        public async Task<IActionResult> GetProjectFeatureServices(Guid organizationId, Guid projectId, Guid featureId)
        {
            var featureServices = await _projectFeatureServiceQueryService.GetProjectFeatureServices(organizationId, projectId, featureId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(featureServices);
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/summary")]
        public async Task<IActionResult> GetProjectFeatureServiceSummaryById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            var featureServiceSummary = await _projectFeatureServiceQueryService.GetProjectFeatureServiceSummaryById(organizationId, projectId, featureId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(featureServiceSummary);
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/pipeline")]
        public async Task<IActionResult> GetProjectFeatureServicePipelineById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            var featureServicePipeline = await _projectFeatureServiceQueryService.GetProjectFeatureServicePipelineById(organizationId, projectId, featureId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(featureServicePipeline);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services")]
        [ActionProfile(Name = "all")]
        public async Task<IActionResult> GetProjectFeatureAllServices(Guid organizationId, Guid projectId, Guid featureId)
        {
            var featureServices = await _projectFeatureServiceQueryService.GetProjectFeatureAllServices(organizationId, projectId, featureId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasForbidden())
            {
                return this.Forbidden(_domainManagerService.GetForbidden());
            }

            return this.Ok(featureServices);
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services")]
        public async Task<IActionResult> CreateProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, [FromBody]ProjectFeatureServicePostRp projectFeatureServicePostRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectFeatureServiceService.CreateProjectFeatureService(organizationId, projectId, featureId, projectFeatureServicePostRp);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}")]
        public async Task<IActionResult> DeleteProjectService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectFeatureServiceService.DeleteProjectFeatureService(organizationId, projectId, featureId, serviceId);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/builds")]
        public async Task<IActionResult> CreateBuildProjectService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectFeatureServiceService.CreateBuildProjectFeatureService(organizationId, projectId, featureId, serviceId);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/releases")]
        public async Task<IActionResult> CreateReleaseProjectService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectFeatureServiceService.CreateReleaseProjectFeatureService(organizationId, projectId, featureId, serviceId);

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
