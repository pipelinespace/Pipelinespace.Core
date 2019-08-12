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
    public class ProjectFeatureServiceDeliveryController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectFeatureServiceDeliveryQueryService _projectFeatureServiceDeliveryQueryService;

        public ProjectFeatureServiceDeliveryController(IDomainManagerService domainManagerService,
                                                       IProjectFeatureServiceDeliveryQueryService projectFeatureServiceDeliveryQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectFeatureServiceDeliveryQueryService = projectFeatureServiceDeliveryQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/deliveries")]
        public async Task<IActionResult> GetFeatureProjectServiceDeliveries(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            var projectFeatureServiceDeliveries = await _projectFeatureServiceDeliveryQueryService.GetProjectFeatureServiceDeliveries(organizationId, projectId, featureId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projectFeatureServiceDeliveries);
        }
    }
}
