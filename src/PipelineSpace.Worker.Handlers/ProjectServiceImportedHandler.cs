using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PipelineSpace.Domain;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public class ProjectServiceImportedHandler : BaseHandler, IEventHandler<ProjectServiceImportedEvent>
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly Func<ConfigurationManagementService, IProjectServiceHandlerService> _projectServiceHandlerService;
        readonly IPipelineSpaceManagerService _pipelineSpaceManagerService;
        readonly IHttpClientWrapperService _httpClientWrapperService;

        public ProjectServiceImportedHandler(IOptions<VSTSServiceOptions> vstsOptions,
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

        public async Task Handle(ProjectServiceImportedEvent @event)
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
                    getQueueOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalName : @event.ProjectVSTSFakeName;

                    getQueueOptions.ProjectName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;
                    getQueueOptions.AgentPoolId = @event.AgentPoolId;

                    queue = await _pipelineSpaceManagerService.GetQueue(getQueueOptions);

                });

                /* REPOSITORY ##############################################################################################################################################*/
                string releaseDefinition = string.Empty;

                await ExecuteProjectServiceActivity(@event.OrganizationId, @event.ProjectId, @event.ServiceId, nameof(DomainConstants.Activities.PSCRRP), async () => {

                    CreateRepositoryOptions createRepositoryOptions = new CreateRepositoryOptions();
                    createRepositoryOptions.OrganizationName = @event.OrganizationName;
                    createRepositoryOptions.ProjectName = @event.ProjectName;
                    createRepositoryOptions.ServiceName = @event.ServiceName;
                    createRepositoryOptions.VSTSAccessId = _vstsOptions.Value.AccessId;
                    createRepositoryOptions.VSTSAccessSecret = _vstsOptions.Value.AccessSecret;
                    createRepositoryOptions.VSTSRepositoryTemplateUrl = @event.ServiceTemplateUrl;
                    createRepositoryOptions.VSTSRepositoryTemplatePath = @event.ServiceTemplatePath;

                    createRepositoryOptions.GitProviderType = @event.CMSType;
                    createRepositoryOptions.GitProviderAccessId = @event.CMSAccessId;
                    createRepositoryOptions.GitProviderAccessSecret = @event.CMSAccessSecret;
                    createRepositoryOptions.GitProviderAccessToken = @event.CMSAccessToken;
                    createRepositoryOptions.GitProviderRepositoryUrl = @event.ServiceExternalUrl;
                    createRepositoryOptions.Branch = @event.BranchName;
                    createRepositoryOptions.TemplateAccess = @event.TemplateAccess;
                    createRepositoryOptions.NeedCredentials = @event.NeedCredentials;
                    createRepositoryOptions.RepositoryCMSType = @event.RepositoryCMSType;
                    createRepositoryOptions.RepositoryAccessId = @event.RepositoryAccessId;
                    createRepositoryOptions.RepositoryAccessSecret = @event.RepositoryAccessSecret;
                    createRepositoryOptions.RepositoryAccessToken = @event.RepositoryAccessToken;

                    releaseDefinition = await _pipelineSpaceManagerService.GetReleaseDefinition(createRepositoryOptions, @event.BuildDefinitionYML);

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
                    createBuildDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalName : @event.ProjectVSTSFakeName;
                    
                    createBuildDefinitionOptions.ProjectName = @event.ProjectName;
                    createBuildDefinitionOptions.ServiceName = @event.ServiceName;
                    createBuildDefinitionOptions.CommitStageName = @event.ServiceName;
                    createBuildDefinitionOptions.GitProviderType = @event.CMSType;
                    createBuildDefinitionOptions.GitProviderAccountId = @event.CMSAccountId;
                    createBuildDefinitionOptions.GitProviderAccountName = @event.CMSAccountName;
                    createBuildDefinitionOptions.GitProviderAccessId = @event.CMSAccessId;
                    createBuildDefinitionOptions.GitProviderAccessSecret = @event.CMSAccessSecret;
                    createBuildDefinitionOptions.GitProviderAccessToken = @event.CMSAccessToken;
                    createBuildDefinitionOptions.GitProviderRepositoryId = @event.ServiceExternalId;
                    createBuildDefinitionOptions.GitProviderRepositoryUrl = @event.ServiceExternalUrl;
                    createBuildDefinitionOptions.GitProviderRepositoryBranch = @event.BranchName;
                    createBuildDefinitionOptions.ProjectExternalGitEndpoint = @event.ProjectExternalGitEndpoint;
                    createBuildDefinitionOptions.QueueId = queue.QueueId;
                    createBuildDefinitionOptions.QueueName = queue.QueueName;
                    createBuildDefinitionOptions.PoolId = queue.PoolId;
                    createBuildDefinitionOptions.PoolName = queue.PoolName;
                    createBuildDefinitionOptions.YamlFilename = ".pipelinespace/build.definition.yml";
                    commitStageId = await _pipelineSpaceManagerService.CreateBuildDefinition(createBuildDefinitionOptions);

                    /* SERVICE-HOOK BUILD ##############################################################################################################################################*/
                    if (@event.CMSType == ConfigurationManagementService.VSTS)
                    {
                        CreateServiceHookOptions createServiceHookCodeOptions = new CreateServiceHookOptions();
                        createServiceHookCodeOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                        createServiceHookCodeOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                        createServiceHookCodeOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                        createServiceHookCodeOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalName : @event.ProjectVSTSFakeName;

                        createServiceHookCodeOptions.OrganizationId = @event.OrganizationId;
                        createServiceHookCodeOptions.ProjectId = @event.ProjectId;
                        createServiceHookCodeOptions.ServiceId = @event.ServiceId;
                        createServiceHookCodeOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                        createServiceHookCodeOptions.EventType = "code";
                        createServiceHookCodeOptions.Definition = createBuildDefinitionOptions.CommitStageName;
                        createServiceHookCodeOptions.Url = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/{@event.ServiceId}/events";

                        createServiceHookCodeOptions.Repository = @event.ServiceExternalId;
                        createServiceHookCodeOptions.Branch = @event.BranchName;

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
                
                /* QUEUE - BUILD ########################################################`######################################################################################*/

                await ExecuteProjectServiceActivity(@event.OrganizationId, @event.ProjectId, @event.ServiceId, nameof(DomainConstants.Activities.PSQUDB), async () => {

                    QueueBuildOptions queueBuildOptions = new QueueBuildOptions();
                    queueBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    queueBuildOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
                    queueBuildOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
                    queueBuildOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalName : @event.ProjectVSTSFakeName;

                    queueBuildOptions.ProjectName = @event.ProjectExternalName;
                    queueBuildOptions.ProjectExternalId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
                    queueBuildOptions.QueueId = queue.QueueId;
                    queueBuildOptions.BuildDefinitionId = commitStageId;
                    queueBuildOptions.SourceBranch = @event.BranchName;

                    await _pipelineSpaceManagerService.QueueBuild(queueBuildOptions);

                    string eventUrl = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/{@event.ServiceId}/events";
                    await _httpClientWrapperService.PostAsync(eventUrl, new
                    {
                        SubscriptionId = Guid.NewGuid(),
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
                                //ReleaseStageId = releaseStageId,
                                CommitServiceHookId = commitServiceHookId,
                                //ReleaseServiceHookId = releaseServiceHookId,
                                CodeServiceHookId = codeServiceHookId,
                                //ReleaseStartedServiceHookId = releaseStartedServiceHookId,
                                //ReleasePendingApprovalServiceHookId = releasePendingApprovalServiceHookId,
                                //ReleaseCompletedApprovalServiceHookId = releaseCompletedApprovalServiceHookId,
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
