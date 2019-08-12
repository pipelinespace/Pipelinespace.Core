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
    public class OrganizationUserService : IOrganizationUserService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;
        
        public OrganizationUserService(IDomainManagerService domainManagerService, 
                                       IIdentityService identityService, 
                                       IOrganizationRepository organizationRepository, 
                                       IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
        }

        public async Task RemoveUser(Guid organizationId, string userId)
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
                await _domainManagerService.AddForbidden($"You are not authorized to remove users in this organization.");
                return;
            }

            OrganizationUser organizationUser = organization.GetOrganizationUserById(userId);
            if (organizationUser == null)
            {
                await _domainManagerService.AddNotFound($"The user with id {userId} does not exists in the organization.");
                return;
            }

            if(loggedUserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase))
            {
                await _domainManagerService.AddConflict($"You cannot delete yourself from the organization.");
                return;
            }

            organizationUser.Delete(loggedUserId);
            
            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }

    }
}
