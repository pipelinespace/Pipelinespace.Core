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
    public class InternalProjectController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IInternalProjectService _internalProjectService;

        public InternalProjectController(IDomainManagerService domainManagerService,
                                         IInternalProjectService internalProjectService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _internalProjectService = internalProjectService;
        }



        [HttpPatch]
        [Route("{organizationId:guid}/projects/{projectId:guid}")]
        public async Task<IActionResult> PatchProject(Guid organizationId, Guid projectId, [FromBody]ProjectPatchRp projectRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectService.PatchProject(organizationId, projectId, projectRp);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/activate")]
        public async Task<IActionResult> ActivateProject(Guid organizationId, Guid projectId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectService.ActivateProject(organizationId, projectId);

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
