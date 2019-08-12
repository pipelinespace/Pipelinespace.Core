using Microsoft.Extensions.Options;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public class ProjectServiceDeletedHandler : IEventHandler<ProjectServiceDeletedEvent>
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly Func<ConfigurationManagementService, IProjectServiceHandlerService> _projectServiceHandlerService;
        readonly Func<CloudProviderService, ICPSService> _cpsService;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IPipelineSpaceManagerService _pipelineSpaceManagerService;

        public ProjectServiceDeletedHandler(IOptions<VSTSServiceOptions> vstsOptions, 
                                            Func<ConfigurationManagementService, IProjectServiceHandlerService> projectServiceHandlerService,
                                            Func<CloudProviderService, ICPSService> cpsService,
                                            IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                            IPipelineSpaceManagerService pipelineSpaceManagerService)
        {
            _vstsOptions = vstsOptions;
            _projectServiceHandlerService = projectServiceHandlerService;
            _cpsService = cpsService;
            _fakeAccountOptions = fakeAccountOptions;
            _pipelineSpaceManagerService = pipelineSpaceManagerService;
        }

        public async Task Handle(ProjectServiceDeletedEvent @event)
        {
            CPSAuthModel authModel = new CPSAuthModel();
            authModel.AccessId = @event.CPSAccessId;
            authModel.AccessName = @event.CPSAccessName;
            authModel.AccessSecret = @event.CPSAccessSecret;
            authModel.AccessRegion = @event.CPSAccessRegion;
            authModel.AccessAppId = @event.CPSAccessAppId;
            authModel.AccessAppSecret = @event.CPSAccessAppSecret;
            authModel.AccessDirectory = @event.CPSAccessDirectory;

            if (@event.CPSType != CloudProviderService.None) {
                foreach (var environmentName in @event.Environments)
                {
                    /*cloud service (stack or resourcegroup)*/
                    string cloudServiceName = $"{@event.OrganizationName}{@event.ProjectName}{@event.ServiceName}{environmentName}root".ToLower();
                    await _cpsService(@event.CPSType).DeleteService(cloudServiceName, authModel);
                }
            }
            

            if (@event.SourceEvent == Domain.Models.Enums.SourceEvent.Service)
            {
                if (@event.ReleaseStartedServiceHookId.HasValue) {
                    DeleteServiceHookOptions deleteServiceHookReleaseStartedOptions = new DeleteServiceHookOptions();
                    deleteServiceHookReleaseStartedOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    deleteServiceHookReleaseStartedOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    deleteServiceHookReleaseStartedOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    deleteServiceHookReleaseStartedOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                    deleteServiceHookReleaseStartedOptions.EventType = "release";
                    deleteServiceHookReleaseStartedOptions.ServiceHookId = @event.ReleaseStartedServiceHookId.Value;

                    await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookReleaseStartedOptions);
                }

                if (@event.ReleasePendingApprovalServiceHookId.HasValue) {
                    DeleteServiceHookOptions deleteServiceHookReleasePendingApprovalOptions = new DeleteServiceHookOptions();
                    deleteServiceHookReleasePendingApprovalOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    deleteServiceHookReleasePendingApprovalOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    deleteServiceHookReleasePendingApprovalOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    deleteServiceHookReleasePendingApprovalOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                    deleteServiceHookReleasePendingApprovalOptions.EventType = "release";
                    deleteServiceHookReleasePendingApprovalOptions.ServiceHookId = @event.ReleasePendingApprovalServiceHookId.Value;

                    await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookReleasePendingApprovalOptions);
                }

                if (@event.ReleaseCompletedApprovalServiceHookId.HasValue) {
                    DeleteServiceHookOptions deleteServiceHookReleaseCompletedApprovalOptions = new DeleteServiceHookOptions();
                    deleteServiceHookReleaseCompletedApprovalOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    deleteServiceHookReleaseCompletedApprovalOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    deleteServiceHookReleaseCompletedApprovalOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    deleteServiceHookReleaseCompletedApprovalOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                    deleteServiceHookReleaseCompletedApprovalOptions.EventType = "release";
                    deleteServiceHookReleaseCompletedApprovalOptions.ServiceHookId = @event.ReleaseCompletedApprovalServiceHookId.Value;

                    await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookReleaseCompletedApprovalOptions);
                }

                if (@event.ReleaseServiceHookId.HasValue) {
                    DeleteServiceHookOptions deleteServiceHookReleaseOptions = new DeleteServiceHookOptions();
                    deleteServiceHookReleaseOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    deleteServiceHookReleaseOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    deleteServiceHookReleaseOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    deleteServiceHookReleaseOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                    deleteServiceHookReleaseOptions.EventType = "release";
                    deleteServiceHookReleaseOptions.ServiceHookId = @event.ReleaseServiceHookId.Value;

                    await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookReleaseOptions);
                }

                if (@event.ReleaseStageId.HasValue) {
                    DeleteReleaseDefinitionOptions deleteReleaseDefinitionOptions = new DeleteReleaseDefinitionOptions();
                    deleteReleaseDefinitionOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    deleteReleaseDefinitionOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    deleteReleaseDefinitionOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    deleteReleaseDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                    deleteReleaseDefinitionOptions.ProjectExternalId = @event.ProjectExternalId;
                    deleteReleaseDefinitionOptions.ReleaseStageId = @event.ReleaseStageId;

                    await _pipelineSpaceManagerService.DeleteReleaseDefinition(deleteReleaseDefinitionOptions);
                }
                
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

                if (@event.CommitServiceHookId.HasValue) {
                    DeleteServiceHookOptions deleteServiceHookBuildOptions = new DeleteServiceHookOptions();
                    deleteServiceHookBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    deleteServiceHookBuildOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    deleteServiceHookBuildOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    deleteServiceHookBuildOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                    deleteServiceHookBuildOptions.EventType = "build";
                    deleteServiceHookBuildOptions.ServiceHookId = @event.CommitServiceHookId.Value;

                    await _pipelineSpaceManagerService.DeleteServiceHook(deleteServiceHookBuildOptions);
                }

                if (@event.CommitStageId.HasValue) {
                    DeleteBuildDefinitionOptions deleteBuildDefinitionOptions = new DeleteBuildDefinitionOptions();
                    deleteBuildDefinitionOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    deleteBuildDefinitionOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    deleteBuildDefinitionOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    deleteBuildDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                    deleteBuildDefinitionOptions.ProjectExternalId = @event.ProjectExternalId;
                    deleteBuildDefinitionOptions.CommitStageId = @event.CommitStageId;

                    await _pipelineSpaceManagerService.DeleteBuildDefinition(deleteBuildDefinitionOptions);
                }
                
                await _projectServiceHandlerService(@event.CMSType).DeleteRepository(@event);
            }

            if (@event.SourceEvent == Domain.Models.Enums.SourceEvent.Organization ||
                @event.SourceEvent == Domain.Models.Enums.SourceEvent.Project)
            {
                if(@event.CMSType == ConfigurationManagementService.GitHub || @event.CMSType == ConfigurationManagementService.Bitbucket || @event.CMSType == ConfigurationManagementService.GitLab)
                {
                    await _projectServiceHandlerService(@event.CMSType).DeleteRepository(@event);
                }
            }
        }
    }
}
