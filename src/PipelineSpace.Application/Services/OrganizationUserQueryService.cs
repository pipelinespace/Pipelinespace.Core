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
    public class OrganizationUserQueryService : IOrganizationUserQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;
        
        public OrganizationUserQueryService(IDomainManagerService domainManagerService, 
                                            IIdentityService identityService, 
                                            IOrganizationRepository organizationRepository, 
                                            IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
        }

        public async Task<OrganizationUserListRp> GetUsers(Guid organizationId)
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
            //    await _domainManagerService.AddForbidden($"You are not authorized to retreive the users in this organization.");
            //    return null;
            //}

            OrganizationUserListRp list = new OrganizationUserListRp();
            list.Items = organization.Users.Select(x => new OrganizationUserListItemRp()
            {
                UserId = x.UserId,
                UserName = x.User.GetFullName(),
                UserEmail = x.User.Email,
                Role = x.Role,
                AddedDate = x.CreationDate
            }).OrderBy(x=> x.AddedDate).ToList();

            return list;
        }

    }
}
