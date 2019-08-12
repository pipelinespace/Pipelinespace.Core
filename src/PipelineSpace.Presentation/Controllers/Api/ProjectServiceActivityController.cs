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
    public class ProjectServiceActivityController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectServiceActivityQueryService _projectServiceActivityQueryService;

        public ProjectServiceActivityController(IDomainManagerService domainManagerService,
                                                IProjectServiceActivityQueryService projectServiceActivityQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectServiceActivityQueryService = projectServiceActivityQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/activities")]
        public async Task<IActionResult> GetProjectServiceActivities(Guid organizationId, Guid projectId, Guid serviceId)
        {
            var projectServiceActivities = await _projectServiceActivityQueryService.GetProjectServiceActivities(organizationId, projectId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projectServiceActivities);
        }
    }
}
