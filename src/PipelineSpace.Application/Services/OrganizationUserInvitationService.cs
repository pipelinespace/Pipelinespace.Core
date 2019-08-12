using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Worker.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class OrganizationUserInvitationService : IOrganizationUserInvitationService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;
        readonly IOrganizationUserInvitationRepository _organizationUserInvitationRepository;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;

        public OrganizationUserInvitationService(IDomainManagerService domainManagerService, 
                                                 IIdentityService identityService, 
                                                 IOrganizationRepository organizationRepository, 
                                                 IUserRepository userRepository,
                                                 IOrganizationUserInvitationRepository organizationUserInvitationRepository,
                                                 IEventBusService eventBusService,
                                                 IActivityMonitorService activityMonitorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _organizationUserInvitationRepository = organizationUserInvitationRepository;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
        }

        public async Task InviteUser(Guid organizationId, OrganizationUserInvitationPostRp resource)
        {
            string loggedUserId = _identityService.GetUserId();
            string loggedUserName = _identityService.GetUserName();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInOrganization(organizationId);
            if (role != PipelineRole.OrganizationAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to invite users in this organization.");
                return;
            }

            User invitedUser = await _userRepository.GetUserByEmail(resource.UserEmail);

            if(invitedUser != null)
            {
                OrganizationUser organizationUser = organization.GetOrganizationUser(resource.UserEmail);
                if (organizationUser != null)
                {
                    await _domainManagerService.AddConflict($"The user with email {resource.UserEmail} already exists.");
                    return;
                }
            }

            OrganizationUserInvitation organizationUserInvitation = organization.GetOrganizationUserInvitation(resource.UserEmail);
            if (organizationUserInvitation != null)
            {
                await _domainManagerService.AddConflict($"The user with email {resource.UserEmail} was already invited.");
                return;
            }

            OrganizationUserInvitation newUserInvitation = OrganizationUserInvitation.Factory.Create(organizationId, invitedUser == null ? null : invitedUser.Id, resource.UserEmail, resource.Role, loggedUserId);

            organization.AddUserInvitation(newUserInvitation);

            await _userRepository.SaveChanges();

            var @event = new OrganizationUserInvitedEvent(_correlationId)
            {
                OrganizationUserInvitationId = newUserInvitation.OrganizationUserInvitationId,
                InvitationType = newUserInvitation.InvitationType,
                RequestorFullName = loggedUserName,
                Role = newUserInvitation.Role,
                UserEmail = newUserInvitation.UserEmail,
                UserFullName = invitedUser == null ? null : invitedUser.GetFullName()
            };

            await _eventBusService.Publish(queueName: "OrganizationUserInvitedEvent", @event: @event);

        }

        public async Task CancelInvitation(Guid organizationId, Guid invitationId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInOrganization(organizationId);
            if (role != PipelineRole.OrganizationAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to cancel user invitations in this organization.");
                return;
            }

            OrganizationUserInvitation organizationUserInvitation = organization.GetUserInvitation(invitationId);

            if (organizationUserInvitation == null)
            {
                await _domainManagerService.AddNotFound($"The invitation with id {invitationId} does not exists.");
                return;
            }

            if (organizationUserInvitation.InvitationStatus != UserInvitationStatus.Pending)
            {
                await _domainManagerService.AddConflict($"The invitation with id {invitationId} cannot be cancel. Only invitations in status pending can be canceled.");
                return;
            }

            organizationUserInvitation.Cancel();

            await _userRepository.SaveChanges();
        }

        public async Task AcceptInvitation(Guid organizationId, Guid invitationId)
        {
            string loggedUserId = _identityService.GetUserId();
            string loggedUserEmail = _identityService.GetUserEmail();

            User user = await _userRepository.GetUser(loggedUserId);
           
            OrganizationUserInvitation organizationUserInvitation = await _organizationUserInvitationRepository.GetOrganizationUserInvitationById(invitationId);

            if (organizationUserInvitation == null)
            {
                await _domainManagerService.AddNotFound($"The invitation with id {invitationId} does not exists.");
                return;
            }

            if (!loggedUserEmail.Equals(organizationUserInvitation.UserEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                await _domainManagerService.AddForbidden($"You are not authorized to accept this invitation.");
                return;
            }

            if (organizationUserInvitation.InvitationStatus != UserInvitationStatus.Pending)
            {
                await _domainManagerService.AddConflict($"The invitation with id {invitationId} cannot be accepted. Only invitations in status pending can be accepted.");
                return;
            }

            organizationUserInvitation.Accept();

            //Grant User Access
            organizationUserInvitation.Organization.GrantUserAccess(loggedUserId, organizationUserInvitation.Role);
            
            await _organizationUserInvitationRepository.SaveChanges();
        }

    }
}
