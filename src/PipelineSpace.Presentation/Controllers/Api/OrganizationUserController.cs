using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Authorize(Roles = "globaladmin,organizationadmin")]
    [Route("{api}/organizations")]
    public class OrganizationInvitationController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;

        readonly IOrganizationUserService _organizationUserService;
        readonly IOrganizationUserQueryService _organizationUserQueryService;

        public OrganizationInvitationController(IDomainManagerService domainManagerService,
                                                IOrganizationUserService organizationUserService,
                                                IOrganizationUserQueryService organizationUserQueryService) : base (domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _organizationUserService = organizationUserService;
            _organizationUserQueryService = organizationUserQueryService;
        }
        
        [HttpGet]
        [Route("{organizationId:guid}/users")]
        public async Task<IActionResult> GetUsers(Guid organizationId)
        {
            var invitations = await _organizationUserQueryService.GetUsers(organizationId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasForbidden())
            {
                return this.Forbidden(_domainManagerService.GetForbidden());
            }

            return this.Ok(invitations);
        }


        [HttpDelete]
        [Route("{organizationId:guid}/users/{userId}")]
        public async Task<IActionResult> RemoveUser(Guid organizationId, string userId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _organizationUserService.RemoveUser(organizationId, userId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }
            
            if (_domainManagerService.HasForbidden())
            {
                return this.Forbidden(_domainManagerService.GetForbidden());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok();
        }
    }
}
