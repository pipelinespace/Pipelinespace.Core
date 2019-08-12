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
    public class InternalProjectActivityController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IInternalProjectActivityService _internalProjectActivityService;
        readonly IProjectServiceService _projectServiceService;

        public InternalProjectActivityController(IDomainManagerService domainManagerService,
                                                 IInternalProjectActivityService internalProjectActivityService,
                                                 IProjectServiceService projectServiceService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _internalProjectActivityService = internalProjectActivityService;
            _projectServiceService = projectServiceService;
        }

        [HttpPut]
        [Route("{organizationId:guid}/projects/{projectId:guid}/activities/{code}")]
        public async Task<IActionResult> UpdateActivity(Guid organizationId, Guid projectId, string code, [FromBody]ProjectActivityPutRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectActivityService.UpdateProjectActivity(organizationId, projectId, code, resource);

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
