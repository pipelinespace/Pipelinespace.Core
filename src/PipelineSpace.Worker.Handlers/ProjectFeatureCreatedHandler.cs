using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PipelineSpace.Domain;
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
    public class ProjectFeatureCreatedHandler : BaseHandler, IEventHandler<ProjectFeatureCreatedEvent>
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly Func<ConfigurationManagementService, IProjectServiceHandlerService> _projectServiceHandlerService;
        readonly IPipelineSpaceManagerService _pipelineSpaceManagerService;
        readonly IHttpClientWrapperService _httpClientWrapperService;

        public ProjectFeatureCreatedHandler(IOptions<VSTSServiceOptions> vstsOptions,
                                            IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                            IOptions<ApplicationOptions> applicationOptions,
                                            Func<ConfigurationManagementService, IProjectServiceHandlerService> projectServiceHandlerService,
                                            IPipelineSpaceManagerService pipelineSpaceManagerService,
                                            IRealtimeService realtimeService,
                                            IHttpClientWrapperService httpClientWrapperService) : base(httpClientWrapperService, applicationOptions, realtimeService)
        {
            _vstsOptions = vstsOptions;
            _applicationOptions = applicationOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _projectServiceHandlerService = projectServiceHandlerService;
            _pipelineSpaceManagerService = pipelineSpaceManagerService;
            _httpClientWrapperService = httpClientWrapperService;
        }

        public async Task Handle(ProjectFeatureCreatedEvent @event)
        {
            this.userId = @event.UserId;
            /* REPOSITORY ##############################################################################################################################################*/
            foreach (var item in @event.Services)
            {
                /* QUEUE - POOL ####################################################################################################################################################*/
                GetQueueResult queue = null;

                await ExecuteProjectFeatureServiceActivity(@event.OrganizationId, @event.ProjectId, @event.FeatureId, item.ServiceId, nameof(DomainConstants.Activities.PSPRRQ), async () => {

                    GetQueueOptions getQueueOptions = new GetQueueOptions();
                    getQueueOptions.CMSType = @event.CMSType;
                    getQueueOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    getQueueOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    getQueueOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    getQueueOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                    getQueueOptions.ProjectName = @event.ProjectName;
                    getQueueOptions.AgentPoolId = @event.AgentPoolId;

                    queue = await _pipelineSpaceManagerService.GetQueue(getQueueOptions);
                });

                string releaseDefinition = string.Empty;

                await ExecuteProjectFeatureServiceActivity(@event.OrganizationId, @event.ProjectId, @event.FeatureId, item.ServiceId, nameof(DomainConstants.Activities.PSCRBR), async () =>
                {
                    CreateBranchOptions createBranchOptions = new CreateBranchOptions();
                    createBranchOptions.ProjectName = @event.ProjectName;
                    createBranchOptions.FeatureName = @event.FeatureName;
                    createBranchOptions.ServiceName = item.ServiceName;
                    createBranchOptions.VSTSAccessId = _vstsOptions.Value.AccessId;
                    createBranchOptions.VSTSAccessSecret = _vstsOptions.Value.AccessSecret;
                    createBranchOptions.VSTSRepositoryTemplateUrl = item.ServiceTemplateUrl;
                    createBranchOptions.GitProviderType = @event.CMSType;
                    createBranchOptions.GitProviderAccessId = @event.CMSAccessId;
                    createBranchOptions.GitProviderAccessSecret = @event.CMSAccessSecret;
                    createBranchOptions.GitProviderAccessToken = @event.CMSAccessToken;
                    createBranchOptions.GitProviderRepositoryUrl = item.ServiceExternalUrl;
                    createBranchOptions.TemplateAccess = @event.TemplateAccess;
                    createBranchOptions.NeedCredentials = @event.NeedCredentials;
                    createBranchOptions.RepositoryCMSType = @event.RepositoryCMSType;
                    createBranchOptions.RepositoryAccessId = @event.RepositoryAccessId;
                    createBranchOptions.RepositoryAccessSecret = @event.RepositoryAccessSecret;
                    createBranchOptions.RepositoryAccessToken = @event.RepositoryAccessToken;

                    releaseDefinition = await _pipelineSpaceManagerService.CreateBranch(createBranchOptions);
                });

                /* COMMIT-STAGE ##############################################################################################################################################*/
                int commitStageId = 0;
                Guid? commitServiceHookId = null;

                CreateBuildDefinitionOptions createBuildDefinitionOptions = new CreateBuildDefinitionOptions();

                await ExecuteProjectFeatureServiceActivity(@event.OrganizationId, @event.ProjectId, @event.FeatureId, item.ServiceId, nameof(DomainConstants.Activities.PSCRBD), async () => {

                    createBuildDefinitionOptions = new CreateBuildDefinitionOptions();
                    createBuildDefinitionOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    createBuildDefinitionOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    createBuildDefinitionOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    createBuildDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                    createBuildDefinitionOptions.ProjectName = @event.ProjectName;
                    createBuildDefinitionOptions.ServiceName = item.ServiceName;
                    createBuildDefinitionOptions.CommitStageName = $"{item.ServiceName}-ft-{@event.FeatureName}";
                    createBuildDefinitionOptions.GitProviderType = @event.CMSType;
                    createBuildDefinitionOptions.GitProviderAccountId = @event.CMSAccountId;
                    createBuildDefinitionOptions.GitProviderAccountName = @event.CMSAccountName;
                    createBuildDefinitionOptions.GitProviderAccessId = @event.CMSAccessId;
                    createBuildDefinitionOptions.GitProviderAccessSecret = @event.CMSAccessSecret;
                    createBuildDefinitionOptions.GitProviderAccessToken = @event.CMSAccessToken;
                    createBuildDefinitionOptions.GitProviderRepositoryId = item.ServiceExternalId;
                    createBuildDefinitionOptions.GitProviderRepositoryUrl = item.ServiceExternalUrl;
                    createBuildDefinitionOptions.GitProviderRepositoryBranch = @event.CMSType == ConfigurationManagementService.VSTS ? $"refs/heads/{@event.FeatureName.ToLower()}" : @event.FeatureName.ToLower();
                    createBuildDefinitionOptions.ProjectExternalGitEndpoint = @event.ProjectExternalGitEndpoint;
                    createBuildDefinitionOptions.QueueId = queue.QueueId;
                    createBuildDefinitionOptions.QueueName = queue.QueueName;
                    createBuildDefinitionOptions.PoolId = queue.PoolId;
                    createBuildDefinitionOptions.PoolName = queue.PoolName;

                    commitStageId = await _pipelineSpaceManagerService.CreateBuildDefinition(createBuildDefinitionOptions);

                    /* SERVICE-HOOK BUILD ##############################################################################################################################################*/

                    CreateServiceHookOptions createServiceHookBuildOptions = new CreateServiceHookOptions();
                    createServiceHookBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    createServiceHookBuildOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    createServiceHookBuildOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    createServiceHookBuildOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                    createServiceHookBuildOptions.OrganizationId = @event.OrganizationId;
                    createServiceHookBuildOptions.ProjectId = @event.ProjectId;
                    createServiceHookBuildOptions.ServiceId = item.ServiceId;
                    createServiceHookBuildOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                    createServiceHookBuildOptions.EventType = "build";
                    createServiceHookBuildOptions.Definition = createBuildDefinitionOptions.CommitStageName;
                    createServiceHookBuildOptions.Url = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/features/{@event.FeatureId}/services/{item.ServiceId}/events";

                    commitServiceHookId = await _pipelineSpaceManagerService.CreateServiceHook(createServiceHookBuildOptions);
                    
                });


                /* RELEASE-STAGE ##############################################################################################################################################*/
                int? releaseStageId = null;
                Guid? releaseServiceHookId = null;

                await ExecuteProjectFeatureServiceActivity(@event.OrganizationId, @event.ProjectId, @event.FeatureId, item.ServiceId, nameof(DomainConstants.Activities.PSCRRD), async () => {

                    CreateReleaseDefinitionOptions createReleaseDefinitionOptions = new CreateReleaseDefinitionOptions();
                    createReleaseDefinitionOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    createReleaseDefinitionOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    createReleaseDefinitionOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    createReleaseDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                    createReleaseDefinitionOptions.OrganizationName = @event.OrganizationName;
                    createReleaseDefinitionOptions.ProjectName = @event.ProjectName;
                    createReleaseDefinitionOptions.ServiceName = item.ServiceName;
                    createReleaseDefinitionOptions.CommitStageName = createBuildDefinitionOptions.CommitStageName;
                    createReleaseDefinitionOptions.ReleaseStageName = createBuildDefinitionOptions.CommitStageName;
                    createReleaseDefinitionOptions.ReleaseDefinition = releaseDefinition;
                    createReleaseDefinitionOptions.CloudProviderEndpointId = @event.ProjectExternalEndpointId;
                    createReleaseDefinitionOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                    createReleaseDefinitionOptions.CommitStageId = commitStageId;
                    createReleaseDefinitionOptions.QueueId = queue.QueueId;
                    createReleaseDefinitionOptions.CloudProviderAccessId = @event.CPSAccessId;
                    createReleaseDefinitionOptions.CloudProviderAccessSecret = @event.CPSAccessSecret;
                    createReleaseDefinitionOptions.CloudProviderAccessRegion = @event.CPSAccessRegion;
                    createReleaseDefinitionOptions.WorkFeature = @event.FeatureName;
                    createReleaseDefinitionOptions.BaseReleaseStageId = item.ReleaseStageId;

                    releaseStageId = await _pipelineSpaceManagerService.CreateReleaseDefinitionFromBaseDefinition(createReleaseDefinitionOptions);

                    if (releaseStageId.HasValue)
                    {
                        /* SERVICE-HOOK RELEASE ##############################################################################################################################################*/

                        CreateServiceHookOptions createServiceHookReleaseOptions = new CreateServiceHookOptions();
                        createServiceHookReleaseOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                        createServiceHookReleaseOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                        createServiceHookReleaseOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                        createServiceHookReleaseOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                        createServiceHookReleaseOptions.OrganizationId = @event.OrganizationId;
                        createServiceHookReleaseOptions.ProjectId = @event.ProjectId;
                        createServiceHookReleaseOptions.ServiceId = item.ServiceId;
                        createServiceHookReleaseOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                        createServiceHookReleaseOptions.EventType = "release";
                        createServiceHookReleaseOptions.Definition = releaseStageId.Value.ToString();
                        createServiceHookReleaseOptions.Url = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/features/{@event.FeatureId}/services/{item.ServiceId}/events";

                        releaseServiceHookId = await _pipelineSpaceManagerService.CreateServiceHook(createServiceHookReleaseOptions);
                    }

                });

                /* QUEUE - BUILD ##############################################################################################################################################*/

                await ExecuteProjectFeatureServiceActivity(@event.OrganizationId, @event.ProjectId, @event.FeatureId, item.ServiceId, nameof(DomainConstants.Activities.PSQUDB), async () => {

                    QueueBuildOptions queueBuildOptions = new QueueBuildOptions();
                    queueBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    queueBuildOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    queueBuildOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    queueBuildOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

                    queueBuildOptions.ProjectName = @event.ProjectName;
                    queueBuildOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                    queueBuildOptions.QueueId = queue.QueueId;
                    queueBuildOptions.BuildDefinitionId = commitStageId;
                    queueBuildOptions.SourceBranch = @event.CMSType == ConfigurationManagementService.VSTS ? $"refs/heads/{@event.FeatureName.ToLower()}" : @event.FeatureName.ToLower();

                    await _pipelineSpaceManagerService.QueueBuild(queueBuildOptions);

                });

                //Patch Feature Service

                await ExecuteProjectFeatureServiceActivity(@event.OrganizationId, @event.ProjectId, @event.FeatureId, item.ServiceId, nameof(DomainConstants.Activities.PSACBA), async () => {

                    string projectFeatureServicePatchUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/features/{@event.FeatureId}/services/{item.ServiceId}";
                    var projectFeatureServicePatchResponse = await _httpClientWrapperService.PatchAsync(projectFeatureServicePatchUrl,
                        new
                        {
                            CommitStageId = commitStageId,
                            ReleaseStageId = releaseStageId,
                            CommitServiceHookId = commitServiceHookId,
                            ReleaseServiceHookId = releaseServiceHookId,
                            PipelineStatus = PipelineStatus.Building
                        }, InternalAuthCredentials);
                        projectFeatureServicePatchResponse.EnsureSuccessStatusCode();
                });
            }
        }
    }
}
