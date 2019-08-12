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
    [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
    [Route("{api}/organizations")]
    public class ProjectUserInvitationController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectUserInvitationService _projectUserInvitationService;
        readonly IProjectUserInvitationQueryService _projectUserInvitationQueryService;

        public ProjectUserInvitationController(IDomainManagerService domainManagerService,
                                               IProjectUserInvitationService projectUserInvitationService,
                                               IProjectUserInvitationQueryService projectUserInvitationQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectUserInvitationService = projectUserInvitationService;
            _projectUserInvitationQueryService = projectUserInvitationQueryService;
        }

        
        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/userinvitations")]
        public async Task<IActionResult> GetInvitations(Guid organizationId, Guid projectId)
        {
            var invitations = await _projectUserInvitationQueryService.GetInvitations(organizationId, projectId);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/userinvitations")]
        public async Task<IActionResult> CreateUserInvitation(Guid organizationId, Guid projectId, [FromBody]ProjectUserInvitationPostRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            if (resource.Role != Domain.Models.PipelineRole.ProjectAdmin && resource.Role != Domain.Models.PipelineRole.ProjectContributor)
            {
                ModelState.AddModelError("", "The role is not valid, select a valid role");
                return this.BadRequest(ModelState);
            }

            await _projectUserInvitationService.InviteUser(organizationId, projectId, resource);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/userinvitations/{invitationId:guid}/accept")]
        public async Task<IActionResult> AcceptUserInvitation(Guid organizationId, Guid projectId, Guid invitationId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectUserInvitationService.AcceptInvitation(organizationId, projectId, invitationId);

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
        [Route("{organizationId:guid}/projects/{projectId:guid}/userinvitations/{invitationId:guid}/cancel")]
        public async Task<IActionResult> CancelUserInvitation(Guid organizationId, Guid projectId, Guid invitationId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _projectUserInvitationService.CancelInvitation(organizationId, projectId, invitationId);

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
