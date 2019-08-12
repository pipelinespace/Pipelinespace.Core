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
    public class ProjectServiceDeliveryController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectServiceDeliveryQueryService _projectServiceDeliveryQueryService;

        public ProjectServiceDeliveryController(IDomainManagerService domainManagerService,
                                                IProjectServiceDeliveryQueryService projectServiceDeliveryQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectServiceDeliveryQueryService = projectServiceDeliveryQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/deliveries")]
        public async Task<IActionResult> GetProjectServiceDeliveries(Guid organizationId, Guid projectId, Guid serviceId)
        {
            var projectServiceDeliveries = await _projectServiceDeliveryQueryService.GetProjectServiceDeliveries(organizationId, projectId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(projectServiceDeliveries);
        }
    }
}
