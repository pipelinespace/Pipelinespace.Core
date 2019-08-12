
using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using System;

namespace PipelineSpace.Application.Services
{
    public class ProjectCloudCredentialService : IProjectCloudCredentialService
    {
        readonly IDataProtectorService _dataProtectorService;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;

        public ProjectCloudCredentialService(IDataProtectorService dataProtectorService,
            Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService,
            IOptions<FakeAccountServiceOptions> fakeAccountOptions)
        {
            this._dataProtectorService = dataProtectorService;
            this._fakeAccountOptions = fakeAccountOptions;
            this._cmsCredentialService = cmsCredentialService;
        }

        public ProjectCloudCredentialModel ProjectCredentialResolver(OrganizationCMS organizationCMS, Project project)
        {
            var accountId = organizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(organizationCMS.AccountId) : this._fakeAccountOptions.Value.AccountId;
            var accountName = organizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(organizationCMS.AccountName) : _fakeAccountOptions.Value.AccountId;
            var accessSecret = organizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(organizationCMS.AccessSecret) : _fakeAccountOptions.Value.AccessSecret;
            var accountProjectId = organizationCMS.Type == ConfigurationManagementService.VSTS ? project.Name : project.ProjectVSTSFakeName;
            var projectName = organizationCMS.Type == ConfigurationManagementService.VSTS ? project.Name : project.ProjectVSTSFakeName;
            var projectExternalId = organizationCMS.Type == ConfigurationManagementService.VSTS ? project.ProjectExternalId : project.ProjectVSTSFakeId;
            
            var accessToken = organizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(organizationCMS.AccessToken) : this._fakeAccountOptions.Value.AccessSecret;
            var accessId = organizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(organizationCMS.AccessId) : this._fakeAccountOptions.Value.AccessId;
            var cmsType = organizationCMS.Type == ConfigurationManagementService.VSTS ? organizationCMS.Type : ConfigurationManagementService.VSTS;
            CMSAuthCredentialModel cmsAuthCredential = this._cmsCredentialService(cmsType).GetToken(accountId,
                                                                    accountName,
                                                                    accessSecret,
                                                                    accessToken);

            return new ProjectCloudCredentialModel {
                AccessId = accessId,
                AccountId = accountId,
                AccessToken = accessToken,
                AccountName = accountName,
                AccessSecret = accessSecret,
                AccountProjectId = accountProjectId,
                ProjectExternalId = projectExternalId,
                ProjectName = projectName,
                CMSType = cmsType,
                CMSAuthCredential = cmsAuthCredential
            };
        }

