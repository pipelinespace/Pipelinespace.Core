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
    public class ProjectServiceController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectServiceService _projectServiceService;
        readonly IProjectServiceQueryService _projectServiceQueryService;

        public ProjectServiceController(IDomainManagerService domainManagerService,
                                        IProjectServiceService projectServiceService,
                                        IProjectServiceQueryService projectServiceQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectServiceService = projectServiceService;
            _projectServiceQueryService = projectServiceQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services")]
        public async Task<IActionResult> GetProjectServices(Guid organizationId, Guid projectId)
        {
            var projectServices = await _projectServiceQueryService.GetProjectServices(organizationId, projectId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projectServices);
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}")]
        public async Task<IActionResult> GetProjectServiceById(Guid organizationId, Guid projectId, Guid serviceId)
        {
            var projectService = await _projectServiceQueryService.GetProjectServiceById(organizationId, projectId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (projectService == null)
                return this.NotFound();

            return this.Ok(projectService);
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/summary")]
        public async Task<IActionResult> GetProjectServiceSummaryById(Guid organizationId, Guid projectId, Guid serviceId)
        {
            var projectServiceSummary = await _projectServiceQueryService.GetProjectServiceSummaryById(organizationId, projectId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (projectServiceSummary == null)
                return this.NotFound();

            return this.Ok(projectServiceSummary);
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/pipeline")]
        public async Task<IActionResult> GetProjectServicePipelineById(Guid organizationId, Guid projectId, Guid serviceId)
        {
            var projectServicePipeline = await _projectServiceQueryService.GetProjectServicePipelineById(organizationId, projectId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (projectServicePipeline == null)
                return this.NotFound();

            return this.Ok(projectServicePipeline);
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/features")]
        public async Task<IActionResult> GetProjectServiceFeaturesById(Guid organizationId, Guid projectId, Guid serviceId)
        {
            var projectServiceFeatures = await _projectServiceQueryService.GetProjectServiceFeaturesById(organizationId, projectId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (projectServiceFeatures == null)
                return this.NotFound();

            return this.Ok(projectServiceFeatures);
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services")]
        public async Task<IActionResult> CreateProjectService(Guid organizationId, Guid projectId, [FromBody]ProjectServicePostRp projectServiceRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectServiceService.CreateProjectService(organizationId, projectId, projectServiceRp);

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

            return this.Ok(new { ServiceId = await _domainManagerService.GetResult<Guid>("ServiceId") });
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/imports")]
        public async Task<IActionResult> ImportProjectService(Guid organizationId, Guid projectId, [FromBody]ProjectServiceImportPostRp projectServiceRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectServiceService.ImportProjectService(organizationId, projectId, projectServiceRp);

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

            return this.Ok(new { ServiceId = await _domainManagerService.GetResult<Guid>("ServiceId") });
        }


        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/builds")]
        public async Task<IActionResult> CreateBuildProjectService(Guid organizationId, Guid projectId, Guid serviceId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectServiceService.CreateBuildProjectService(organizationId, projectId, serviceId);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/releases")]
        public async Task<IActionResult> CreateReleaseProjectService(Guid organizationId, Guid projectId, Guid serviceId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectServiceService.CreateReleaseProjectService(organizationId, projectId, serviceId);

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

        [HttpPut]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/releases/approvals/{approvalId:int}")]
        public async Task<IActionResult> CompleteApprovalProjectService(Guid organizationId, Guid projectId, Guid serviceId, int approvalId, [FromBody]ProjectServiceApprovalPutRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            if(!(resource.Status.Equals("approved", StringComparison.InvariantCultureIgnoreCase) ||
                 resource.Status.Equals("rejected", StringComparison.InvariantCultureIgnoreCase)))
            {
                ModelState.AddModelError("", "The status shouble be approved or rejected");
                return BadRequest(ModelState);
            }

            await _projectServiceService.CompleteApprovalProjectService(organizationId, projectId, serviceId, approvalId, resource);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}")]
        public async Task<IActionResult> DeleteProjectService(Guid organizationId, Guid projectId, Guid serviceId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectServiceService.DeleteProjectService(organizationId, projectId, serviceId);

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
