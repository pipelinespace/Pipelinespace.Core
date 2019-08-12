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
    public class ProjectUserService : IProjectUserService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;
        
        public ProjectUserService(IDomainManagerService domainManagerService, 
                                  IIdentityService identityService, 
                                  IOrganizationRepository organizationRepository, 
                                  IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
        }

        public async Task RemoveUser(Guid organizationId, Guid projectId, string userId)
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

            ProjectUser projectUser = project.GetProjectUserById(userId);
            if (projectUser == null)
            {
                await _domainManagerService.AddNotFound($"The user with id {userId} does not exists in the project.");
                return;
            }

            if (loggedUserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase))
            {
                await _domainManagerService.AddConflict($"You cannot delete yourself from the project.");
                return;
            }

            projectUser.Delete(loggedUserId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }

    }
}
