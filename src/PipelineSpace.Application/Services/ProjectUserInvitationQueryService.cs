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
    public class ProjectUserInvitationQueryService : IProjectUserInvitationQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;
        readonly IProjectUserInvitationRepository _projectUserInvitationRepository;

        public ProjectUserInvitationQueryService(IDomainManagerService domainManagerService, 
                                                 IIdentityService identityService, 
                                                 IOrganizationRepository organizationRepository, 
                                                 IUserRepository userRepository,
                                                 IProjectUserInvitationRepository projectUserInvitationRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _projectUserInvitationRepository = projectUserInvitationRepository;
        }

        public async Task<ProjectUserInvitationListRp> GetUserInvitations()
        {
            string loggedUserEmail = _identityService.GetUserEmail();

            var invitations = await _projectUserInvitationRepository.GetProjectUserInvitationByEmail(loggedUserEmail);

            ProjectUserInvitationListRp list = new ProjectUserInvitationListRp();
            list.Items = invitations.Select(x => new ProjectUserInvitationListItemRp()
            {
                InvitationId = x.ProjectUserInvitationId,
                OrganizationId = x.Project.OrganizationId,
                OrganizationName = x.Project.Organization.Name,
                ProjectId = x.ProjectId,
                ProjectName = x.Project.Name,
                UserEmail = x.UserEmail,
                Role = x.Role,
                InvitationStatus = x.InvitationStatus,
                AcceptedDate = x.AcceptedDate,
                CreationDate = x.CreationDate
            }).OrderByDescending(x => x.CreationDate).ToList();

            return list;
        }

        public async Task<ProjectUserInvitationListRp> GetInvitations(Guid organizationId, Guid projectId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            Project project = user.FindProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return null;
            }

            //PipelineRole role = user.GetRoleInProject(projectId);
            //if (role != PipelineRole.ProjectAdmin)
            //{
            //    await _domainManagerService.AddForbidden($"You are not authorized to invite users in this project.");
            //    return null;
            //}

            ProjectUserInvitationListRp list = new ProjectUserInvitationListRp();
            list.Items = project.UserInvitations.Select(x => new ProjectUserInvitationListItemRp()
            {
                InvitationId = x.ProjectUserInvitationId,
                UserEmail = x.UserEmail,
                Role = x.Role,
                InvitationStatus = x.InvitationStatus,
                AcceptedDate = x.AcceptedDate,
                CreationDate = x.CreationDate
            }).ToList();

            return list;

        }

        public async Task<ProjectUserInvitationGetRp> GetInvitationById(Guid invitationId)
        {
            var entity = await _projectUserInvitationRepository.GetProjectUserInvitationById(invitationId);

            ProjectUserInvitationGetRp invitation = null;
            if (entity != null)
            {
                invitation = new ProjectUserInvitationGetRp()
                {
                    InvitationId = entity.ProjectUserInvitationId,
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
