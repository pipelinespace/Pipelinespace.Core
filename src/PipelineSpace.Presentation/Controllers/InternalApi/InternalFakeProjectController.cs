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
    public class InternalFakeProjectController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IInternalProjectService _internalProjectService;

        public InternalFakeProjectController(IDomainManagerService domainManagerService,
                                             IInternalProjectService internalProjectService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _internalProjectService = internalProjectService;
        }

        [HttpPost]
        [Route("{organizationId:guid}/fake/vsts/projects/{projectId:guid}")]
        public async Task<IActionResult> PatchProject(Guid organizationId, Guid projectId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectService.CreateVSTSFakeProject(organizationId, projectId);

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
