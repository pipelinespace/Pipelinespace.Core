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
    public class InternalProjectFeatureServiceActivityController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IInternalProjectFeatureServiceActivityService _internalProjectFeatureServiceActivityService;
        readonly IProjectServiceService _projectServiceService;

        public InternalProjectFeatureServiceActivityController(IDomainManagerService domainManagerService,
                                                               IInternalProjectFeatureServiceActivityService internalProjectFeatureServiceActivityService,
                                                               IProjectServiceService projectServiceService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _internalProjectFeatureServiceActivityService = internalProjectFeatureServiceActivityService;
            _projectServiceService = projectServiceService;
        }

        [HttpPut]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/activities/{code}")]
        public async Task<IActionResult> UpdateServiceActivity(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, string code, [FromBody]ProjectFeatureServiceActivityPutRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectFeatureServiceActivityService.UpdateProjectFeatureServiceActivity(organizationId, projectId, featureId, serviceId, code, resource);

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
