using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
    [Route("{api}/organizations")]
    public class ProjectFeatureServiceEventController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectFeatureServiceEventService _projectFeatureServiceEventService;
        readonly IProjectFeatureServiceEventQueryService _projectFeatureServiceEventQueryService;

        public ProjectFeatureServiceEventController(IDomainManagerService domainManagerService,
                                                    IProjectFeatureServiceEventService projectFeatureServiceEventService,
                                                    IProjectFeatureServiceEventQueryService projectFeatureServiceEventQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectFeatureServiceEventService = projectFeatureServiceEventService;
            _projectFeatureServiceEventQueryService = projectFeatureServiceEventQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/events")]
        public async Task<IActionResult> GetProjectFeatureServiceEvents(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, [FromQuery(Name = "baseEventType")]BaseEventType baseEventType)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            var events = await _projectFeatureServiceEventQueryService.GetProjectFeatureServiceEvents(organizationId, projectId, featureId, serviceId, baseEventType);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(events);
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/events")]
        public async Task<IActionResult> CreateProjectFeatureServiceEvents(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, [FromBody] ProjectFeatureServiceEventPostRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectFeatureServiceEventService.CreateProjectFeatureServiceEvent(organizationId, projectId, featureId, serviceId, resource);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok();
        }
    }
}