        public ProjectCloudCredentialModel ProjectFeatureServiceCredentialResolver(Project project, Domain.Models.ProjectFeatureService projectFeatureService)
        {
            var accountName = projectFeatureService.ProjectService.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(projectFeatureService.ProjectService.OrganizationCMS.AccountName) : _fakeAccountOptions.Value.AccountId;
            var accessSecret = projectFeatureService.ProjectService.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(projectFeatureService.ProjectService.OrganizationCMS.AccessSecret) : _fakeAccountOptions.Value.AccessSecret;
            var accountProjectId = projectFeatureService.ProjectService.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? projectFeatureService.ProjectService.ProjectExternalName : project.ProjectVSTSFakeName;
            var projectName = projectFeatureService.ProjectService.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? projectFeatureService.ProjectService.ProjectExternalName : project.ProjectVSTSFakeName;
            var projectExternalId = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? projectFeatureService.ProjectService.ProjectExternalId : project.ProjectVSTSFakeId;

            var accountId = _dataProtectorService.Unprotect(projectFeatureService.ProjectService.OrganizationCMS.AccountId);
            var accessToken = _dataProtectorService.Unprotect(projectFeatureService.ProjectService.OrganizationCMS.AccessToken);
            var accessId = _dataProtectorService.Unprotect(projectFeatureService.ProjectService.OrganizationCMS.AccessId);

            var repositoryCMSType = projectFeatureService.ProjectService.ProjectServiceTemplate.TemplateAccess == Domain.Models.Enums.TemplateAccess.Organization ? projectFeatureService.ProjectService.ProjectServiceTemplate.Credential.CMSType : ConfigurationManagementService.VSTS;
            var repositoryAccessId = projectFeatureService.ProjectService.ProjectServiceTemplate.TemplateAccess == Domain.Models.Enums.TemplateAccess.Organization ? projectFeatureService.ProjectService.ProjectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectFeatureService.ProjectService.ProjectServiceTemplate.Credential.AccessId) : string.Empty : string.Empty;
            var repositoryAccessSecret = projectFeatureService.ProjectService.ProjectServiceTemplate.TemplateAccess == Domain.Models.Enums.TemplateAccess.Organization ? projectFeatureService.ProjectService.ProjectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectFeatureService.ProjectService.ProjectServiceTemplate.Credential.AccessSecret) : string.Empty : string.Empty;
            var repositoryAccessToken = projectFeatureService.ProjectService.ProjectServiceTemplate.TemplateAccess == Domain.Models.Enums.TemplateAccess.Organization ? projectFeatureService.ProjectService.ProjectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectFeatureService.ProjectService.ProjectServiceTemplate.Credential.AccessToken) : string.Empty : string.Empty;
            var branchUrl = projectFeatureService.ProjectService.ProjectServiceTemplate.Url;

            if (projectFeatureService.ProjectService.IsImported)
            {
                accountProjectId = projectFeatureService.ProjectService.ProjectExternalName;
                projectName = projectFeatureService.ProjectService.ProjectExternalName;
                projectExternalId = projectFeatureService.ProjectService.ProjectExternalId;
                repositoryAccessId = accessId;
                repositoryAccessToken = accessToken;
                repositoryAccessSecret = accessSecret;
                repositoryAccessToken = accessToken;
                branchUrl = projectFeatureService.ProjectService.ProjectBranchServiceExternalUrl;
            }

            return new ProjectCloudCredentialModel
            {
                AccessId = accessId,
                AccountId = accountId,
                AccessToken = accessToken,
                AccountName = accountName,
                AccessSecret = accessSecret,
                AccountProjectId = accountProjectId,
                ProjectExternalId = projectExternalId,
                ProjectName = projectName,
                CMSType = projectFeatureService.ProjectService.OrganizationCMS.Type,
                BranchUrl = branchUrl
            };
        }

        public ProjectCloudCredentialModel ProjectServiceCredentialResolver(Project project, Domain.Models.ProjectService projectService)
        {
            var accountName = projectService.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(projectService.OrganizationCMS.AccountName) : _fakeAccountOptions.Value.AccountId;
            var accessSecret = projectService.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(projectService.OrganizationCMS.AccessSecret) : _fakeAccountOptions.Value.AccessSecret;
            var accountProjectId = projectService.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.ProjectExternalName : project.ProjectVSTSFakeName;
            var projectName = projectService.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.ProjectExternalName : project.ProjectVSTSFakeName;
            var projectExternalId = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.ProjectExternalId : project.ProjectVSTSFakeId;

            var accountId = _dataProtectorService.Unprotect(projectService.OrganizationCMS.AccountId);
            var accessToken = _dataProtectorService.Unprotect(projectService.OrganizationCMS.AccessToken);
            var accessId = _dataProtectorService.Unprotect(projectService.OrganizationCMS.AccessId);

            var repositoryCMSType = projectService.ProjectServiceTemplate.TemplateAccess == Domain.Models.Enums.TemplateAccess.Organization ? projectService.ProjectServiceTemplate.Credential.CMSType : ConfigurationManagementService.VSTS;
            var repositoryAccessId = projectService.ProjectServiceTemplate.TemplateAccess == Domain.Models.Enums.TemplateAccess.Organization ? projectService.ProjectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectService.ProjectServiceTemplate.Credential.AccessId) : string.Empty : string.Empty;
            var repositoryAccessSecret = projectService.ProjectServiceTemplate.TemplateAccess == Domain.Models.Enums.TemplateAccess.Organization ? projectService.ProjectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectService.ProjectServiceTemplate.Credential.AccessSecret) : string.Empty : string.Empty;
            var repositoryAccessToken = projectService.ProjectServiceTemplate.TemplateAccess == Domain.Models.Enums.TemplateAccess.Organization ? projectService.ProjectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectService.ProjectServiceTemplate.Credential.AccessToken) : string.Empty : string.Empty;
            var branchUrl = projectService.ProjectServiceTemplate.Url;

            if (projectService.IsImported)
            {
                accountProjectId = projectService.ProjectExternalName;
                projectName = projectService.ProjectExternalName;
                projectExternalId = projectService.ProjectExternalId;
                repositoryAccessId = accessId;
                repositoryAccessToken = accessToken;
                repositoryAccessSecret = accessSecret;
                repositoryAccessToken = accessToken;
                branchUrl = projectService.ProjectBranchServiceExternalUrl;
            }

            return new ProjectCloudCredentialModel {
                AccessId = accessId,
                AccountId = accountId,
                AccessToken = accessToken,
                AccountName = accountName,
                AccessSecret = accessSecret, 
                AccountProjectId = accountProjectId,
                ProjectExternalId = projectExternalId,
                ProjectName = projectName,
                CMSType = projectService.OrganizationCMS.Type,
                BranchUrl = branchUrl
            };
        }
        
    }
}
