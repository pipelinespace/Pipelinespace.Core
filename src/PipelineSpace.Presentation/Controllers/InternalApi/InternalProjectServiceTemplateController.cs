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
    public class InternalProjectServiceTemplateController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IInternalProjectServiceTemplateService _internalProjectServiceTemplateService;
        
        public InternalProjectServiceTemplateController(IDomainManagerService domainManagerService,
                                                        IInternalProjectServiceTemplateService internalProjectServiceTemplateService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _internalProjectServiceTemplateService = internalProjectServiceTemplateService;
        }

        [HttpPatch]
        [Route("{organizationId:guid}/servicetemplates/{serviceTemplateId:guid}/activate")]
        public async Task<IActionResult> ActivateTemplate(Guid organizationId, Guid serviceTemplateId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectServiceTemplateService.ActivateProjectServiceTemplate(organizationId, serviceTemplateId);

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
