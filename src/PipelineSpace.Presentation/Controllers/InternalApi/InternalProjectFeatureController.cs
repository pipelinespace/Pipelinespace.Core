using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Application.Services.InternalServices.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.InternalApi
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "internaladmin")]
    [Route("internalapi/organizations")]
    public class InternalProjectFeatureController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IInternalProjectFeatureService _internalProjectFeatureService;

        public InternalProjectFeatureController(IDomainManagerService domainManagerService,
                                                IInternalProjectFeatureService internalProjectFeatureService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _internalProjectFeatureService = internalProjectFeatureService;
        }
        
        [HttpPatch]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/activate")]
        public async Task<IActionResult> ActivateFeature(Guid organizationId, Guid projectId, Guid featureId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectFeatureService.ActivateProjectFeature(organizationId, projectId, featureId);

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

        [HttpPatch]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}")]
        public async Task<IActionResult> PatchFeatureService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, [FromBody]ProjectFeatureServicePatchtRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectFeatureService.PatchProjectFeatureService(organizationId, projectId, featureId, serviceId, resource);

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
