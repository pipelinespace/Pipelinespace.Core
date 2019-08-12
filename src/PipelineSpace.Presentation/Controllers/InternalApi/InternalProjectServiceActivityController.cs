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
    public class InternalProjectServiceActivityController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IInternalProjectServiceActivityService _internalProjectServiceActivityService;
        readonly IProjectServiceService _projectServiceService;

        public InternalProjectServiceActivityController(IDomainManagerService domainManagerService,
                                                        IInternalProjectServiceActivityService internalProjectServiceActivityService,
                                                        IProjectServiceService projectServiceService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _internalProjectServiceActivityService = internalProjectServiceActivityService;
            _projectServiceService = projectServiceService;
        }

        [HttpPut]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/activities/{code}")]
        public async Task<IActionResult> UpdateServiceActivity(Guid organizationId, Guid projectId, Guid serviceId, string code, [FromBody]ProjectServiceActivityPutRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectServiceActivityService.UpdateProjectServiceActivity(organizationId, projectId, serviceId, code, resource);

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
