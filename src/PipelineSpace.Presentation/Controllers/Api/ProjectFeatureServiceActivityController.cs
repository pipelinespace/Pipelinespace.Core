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
    public class ProjectFeatureServiceActivityController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectFeatureServiceActivityQueryService _projectFeatureServiceActivityQueryService;

        public ProjectFeatureServiceActivityController(IDomainManagerService domainManagerService,
                                                       IProjectFeatureServiceActivityQueryService projectFeatureServiceActivityQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectFeatureServiceActivityQueryService = projectFeatureServiceActivityQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/activities")]
        public async Task<IActionResult> GetProjectFeatureServiceActivities(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            var projectFeatureServiceActivities = await _projectFeatureServiceActivityQueryService.GetProjectFeatureServiceActivities(organizationId, projectId, featureId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projectFeatureServiceActivities);
        }
    }
}
