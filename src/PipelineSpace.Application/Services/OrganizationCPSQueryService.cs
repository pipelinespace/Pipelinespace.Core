using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain;
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
    public class OrganizationCPSQueryService : IOrganizationCPSQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationCPSRepository _organizationCPSRepository;
        readonly IUserRepository _userRepository;
        readonly IDataProtectorService _dataProtectorService;

        public OrganizationCPSQueryService(IDomainManagerService domainManagerService,
                                           IIdentityService identityService, 
                                           IOrganizationCPSRepository organizationCPSRepository,
                                           IUserRepository userRepository,
                                           IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationCPSRepository = organizationCPSRepository;
            _userRepository = userRepository;
            _dataProtectorService = dataProtectorService;
        }

        public async Task<OrganizationCPSListRp> GetOrganizationCloudProviderServices(Guid organizationId)
        {
            var loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            PipelineRole role = user.GetRoleInOrganization(organizationId);

            OrganizationCPSListRp list = new OrganizationCPSListRp();

            List<OrganizationCPS> cloudProviderServices = organization.GetCloudProviderServices();
            if (cloudProviderServices != null)
            {
                list.Items = cloudProviderServices.Select(x => new OrganizationCPSListItemRp()
                {
                    OrganizationCPSId = x.OrganizationCPSId,
                    Name = x.Name,
                    Type = x.Type,
                    AccessId = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccessId) : DomainConstants.Obfuscator.Default,
                    AccessName = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccessName) : DomainConstants.Obfuscator.Default,
                    AccessSecret = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccessSecret) : DomainConstants.Obfuscator.Default,
                    AccessRegion = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccessRegion) : DomainConstants.Obfuscator.Default,
                    AccessAppId = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccessAppId) : DomainConstants.Obfuscator.Default,
                    AccessAppSecret = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccessAppSecret) : DomainConstants.Obfuscator.Default,
                    AccessDirectory = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccessDirectory) : DomainConstants.Obfuscator.Default
                }).ToList();
            }

            return list;
        }

        public async Task<OrganizationCPSGetRp> GetOrganizationCloudProviderServiceById(Guid organizationId, Guid organizationCPSId)
        {
            var loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            OrganizationCPS cloudProvider = organization.GetCloudProviderServiceById(organizationCPSId);
            if (cloudProvider == null)
            {
                return null;
            }

            PipelineRole role = user.GetRoleInOrganization(organizationId);

            OrganizationCPSGetRp organizationCPSRp = new OrganizationCPSGetRp()
            {
                OrganizationCPSId = cloudProvider.OrganizationCPSId,
                Name = cloudProvider.Name,
                Type = cloudProvider.Type,
                AccessId = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(cloudProvider.AccessId) : DomainConstants.Obfuscator.Default,
                AccessName = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(cloudProvider.AccessName) : DomainConstants.Obfuscator.Default,
                AccessSecret = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(cloudProvider.AccessSecret) : DomainConstants.Obfuscator.Default,
                AccessRegion = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(cloudProvider.AccessRegion) : DomainConstants.Obfuscator.Default,
                AccessAppId = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(cloudProvider.AccessAppId) : DomainConstants.Obfuscator.Default,
                AccessAppSecret = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(cloudProvider.AccessAppSecret) : DomainConstants.Obfuscator.Default,
                AccessDirectory = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(cloudProvider.AccessDirectory) : DomainConstants.Obfuscator.Default
            };

            return organizationCPSRp;
        }


    }
}
