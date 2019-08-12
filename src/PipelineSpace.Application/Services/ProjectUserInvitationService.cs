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
    public class ProjectUserInvitationService : IProjectUserInvitationService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;
        readonly IProjectUserInvitationRepository _projectUserInvitationRepository;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;

        public ProjectUserInvitationService(IDomainManagerService domainManagerService, 
                                                 IIdentityService identityService, 
                                                 IOrganizationRepository organizationRepository, 
                                                 IUserRepository userRepository,
                                                 IProjectUserInvitationRepository projectUserInvitationRepository,
                                                 IEventBusService eventBusService,
                                                 IActivityMonitorService activityMonitorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _projectUserInvitationRepository = projectUserInvitationRepository;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
        }

        public async Task InviteUser(Guid organizationId, Guid projectId, ProjectUserInvitationPostRp resource)
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

            Project project = user.FindProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInProject(projectId);
            if (role != PipelineRole.ProjectAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to invite users in this project.");
                return;
            }

            User invitedUser = await _userRepository.GetUserByEmail(resource.UserEmail);

            if(invitedUser != null)
            {
                ProjectUser organizationUser = project.GetProjectUser(resource.UserEmail);
                if (organizationUser != null)
                {
                    await _domainManagerService.AddConflict($"The user with email {resource.UserEmail} already exists.");
                    return;
                }
            }

            ProjectUserInvitation projectUserInvitation = project.GetProjectUserInvitation(resource.UserEmail);
            if (projectUserInvitation != null)
            {
                await _domainManagerService.AddConflict($"The user with email {resource.UserEmail} was already invited.");
                return;
            }

            ProjectUserInvitation newUserInvitation = ProjectUserInvitation.Factory.Create(projectId, invitedUser == null ? null : invitedUser.Id, resource.UserEmail, resource.Role, loggedUserId);

            project.AddUserInvitation(newUserInvitation);

            await _userRepository.SaveChanges();

            var @event = new ProjectUserInvitedEvent(_correlationId) {
                ProjectUserInvitationId = newUserInvitation.ProjectUserInvitationId,
                InvitationType = newUserInvitation.InvitationType,
                RequestorFullName = loggedUserName,
                Role = newUserInvitation.Role,
                UserEmail = newUserInvitation.UserEmail,
                UserFullName = invitedUser == null ? null : invitedUser.GetFullName()
            };

            await _eventBusService.Publish(queueName: "ProjectUserInvitedEvent", @event: @event);

        }

        public async Task CancelInvitation(Guid organizationId, Guid projectId, Guid invitationId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            Project project = user.FindProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInProject(projectId);
            if (role != PipelineRole.ProjectAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to invite users in this project.");
                return;
            }

            ProjectUserInvitation projectUserInvitation = project.GetUserInvitation(invitationId);

            if (projectUserInvitation == null)
            {
                await _domainManagerService.AddNotFound($"The invitation with id {invitationId} does not exists.");
                return;
            }

            if (projectUserInvitation.InvitationStatus != UserInvitationStatus.Pending)
            {
                await _domainManagerService.AddConflict($"The invitation with id {invitationId} cannot be cancel. Only invitations in status pending can be canceled.");
                return;
            }

            projectUserInvitation.Cancel();

            await _userRepository.SaveChanges();
        }

        public async Task AcceptInvitation(Guid organizationId, Guid projectId, Guid invitationId)
        {
            string loggedUserId = _identityService.GetUserId();
            string loggedUserEmail = _identityService.GetUserEmail();

            User user = await _userRepository.GetUser(loggedUserId);

            ProjectUserInvitation projectUserInvitation = await _projectUserInvitationRepository.GetProjectUserInvitationById(invitationId);

            if (projectUserInvitation == null)
            {
                await _domainManagerService.AddNotFound($"The invitation with id {invitationId} does not exists.");
                return;
            }

            if (!loggedUserEmail.Equals(projectUserInvitation.UserEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                await _domainManagerService.AddForbidden($"You are not authorized to accept this invitation.");
                return;
            }

            if (projectUserInvitation.InvitationStatus != UserInvitationStatus.Pending)
            {
                await _domainManagerService.AddConflict($"The invitation with id {invitationId} cannot be accepted. Only invitations in status pending can be accepted.");
                return;
            }

            projectUserInvitation.Accept();

            //Grant User Access
            projectUserInvitation.Project.GrantUserAccess(loggedUserId, projectUserInvitation.Role);

            await _projectUserInvitationRepository.SaveChanges();
        }

    }
}
