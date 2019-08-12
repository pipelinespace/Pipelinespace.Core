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
    public class OrganizationUserInvitationController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IOrganizationUserInvitationService _organizationUserInvitationService;
        readonly IOrganizationUserInvitationQueryService _organizationUserInvitationQueryService;

        public OrganizationUserInvitationController(IDomainManagerService domainManagerService,
                                                    IOrganizationUserInvitationService organizationUserInvitationService,
                                                    IOrganizationUserInvitationQueryService organizationUserInvitationQueryService) : base (domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _organizationUserInvitationService = organizationUserInvitationService;
            _organizationUserInvitationQueryService = organizationUserInvitationQueryService;
        }

      
        [HttpGet]
        [Route("{organizationId:guid}/userinvitations")]
        public async Task<IActionResult> GetInvitations(Guid organizationId)
        {
            var invitations = await _organizationUserInvitationQueryService.GetInvitations(organizationId);

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
        
        [HttpPost]
        [Route("{organizationId:guid}/userinvitations")]
        public async Task<IActionResult> CreateUserInvitation(Guid organizationId, [FromBody]OrganizationUserInvitationPostRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            if(resource.Role != Domain.Models.PipelineRole.OrganizationAdmin && resource.Role != Domain.Models.PipelineRole.OrganizationContributor)
            {
                ModelState.AddModelError("", "The role is not valid, select a valid role");
                return this.BadRequest(ModelState);
            }

            await _organizationUserInvitationService.InviteUser(organizationId, resource);

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

        [HttpPost]
        [Route("{organizationId:guid}/userinvitations/{invitationId:guid}/accept")]
        public async Task<IActionResult> AcceptUserInvitation(Guid organizationId, Guid invitationId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _organizationUserInvitationService.AcceptInvitation(organizationId, invitationId);

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

        [HttpPost]
        [Route("{organizationId:guid}/userinvitations/{invitationId:guid}/cancel")]
        public async Task<IActionResult> CancelUserInvitation(Guid organizationId, Guid invitationId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _organizationUserInvitationService.CancelInvitation(organizationId, invitationId);

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
