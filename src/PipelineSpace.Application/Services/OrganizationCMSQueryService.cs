using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class OrganizationCMSQueryService : IOrganizationCMSQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationCMSRepository _organizationCMSRepository;
        readonly IUserRepository _userRepository;
        readonly Func<ConfigurationManagementService, ICMSQueryService> _cmsQueryService;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;
        readonly IDataProtectorService _dataProtectorService;

        public OrganizationCMSQueryService(IDomainManagerService domainManagerService,
                                           IIdentityService identityService, 
                                           IOrganizationCMSRepository organizationCMSRepository,
                                           IUserRepository userRepository,
                                           Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService,
                                           Func<ConfigurationManagementService, ICMSQueryService> cmsQueryService,
                                           IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationCMSRepository = organizationCMSRepository;
            _userRepository = userRepository;
            _cmsQueryService = cmsQueryService;
            _cmsCredentialService = cmsCredentialService;
            _dataProtectorService = dataProtectorService;
        }

        public async Task<OrganizationCMSListRp> GetOrganizationConfigurationManagementServices(Guid organizationId, CMSConnectionType connectionType)
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

            OrganizationCMSListRp list = new OrganizationCMSListRp();

            List<OrganizationCMS> configurationManagementServices = organization.GetConfigurationManagementServices(connectionType);
            if (configurationManagementServices != null)
            {
                list.Items = configurationManagementServices.Select(x => new OrganizationCMSListItemRp()
                {
                    OrganizationCMSId = x.OrganizationCMSId,
                    Name = x.Name,
                    Type = x.Type,
                    AccountId = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccountId) : DomainConstants.Obfuscator.Default,
                    AccountName = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccountName) : DomainConstants.Obfuscator.Default,
                    AccessId = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccessId) : DomainConstants.Obfuscator.Default,
                    AccessSecret = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccessSecret) : DomainConstants.Obfuscator.Default,
                    AccessToken = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(x.AccessToken) : DomainConstants.Obfuscator.Default
                }).ToList();
            }

            return list;
        }

        public async Task<OrganizationCMSGetRp> GetOrganizationConfigurationManagementServiceById(Guid organizationId, Guid organizationCMSId)
        {
            var loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            OrganizationCMS configurationManagement = organization.GetConfigurationManagementServiceById(organizationCMSId);
            if (configurationManagement == null)
            {
                return null;
            }

            PipelineRole role = user.GetRoleInOrganization(organizationId);

            OrganizationCMSGetRp organizationCMSRp = new OrganizationCMSGetRp()
            {
                OrganizationCMSId = configurationManagement.OrganizationCMSId,
                Name = configurationManagement.Name,
                Type = configurationManagement.Type,
                AccountId = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(configurationManagement.AccountId) : DomainConstants.Obfuscator.Default,
                AccountName = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(configurationManagement.AccountName) : DomainConstants.Obfuscator.Default,
                AccessId = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(configurationManagement.AccessId) : DomainConstants.Obfuscator.Default,
                AccessSecret = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(configurationManagement.AccessSecret) : DomainConstants.Obfuscator.Default,
                AccessToken = role == PipelineRole.OrganizationAdmin ? _dataProtectorService.Unprotect(configurationManagement.AccessToken) : DomainConstants.Obfuscator.Default
            };

            return organizationCMSRp;
        }

        public async Task<OrganizationCMSAgentPoolListRp> GetOrganizationConfigurationManagementServiceAgentPools(Guid organizationId, Guid organizationCMSId)
        {
            var loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            OrganizationCMS configurationManagement = organization.GetConfigurationManagementServiceById(organizationCMSId);
            if (configurationManagement == null)
            {
                await _domainManagerService.AddNotFound($"The organzation configuration management service with id {organizationCMSId} does not exists.");
                return null;
            }

            var cmsAuthCredential = this._cmsCredentialService(configurationManagement.Type).GetToken(
                    accountId: _dataProtectorService.Unprotect(configurationManagement.AccountId), 
                    accountName: _dataProtectorService.Unprotect(configurationManagement.AccountName), 
                    accessSecret: _dataProtectorService.Unprotect(configurationManagement.AccessSecret), 
                    accessToken: _dataProtectorService.Unprotect(configurationManagement.AccessToken));

            var cmsAgentPools = await _cmsQueryService(configurationManagement.Type).GetAgentPools(cmsAuthCredential);

            OrganizationCMSAgentPoolListRp list = new OrganizationCMSAgentPoolListRp();
            list.Items = cmsAgentPools.Items.Select(c => new OrganizationCMSAgentPoolListItemRp { Id = c.Id, Name = c.Name, IsHosted = c.IsHosted }).ToList();

            return list;
        }

        public async Task<OrganizationCMSProjectListRp> GetOrganizationConfigurationManagementServiceProjects(Guid organizationId, Guid organizationCMSId)
        {
            var loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            OrganizationCMS configurationManagement = organization.GetConfigurationManagementServiceById(organizationCMSId);
            if (configurationManagement == null)
            {
                await _domainManagerService.AddNotFound($"The organzation configuration management service with id {organizationCMSId} does not exists.");
                return null;
            }

            var cmsAuthCredential = this._cmsCredentialService(configurationManagement.Type).GetToken(
                    accountId: _dataProtectorService.Unprotect(configurationManagement.AccountId),
                    accountName: _dataProtectorService.Unprotect(configurationManagement.AccountName),
                    accessSecret: _dataProtectorService.Unprotect(configurationManagement.AccessSecret),
                    accessToken: _dataProtectorService.Unprotect(configurationManagement.AccessToken));

            var projectList = await _cmsQueryService(configurationManagement.Type).GetProjects(_dataProtectorService.Unprotect(configurationManagement.AccountId), cmsAuthCredential);

            OrganizationCMSProjectListRp list = new OrganizationCMSProjectListRp();
            list.Items = projectList.Items.Select(c => new OrganizationCMSProjectListItemRp {
                ProjectId = c.Id,
                DisplayName = c.DisplayName,
                Name = c.Name }).ToList();

            return list;
        }

        public async Task<OrganizationCMSRepositoryListRp> GetOrganizationConfigurationManagementServiceRepositories(Guid organizationId, Guid organizationCMSId, string projectId)
        {
            var loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            OrganizationCMS configurationManagement = organization.GetConfigurationManagementServiceById(organizationCMSId);
            if (configurationManagement == null)
            {
                await _domainManagerService.AddNotFound($"The organzation configuration management service with id {organizationCMSId} does not exists.");
                return null;
            }

            var cmsAuthCredential = this._cmsCredentialService(configurationManagement.Type).GetToken(
                    accountId: _dataProtectorService.Unprotect(configurationManagement.AccountId),
                    accountName: _dataProtectorService.Unprotect(configurationManagement.AccountName),
                    accessSecret: _dataProtectorService.Unprotect(configurationManagement.AccessSecret),
                    accessToken: _dataProtectorService.Unprotect(configurationManagement.AccessToken));

            var repositories = await _cmsQueryService(configurationManagement.Type).GetRepositories(projectId, cmsAuthCredential);

            var list = new OrganizationCMSRepositoryListRp();

            list.Items = repositories.Items.Select(c => new OrganizationCMSRepositoryListItemRp {
                Id = c.ServiceId,
                Name = c.Name,
                Link = c.Link
            }).ToList();

            return list;
        }

        public async Task<OrganizationCMSBranchListRp> GetOrganizationConfigurationManagementServiceRepositoriesBranches(Guid organizationId, Guid organizationCMSId, 
            string projectId,
            string repositoryId)
        {
            var loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            OrganizationCMS configurationManagement = organization.GetConfigurationManagementServiceById(organizationCMSId);
            if (configurationManagement == null)
            {
                await _domainManagerService.AddNotFound($"The organzation configuration management service with id {organizationCMSId} does not exists.");
                return null;
            }

            var cmsAuthCredential = this._cmsCredentialService(configurationManagement.Type).GetToken(
                    accountId: _dataProtectorService.Unprotect(configurationManagement.AccountId),
                    accountName: _dataProtectorService.Unprotect(configurationManagement.AccountName),
                    accessSecret: _dataProtectorService.Unprotect(configurationManagement.AccessSecret),
                    accessToken: _dataProtectorService.Unprotect(configurationManagement.AccessToken));

            var repositories = await _cmsQueryService(configurationManagement.Type).GetBranches(projectId, repositoryId, cmsAuthCredential);

            var list = new OrganizationCMSBranchListRp();

            list.Items = repositories.Items.Select(c => new OrganizationCMSBranchListItemRp
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return list;
        }

        public async Task<OrganizationCMSTeamListRp> GetOrganizationConfigurationManagementServiceTeams(Guid organizationId, Guid organizationCMSId)
        {
            var loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            OrganizationCMS configurationManagement = organization.GetConfigurationManagementServiceById(organizationCMSId);
            if (configurationManagement == null)
            {
                await _domainManagerService.AddNotFound($"The organzation configuration management service with id {organizationCMSId} does not exists.");
                return null;
            }

            var cmsAuthCredential = this._cmsCredentialService(configurationManagement.Type).GetToken(
                    accountId: _dataProtectorService.Unprotect(configurationManagement.AccountId),
                    accountName: _dataProtectorService.Unprotect(configurationManagement.AccountName),
                    accessSecret: _dataProtectorService.Unprotect(configurationManagement.AccessSecret),
                    accessToken: _dataProtectorService.Unprotect(configurationManagement.AccessToken));

            var accountList = await _cmsQueryService(configurationManagement.Type).GetAccounts(cmsAuthCredential);

            OrganizationCMSTeamListRp list = new OrganizationCMSTeamListRp();
            list.Items = accountList.Items.Select(c => new OrganizationCMSTeamListItemRp
            {
                TeamId = c.AccountId,
                DisplayName = c.Name,
                Name = c.Name
            }).ToList();

            return list;
        }
    }
}
