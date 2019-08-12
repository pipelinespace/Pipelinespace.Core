using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class OrganizationQueryService : IOrganizationQueryService
    {
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;

        public OrganizationQueryService(IIdentityService identityService,
                                        IOrganizationRepository organizationRepository,
                                        IUserRepository userRepository)
        {
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
        }

        public async Task<OrganizationListRp> GetOrganizations()
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            OrganizationListRp list = new OrganizationListRp();

            if(user.Organizations != null)
            {
                var organizations = user.FindOrganizations();

                list.Items = organizations.Select(x => new OrganizationListItemRp()
                {
                    OrganizationId = x.OrganizationId,
                    Name = x.Name,
                    Description = x.Description,
                    Status = x.Status
                }).ToList();
            }

            return list;
        }

        public async Task<OrganizationGetRp> GetOrganizationById(Guid organizationId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);

            OrganizationGetRp organizationRp = null;
            if(organization != null)
            {
                organizationRp = new OrganizationGetRp() {
                    OrganizationId = organization.OrganizationId,
                    Name = organization.Name,
                    Description = organization.Description,
                    WebSiteUrl = organization.WebSiteUrl,
                    Status = organization.Status
                };
            }

            return organizationRp;
        }
    }
}
