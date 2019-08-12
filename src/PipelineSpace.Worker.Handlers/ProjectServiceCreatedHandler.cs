using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PipelineSpace.Domain;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Handlers.Extensions;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public class ProjectServiceCreatedHandler : BaseHandler, IEventHandler<ProjectServiceCreatedEvent>
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly Func<ConfigurationManagementService, IProjectServiceHandlerService> _projectServiceHandlerService;
        readonly IPipelineSpaceManagerService _pipelineSpaceManagerService;
        readonly IHttpClientWrapperService _httpClientWrapperService;

        public ProjectServiceCreatedHandler(IOptions<VSTSServiceOptions> vstsOptions,
                                            IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                            IOptions<ApplicationOptions> applicationOptions,
                                            Func<ConfigurationManagementService, IProjectServiceHandlerService> projectServiceHandlerService,
                                            IPipelineSpaceManagerService pipelineSpaceManagerService,
                                            IHttpClientWrapperService httpClientWrapperService,
                                            IRealtimeService realtimeService) : base(httpClientWrapperService, applicationOptions, realtimeService)
        {
            _vstsOptions = vstsOptions;
            _applicationOptions = applicationOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _projectServiceHandlerService = projectServiceHandlerService;
            _pipelineSpaceManagerService = pipelineSpaceManagerService;
            _httpClientWrapperService = httpClientWrapperService;
        }

        public async Task Handle(ProjectServiceCreatedEvent @event)
        {
            try
            {
                this.userId = @event.UserId;
                
                /* QUEUE - POOL ####################################################################################################################################################*/

                GetQueueResult queue = null;

                await ExecuteProjectServiceActivity(@event.OrganizationId, @event.ProjectId, @event.ServiceId, nameof(DomainConstants.Activities.PSPRRQ), async () => {

                    GetQueueOptions getQueueOptions = new GetQueueOptions();
                    getQueueOptions.CMSType = @event.CMSType;
                    getQueueOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    getQueueOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    getQueueOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    getQueueOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                    getQueueOptions.ProjectName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                    getQueueOptions.AgentPoolId = @event.AgentPoolId;

                    queue = await _pipelineSpaceManagerService.GetQueue(getQueueOptions);

                });

                /* REPOSITORY ##############################################################################################################################################*/
                string releaseDefinition = string.Empty;

                await ExecuteProjectServiceActivity(@event.OrganizationId, @event.ProjectId, @event.ServiceId, nameof(DomainConstants.Activities.PSCRRP), async () => {

                    CreateRepositoryOptions createRepositoryOptions = new CreateRepositoryOptions();
                    createRepositoryOptions.OrganizationName = @event.OrganizationName;
                    createRepositoryOptions.ProjectName = @event.InternalProjectName;
                    createRepositoryOptions.ServiceName = @event.ServiceName;
                    createRepositoryOptions.RepositoryName = @event.InternalServiceName;
                    createRepositoryOptions.VSTSAccessId = _vstsOptions.Value.AccessId;
                    createRepositoryOptions.VSTSAccessSecret = _vstsOptions.Value.AccessSecret;
                    createRepositoryOptions.VSTSRepositoryTemplateUrl = @event.ServiceTemplateUrl;
                    createRepositoryOptions.GitProviderType = @event.CMSType;
                    createRepositoryOptions.GitProviderAccessId = @event.CMSAccessId;
                    createRepositoryOptions.GitProviderAccessSecret = @event.CMSAccessSecret;
                    createRepositoryOptions.GitProviderAccessToken = @event.CMSAccessToken;
                    createRepositoryOptions.GitProviderRepositoryUrl = @event.ServiceExternalUrl;
                    createRepositoryOptions.Branch = @"refs/heads/master";
                    createRepositoryOptions.TemplateAccess = @event.TemplateAccess;
                    createRepositoryOptions.NeedCredentials = @event.NeedCredentials;
                    createRepositoryOptions.RepositoryCMSType = @event.RepositoryCMSType;
                    createRepositoryOptions.RepositoryAccessId = @event.RepositoryAccessId;
                    createRepositoryOptions.RepositoryAccessSecret = @event.RepositoryAccessSecret;
                    createRepositoryOptions.RepositoryAccessToken = @event.RepositoryAccessToken;

                    releaseDefinition = await _pipelineSpaceManagerService.CreateRepository(createRepositoryOptions);

                });
                
                /* COMMIT-STAGE ##############################################################################################################################################*/
                CreateBuildDefinitionOptions createBuildDefinitionOptions = null;
                int commitStageId = 0;
                Guid? commitServiceHookId = null;
                Guid? codeServiceHookId = null;
                
                await ExecuteProjectServiceActivity(@event.OrganizationId, @event.ProjectId, @event.ServiceId, nameof(DomainConstants.Activities.PSCRBD), async () => {

                    createBuildDefinitionOptions = new CreateBuildDefinitionOptions();
                    createBuildDefinitionOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    createBuildDefinitionOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    createBuildDefinitionOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    createBuildDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                    createBuildDefinitionOptions.ProjectName = @event.ProjectName;
                    createBuildDefinitionOptions.ServiceName = @event.ServiceName;
                    createBuildDefinitionOptions.CommitStageName = @event.InternalServiceName;
                    createBuildDefinitionOptions.GitProviderType = @event.CMSType;
                    createBuildDefinitionOptions.GitProviderAccountId = @event.CMSAccountId;
                    createBuildDefinitionOptions.GitProviderAccountName = @event.CMSAccountName;
                    createBuildDefinitionOptions.GitProviderAccessId = @event.CMSAccessId;
                    createBuildDefinitionOptions.GitProviderAccessSecret = @event.CMSAccessSecret;
                    createBuildDefinitionOptions.GitProviderAccessToken = @event.CMSAccessToken;
                    createBuildDefinitionOptions.GitProviderRepositoryId = @event.ServiceExternalId;
                    createBuildDefinitionOptions.GitProviderRepositoryUrl = @event.ServiceExternalUrl;
                    createBuildDefinitionOptions.GitProviderRepositoryBranch = @event.CMSType == ConfigurationManagementService.VSTS ? "refs/heads/master" : "master";
                    createBuildDefinitionOptions.ProjectExternalGitEndpoint = @event.ProjectExternalGitEndpoint;
                    createBuildDefinitionOptions.QueueId = queue.QueueId;
                    createBuildDefinitionOptions.QueueName = queue.QueueName;
                    createBuildDefinitionOptions.PoolId = queue.PoolId;
                    createBuildDefinitionOptions.PoolName = queue.PoolName;

                    commitStageId = await _pipelineSpaceManagerService.CreateBuildDefinition(createBuildDefinitionOptions);

                    /* SERVICE-HOOK BUILD ##############################################################################################################################################*/
                    if (@event.CMSType == ConfigurationManagementService.VSTS)
                    {
                        CreateServiceHookOptions createServiceHookCodeOptions = new CreateServiceHookOptions();
                        createServiceHookCodeOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                        createServiceHookCodeOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                        createServiceHookCodeOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                        createServiceHookCodeOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                        createServiceHookCodeOptions.OrganizationId = @event.OrganizationId;
                        createServiceHookCodeOptions.ProjectId = @event.ProjectId;
                        createServiceHookCodeOptions.ServiceId = @event.ServiceId;
                        createServiceHookCodeOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                        createServiceHookCodeOptions.EventType = "code";
                        createServiceHookCodeOptions.Definition = createBuildDefinitionOptions.CommitStageName;
                        createServiceHookCodeOptions.Url = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/{@event.ServiceId}/events";

                        createServiceHookCodeOptions.Repository = @event.ServiceExternalId;
                        createServiceHookCodeOptions.Branch = "master";

                        codeServiceHookId = await _pipelineSpaceManagerService.CreateServiceHook(createServiceHookCodeOptions);
                    }

                    CreateServiceHookOptions createServiceHookBuildOptions = new CreateServiceHookOptions();
                    createServiceHookBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    createServiceHookBuildOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    createServiceHookBuildOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    createServiceHookBuildOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                    createServiceHookBuildOptions.OrganizationId = @event.OrganizationId;
                    createServiceHookBuildOptions.ProjectId = @event.ProjectId;
                    createServiceHookBuildOptions.ServiceId = @event.ServiceId;
                    createServiceHookBuildOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                    createServiceHookBuildOptions.EventType = "build";
                    createServiceHookBuildOptions.Definition = createBuildDefinitionOptions.CommitStageName;
                    createServiceHookBuildOptions.Url = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/{@event.ServiceId}/events";

                    commitServiceHookId = await _pipelineSpaceManagerService.CreateServiceHook(createServiceHookBuildOptions);

                });
                
                /* RELEASE-STAGE ##############################################################################################################################################*/
                CreateReleaseDefinitionOptions createReleaseDefinitionOptions = null;
                int? releaseStageId = null;
                Guid? releaseServiceHookId = null;
                Guid? releaseStartedServiceHookId = null;
                Guid? releasePendingApprovalServiceHookId = null;
                Guid? releaseCompletedApprovalServiceHookId = null;
                
                await ExecuteProjectServiceActivity(@event.OrganizationId, @event.ProjectId, @event.ServiceId, nameof(DomainConstants.Activities.PSCRRD), async () => {

                    if (@event.CPSType != CloudProviderService.None) {
                        createReleaseDefinitionOptions = new CreateReleaseDefinitionOptions();
                        createReleaseDefinitionOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                        createReleaseDefinitionOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                        createReleaseDefinitionOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                        createReleaseDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                        createReleaseDefinitionOptions.OrganizationName = @event.OrganizationName;
                        createReleaseDefinitionOptions.ProjectName = @event.InternalProjectName;
                        createReleaseDefinitionOptions.ServiceName = @event.ServiceName;
                        createReleaseDefinitionOptions.CommitStageName = createBuildDefinitionOptions.CommitStageName;
                        createReleaseDefinitionOptions.ReleaseStageName = createBuildDefinitionOptions.CommitStageName;
                        createReleaseDefinitionOptions.BuildDefinitionName = createBuildDefinitionOptions.CommitStageName;
                        createReleaseDefinitionOptions.ReleaseDefinition = releaseDefinition;
                        createReleaseDefinitionOptions.CloudProviderEndpointId = @event.ProjectExternalEndpointId;
                        createReleaseDefinitionOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                        createReleaseDefinitionOptions.CommitStageId = commitStageId;
                        createReleaseDefinitionOptions.QueueId = queue.QueueId;
                        createReleaseDefinitionOptions.CloudProviderAccessId = @event.CPSAccessId;
                        createReleaseDefinitionOptions.CloudProviderAccessSecret = @event.CPSAccessSecret;
                        createReleaseDefinitionOptions.CloudProviderAccessRegion = @event.CPSAccessRegion;
                        createReleaseDefinitionOptions.WorkFeature = "Root";
                        createReleaseDefinitionOptions.TemplateParameters = @event.TemplateParameters.Select(x => new ProjectServiceTemplateParameterOptions()
                        {
                            VariableName = x.VariableName,
                            Value = x.Value,
                            Scope = x.Scope
                        }).ToList();

                        releaseStageId = await _pipelineSpaceManagerService.CreateReleaseDefinition(createReleaseDefinitionOptions);

                        if (releaseStageId.HasValue)
                        {
                            /* SERVICE-HOOK RELEASE ##############################################################################################################################################*/
                            CreateServiceHookOptions createServiceHookReleaseStartedOptions = new CreateServiceHookOptions();
                            createServiceHookReleaseStartedOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                            createServiceHookReleaseStartedOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                            createServiceHookReleaseStartedOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                            createServiceHookReleaseStartedOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                            createServiceHookReleaseStartedOptions.OrganizationId = @event.OrganizationId;
                            createServiceHookReleaseStartedOptions.ProjectId = @event.ProjectId;
                            createServiceHookReleaseStartedOptions.ServiceId = @event.ServiceId;
                            createServiceHookReleaseStartedOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                            createServiceHookReleaseStartedOptions.EventType = "releaseStarted";
                            createServiceHookReleaseStartedOptions.Definition = releaseStageId.Value.ToString();
                            createServiceHookReleaseStartedOptions.Url = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/{@event.ServiceId}/events";

                            releaseStartedServiceHookId = await _pipelineSpaceManagerService.CreateServiceHook(createServiceHookReleaseStartedOptions);

                            CreateServiceHookOptions createServiceHookReleasePendingApprovalOptions = new CreateServiceHookOptions();
                            createServiceHookReleasePendingApprovalOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                            createServiceHookReleasePendingApprovalOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                            createServiceHookReleasePendingApprovalOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                            createServiceHookReleasePendingApprovalOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                            createServiceHookReleasePendingApprovalOptions.OrganizationId = @event.OrganizationId;
                            createServiceHookReleasePendingApprovalOptions.ProjectId = @event.ProjectId;
                            createServiceHookReleasePendingApprovalOptions.ServiceId = @event.ServiceId;
                            createServiceHookReleasePendingApprovalOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                            createServiceHookReleasePendingApprovalOptions.EventType = "releasePendingApproval";
                            createServiceHookReleasePendingApprovalOptions.Definition = releaseStageId.Value.ToString();
                            createServiceHookReleasePendingApprovalOptions.Url = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/{@event.ServiceId}/events";

                            releasePendingApprovalServiceHookId = await _pipelineSpaceManagerService.CreateServiceHook(createServiceHookReleasePendingApprovalOptions);

                            CreateServiceHookOptions createServiceHookReleaseCompletedApprovalOptions = new CreateServiceHookOptions();
                            createServiceHookReleaseCompletedApprovalOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                            createServiceHookReleaseCompletedApprovalOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                            createServiceHookReleaseCompletedApprovalOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                            createServiceHookReleaseCompletedApprovalOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                            createServiceHookReleaseCompletedApprovalOptions.OrganizationId = @event.OrganizationId;
                            createServiceHookReleaseCompletedApprovalOptions.ProjectId = @event.ProjectId;
                            createServiceHookReleaseCompletedApprovalOptions.ServiceId = @event.ServiceId;
                            createServiceHookReleaseCompletedApprovalOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                            createServiceHookReleaseCompletedApprovalOptions.EventType = "releaseCompletedApproval";
                            createServiceHookReleaseCompletedApprovalOptions.Definition = releaseStageId.Value.ToString();
                            createServiceHookReleaseCompletedApprovalOptions.Url = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/{@event.ServiceId}/events";

                            releaseCompletedApprovalServiceHookId = await _pipelineSpaceManagerService.CreateServiceHook(createServiceHookReleaseCompletedApprovalOptions);

                            CreateServiceHookOptions createServiceHookReleaseOptions = new CreateServiceHookOptions();
                            createServiceHookReleaseOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                            createServiceHookReleaseOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                            createServiceHookReleaseOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                            createServiceHookReleaseOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                            createServiceHookReleaseOptions.OrganizationId = @event.OrganizationId;
                            createServiceHookReleaseOptions.ProjectId = @event.ProjectId;
                            createServiceHookReleaseOptions.ServiceId = @event.ServiceId;
                            createServiceHookReleaseOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                            createServiceHookReleaseOptions.EventType = "release";
                            createServiceHookReleaseOptions.Definition = releaseStageId.Value.ToString();
                            createServiceHookReleaseOptions.Url = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/{@event.ServiceId}/events";

                            releaseServiceHookId = await _pipelineSpaceManagerService.CreateServiceHook(createServiceHookReleaseOptions);
                        }

                    }

                });

                /* QUEUE - BUILD ##############################################################################################################################################*/

                await ExecuteProjectServiceActivity(@event.OrganizationId, @event.ProjectId, @event.ServiceId, nameof(DomainConstants.Activities.PSQUDB), async () => {
                    
                    QueueBuildOptions queueBuildOptions = new QueueBuildOptions();
                    queueBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    queueBuildOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    queueBuildOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    queueBuildOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                    queueBuildOptions.ProjectName = @event.ProjectName;
                    queueBuildOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                    queueBuildOptions.QueueId = queue.QueueId;
                    queueBuildOptions.BuildDefinitionId = commitStageId;
                    queueBuildOptions.SourceBranch = @event.CMSType == ConfigurationManagementService.VSTS ? "refs/heads/master" : "master";

                    await _pipelineSpaceManagerService.QueueBuild(queueBuildOptions);

                    string eventUrl = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/{@event.ServiceId}/events";
                    await _httpClientWrapperService.PostAsync(eventUrl, new {
                        SubscriptionId  = Guid.NewGuid(),
                        NotificationId = 1,
                        Id = string.Empty,
                        EventType = "git.push",
                        PublisherId = "ps",
                        Message = new
                        {
                            Text = "PipelineSpace initial build"
                        },
                        DetailedMessage = new { },
                        Resource = new { },
                        Status = "Queued",
                        Date = DateTime.UtcNow
                    }, InternalAuthCredentials);

                });

                /* ACTIVATE SERVICE ##############################################################################################################################################*/

                await ExecuteProjectServiceActivity(@event.OrganizationId, @event.ProjectId, @event.ServiceId, nameof(DomainConstants.Activities.PSACBA), async () => {

                    string projectServicePatchUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/{@event.ServiceId}";
                    var projectServicePatchResponse = await _httpClientWrapperService.PatchAsync(projectServicePatchUrl,
                            new
                            {
                                CommitStageId = commitStageId,
                                ReleaseStageId = releaseStageId,
                                CommitServiceHookId = commitServiceHookId,
                                ReleaseServiceHookId = releaseServiceHookId,
                                CodeServiceHookId = codeServiceHookId,
                                ReleaseStartedServiceHookId = releaseStartedServiceHookId,
                                ReleasePendingApprovalServiceHookId = releasePendingApprovalServiceHookId,
                                ReleaseCompletedApprovalServiceHookId = releaseCompletedApprovalServiceHookId,
                                PipelineStatus = PipelineStatus.Building
                            }, InternalAuthCredentials);
                    projectServicePatchResponse.EnsureSuccessStatusCode();

                });
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
