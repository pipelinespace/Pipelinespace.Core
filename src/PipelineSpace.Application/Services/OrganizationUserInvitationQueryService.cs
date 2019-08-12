using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class OrganizationUserInvitationQueryService : IOrganizationUserInvitationQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;
        readonly IOrganizationUserInvitationRepository _organizationUserInvitationRepository;

        public OrganizationUserInvitationQueryService(IDomainManagerService domainManagerService, 
                                                      IIdentityService identityService, 
                                                      IOrganizationRepository organizationRepository, 
                                                      IUserRepository userRepository,
                                                      IOrganizationUserInvitationRepository organizationUserInvitationRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _organizationUserInvitationRepository = organizationUserInvitationRepository;
        }

        public async Task<OrganizationUserInvitationListRp> GetUserInvitations()
        {
            string loggedUserEmail = _identityService.GetUserEmail();

            var invitations = await _organizationUserInvitationRepository.GetOrganizationUserInvitationByEmail(loggedUserEmail);

            OrganizationUserInvitationListRp list = new OrganizationUserInvitationListRp();
            list.Items = invitations.Select(x => new OrganizationUserInvitationListItemRp()
            {
                InvitationId = x.OrganizationUserInvitationId,
                OrganizationId = x.OrganizationId,
                OrganizationName = x.Organization.Name,
                UserEmail = x.UserEmail,
                Role = x.Role,
                InvitationStatus = x.InvitationStatus,
                AcceptedDate = x.AcceptedDate,
                CreationDate = x.CreationDate
            }).ToList();

            return list;
        }

        public async Task<OrganizationUserInvitationListRp> GetInvitations(Guid organizationId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            //PipelineRole role = user.GetRoleInOrganization(organizationId);
            //if (role != PipelineRole.OrganizationAdmin)
            //{
            //    await _domainManagerService.AddForbidden($"You are not authorized to retreive the invitations in this organization.");
            //    return null;
            //}

            OrganizationUserInvitationListRp list = new OrganizationUserInvitationListRp();
            list.Items = organization.UserInvitations.Select(x => new OrganizationUserInvitationListItemRp()
            {
                InvitationId = x.OrganizationUserInvitationId,
                UserEmail = x.UserEmail,
                Role = x.Role,
                InvitationStatus = x.InvitationStatus,
                AcceptedDate = x.AcceptedDate,
                CreationDate = x.CreationDate
            }).OrderByDescending(x=> x.CreationDate).ToList();

            return list;
        }

        public async Task<OrganizationUserInvitationGetRp> GetInvitationById(Guid invitationId)
        {
            var entity = await _organizationUserInvitationRepository.GetOrganizationUserInvitationById(invitationId);

            OrganizationUserInvitationGetRp invitation = null;
            if (entity != null)
            {
                invitation = new OrganizationUserInvitationGetRp()
                {
                    InvitationId = entity.OrganizationUserInvitationId,
                    UserEmail = entity.UserEmail,
                    Role = entity.Role,
                    InvitationStatus = entity.InvitationStatus,
                    AcceptedDate = entity.AcceptedDate,
                    CreationDate = entity.CreationDate
                };
            }

            return invitation;
        }

    }
}
