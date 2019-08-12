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
    public class ProjectServiceEventController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectServiceEventService _pojectServiceEventService;
        readonly IProjectServiceEventQueryService _projectServiceEventQueryService;

        public ProjectServiceEventController(IDomainManagerService domainManagerService,
                                             IProjectServiceEventService pojectServiceEventService,
                                             IProjectServiceEventQueryService projectServiceEventQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _pojectServiceEventService = pojectServiceEventService;
            _projectServiceEventQueryService = projectServiceEventQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/events")]
        public async Task<IActionResult> GetProjectServiceEvents(Guid organizationId, Guid projectId, Guid serviceId, [FromQuery(Name = "baseEventType")]BaseEventType baseEventType)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            var events = await _projectServiceEventQueryService.GetProjectServiceEvents(organizationId, projectId, serviceId, baseEventType);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(events);
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/events")]
        public async Task<IActionResult> CreateProjectServiceEvent(Guid organizationId, Guid projectId, Guid serviceId, [FromBody]ProjectServiceEventPostRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _pojectServiceEventService.CreateProjectServiceEvent(organizationId, projectId, serviceId, resource);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok();
        }
    }
}
