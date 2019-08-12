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
    public class ProjectUserQueryService : IProjectUserQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;
        
        public ProjectUserQueryService(IDomainManagerService domainManagerService, 
                                                 IIdentityService identityService, 
                                                 IOrganizationRepository organizationRepository, 
                                                 IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
        }

        public async Task<ProjectUserListRp> GetUsers(Guid organizationId, Guid projectId)
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

            ProjectUserListRp list = new ProjectUserListRp();
            list.Items = project.Users.Select(x => new ProjectUserListItemRp()
            {
                UserId = x.UserId,
                UserName = x.User.GetFullName(),
                UserEmail = x.User.Email,
                Role = x.Role,
                AddedDate = x.CreationDate
            }).OrderBy(x => x.AddedDate).ToList();

            return list;
        }

    }
}
