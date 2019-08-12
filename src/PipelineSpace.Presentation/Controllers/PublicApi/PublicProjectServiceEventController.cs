using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.PublicServices.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.PublicApi
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("publicapi/organizations")]
    public class PublicProjectServiceEventController : Controller
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IPublicProjectServiceEventService _projectServiceEventService;

        public PublicProjectServiceEventController(IDomainManagerService domainManagerService, IPublicProjectServiceEventService projectServiceEventService)
        {
            _domainManagerService = domainManagerService;
            _projectServiceEventService = projectServiceEventService;
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/events")]
        public async Task<IActionResult> CreateProjectServiceEvent(Guid organizationId, Guid projectId, Guid serviceId, [FromBody]ProjectServiceEventPostRp projectServiceEventPostRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectServiceEventService.CreateProjectServiceEvent(organizationId, projectId, serviceId, projectServiceEventPostRp);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok();
        }
    }
}
