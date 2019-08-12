using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
    [Route("{api}/me")]
    public class MeController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IOrganizationUserInvitationQueryService _organizationUserInvitationQueryService;
        readonly IProjectUserInvitationQueryService _projectUserInvitationQueryService;

        public MeController(IDomainManagerService domainManagerService,
                            IOrganizationUserInvitationQueryService organizationUserInvitationQueryService,
                            IProjectUserInvitationQueryService projectUserInvitationQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _organizationUserInvitationQueryService = organizationUserInvitationQueryService;
            _projectUserInvitationQueryService = projectUserInvitationQueryService;
        }

        [HttpGet]
        [Route("organizationinvitations")]
        public async Task<IActionResult> GetOrganizationInvitations()
        {
            var invitations = await _organizationUserInvitationQueryService.GetUserInvitations();

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

        [HttpGet]
        [Route("projectinvitations")]
        public async Task<IActionResult> GetProjectInvitations()
        {
            var invitations = await _projectUserInvitationQueryService.GetUserInvitations();

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
    }
}
