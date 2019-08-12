using Microsoft.Extensions.Options;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public class ProjectFeatureServiceDeletedHandler : IEventHandler<ProjectFeatureServiceDeletedEvent>
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly Func<ConfigurationManagementService, IProjectServiceHandlerService> _projectServiceHandlerService;
        readonly Func<CloudProviderService, ICPSService> _cpsService;
        readonly IPipelineSpaceManagerService _pipelineSpaceManagerService;

        public ProjectFeatureServiceDeletedHandler(IOptions<VSTSServiceOptions> vstsOptions,
                                                   IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                                   IOptions<ApplicationOptions> applicationOptions,
                                                   Func<ConfigurationManagementService, IProjectServiceHandlerService> projectServiceHandlerService,
                                                   Func<CloudProviderService, ICPSService> cpsService,
                                                   IPipelineSpaceManagerService pipelineSpaceManagerService)
        {
            _vstsOptions = vstsOptions;
            _applicationOptions = applicationOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _projectServiceHandlerService = projectServiceHandlerService;
            _cpsService = cpsService;
            _pipelineSpaceManagerService = pipelineSpaceManagerService;
        }

        public async Task Handle(ProjectFeatureServiceDeletedEvent @event)
        {

            CPSAuthModel authModel = new CPSAuthModel();
            authModel.AccessId = @event.CPSAccessId;
            authModel.AccessName = @event.CPSAccessName;
            authModel.AccessSecret = @event.CPSAccessSecret;
            authModel.AccessRegion = @event.CPSAccessRegion;
            authModel.AccessAppId = @event.CPSAccessAppId;
            authModel.AccessAppSecret = @event.CPSAccessAppSecret;
            authModel.AccessDirectory = @event.CPSAccessDirectory;

            /*cloud service (stack or resourcegroup)*/
            string cloudServiceName = $"{@event.OrganizationName}{@event.ProjectName}{@event.ServiceName}development{@event.FeatureName}".ToLower();

            await _cpsService(@event.CPSType).DeleteService(cloudServiceName, authModel);

            DeleteServiceHookOptions deleteServiceHookReleaseStartedOptions = new DeleteServiceHookOptions();
            deleteServiceHookReleaseStartedOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            deleteServiceHookReleaseStartedOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
            deleteServiceHookReleaseStartedOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
            deleteServiceHookReleaseStartedOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
            deleteServiceHookReleaseStartedOptions.EventType = "release";
            deleteServiceHookReleaseStartedOptions.ServiceHookId = @event.ReleaseStartedServiceHookId.Value;

            await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookReleaseStartedOptions);

            DeleteServiceHookOptions deleteServiceHookReleasePendingApprovalOptions = new DeleteServiceHookOptions();
            deleteServiceHookReleasePendingApprovalOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            deleteServiceHookReleasePendingApprovalOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
            deleteServiceHookReleasePendingApprovalOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
            deleteServiceHookReleasePendingApprovalOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
            deleteServiceHookReleasePendingApprovalOptions.EventType = "release";
            deleteServiceHookReleasePendingApprovalOptions.ServiceHookId = @event.ReleasePendingApprovalServiceHookId.Value;

            await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookReleasePendingApprovalOptions);

            DeleteServiceHookOptions deleteServiceHookReleaseCompletedApprovalOptions = new DeleteServiceHookOptions();
            deleteServiceHookReleaseCompletedApprovalOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            deleteServiceHookReleaseCompletedApprovalOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
            deleteServiceHookReleaseCompletedApprovalOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
            deleteServiceHookReleaseCompletedApprovalOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
            deleteServiceHookReleaseCompletedApprovalOptions.EventType = "release";
            deleteServiceHookReleaseCompletedApprovalOptions.ServiceHookId = @event.ReleaseCompletedApprovalServiceHookId.Value;

            await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookReleaseCompletedApprovalOptions);

            DeleteServiceHookOptions deleteServiceHookBuildOptions = new DeleteServiceHookOptions();
            deleteServiceHookBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            deleteServiceHookBuildOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
            deleteServiceHookBuildOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
            deleteServiceHookBuildOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
            deleteServiceHookBuildOptions.EventType = "release";
            deleteServiceHookBuildOptions.ServiceHookId = @event.ReleaseServiceHookId.Value;

            await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookBuildOptions);

            DeleteReleaseDefinitionOptions deleteReleaseDefinitionOptions = new DeleteReleaseDefinitionOptions();
            deleteReleaseDefinitionOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            deleteReleaseDefinitionOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
            deleteReleaseDefinitionOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
            deleteReleaseDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
            deleteReleaseDefinitionOptions.ProjectExternalId = @event.ProjectExternalId;
            deleteReleaseDefinitionOptions.ReleaseStageId = @event.ReleaseStageId;

            await _pipelineSpaceManagerService.DeleteReleaseDefinition(deleteReleaseDefinitionOptions);

            if (@event.CMSType == ConfigurationManagementService.VSTS)
            {
                DeleteServiceHookOptions deleteServiceHookCodeOptions = new DeleteServiceHookOptions();
                deleteServiceHookCodeOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                deleteServiceHookCodeOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                deleteServiceHookCodeOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                deleteServiceHookCodeOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                deleteServiceHookCodeOptions.EventType = "build";
                deleteServiceHookCodeOptions.ServiceHookId = @event.CodeServiceHookId.Value;

                await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookCodeOptions);
            }

            DeleteServiceHookOptions deleteServiceHookReleaseOptions = new DeleteServiceHookOptions();
            deleteServiceHookReleaseOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            deleteServiceHookReleaseOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
            deleteServiceHookReleaseOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
            deleteServiceHookReleaseOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
            deleteServiceHookReleaseOptions.EventType = "build";
            deleteServiceHookReleaseOptions.ServiceHookId = @event.CommitServiceHookId.Value;

            await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookReleaseOptions);

            DeleteBuildDefinitionOptions deleteBuildDefinitionOptions = new DeleteBuildDefinitionOptions();
            deleteBuildDefinitionOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            deleteBuildDefinitionOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
            deleteBuildDefinitionOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
            deleteBuildDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
            deleteBuildDefinitionOptions.ProjectExternalId = @event.ProjectExternalId;
            deleteBuildDefinitionOptions.CommitStageId = @event.CommitStageId;

            await _pipelineSpaceManagerService.DeleteBuildDefinition(deleteBuildDefinitionOptions);

            DeleteBranchOptions deleteBranchOptions = new DeleteBranchOptions();
            deleteBranchOptions.ProjectName = @event.ProjectName;
            deleteBranchOptions.FeatureName = @event.FeatureName;
            deleteBranchOptions.ServiceName = @event.ServiceName;
            deleteBranchOptions.VSTSAccessId = _vstsOptions.Value.AccessId;
            deleteBranchOptions.VSTSAccessSecret = _vstsOptions.Value.AccessSecret;
            deleteBranchOptions.VSTSRepositoryTemplateUrl = @event.ServiceTemplateUrl;
            deleteBranchOptions.GitProviderType = @event.CMSType;
            deleteBranchOptions.GitProviderAccessId = @event.CMSAccessId;
            deleteBranchOptions.GitProviderAccessSecret = @event.CMSAccessSecret;
            deleteBranchOptions.GitProviderAccessToken = @event.CMSAccessToken;
            deleteBranchOptions.GitProviderRepositoryUrl = @event.ServiceExternalUrl;

            await _pipelineSpaceManagerService.DeleteBranch(deleteBranchOptions);
        }


    }
}
