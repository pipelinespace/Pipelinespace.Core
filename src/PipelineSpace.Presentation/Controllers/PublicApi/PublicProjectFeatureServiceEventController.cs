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
    public class PublicProjectFeatureServiceEventController : Controller
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IPublicProjectFeatureServiceEventService _projectFeatureServiceEventService;

        public PublicProjectFeatureServiceEventController(IDomainManagerService domainManagerService, IPublicProjectFeatureServiceEventService projectServiceEventService)
        {
            _domainManagerService = domainManagerService;
            _projectFeatureServiceEventService = projectServiceEventService;
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/events")]
        public async Task<IActionResult> CreateProjectServiceEvent(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, [FromBody]ProjectFeatureServiceEventPostRp projectFeatureServiceEventPostRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectFeatureServiceEventService.CreateProjectFeatureServiceEvent(organizationId, projectId, featureId, serviceId, projectFeatureServiceEventPostRp);

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
