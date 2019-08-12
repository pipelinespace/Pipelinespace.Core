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
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public class ProjectImportedHandler : BaseHandler, IEventHandler<ProjectImportedEvent>
    {
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _vstsFakeOptions;
        readonly Func<ConfigurationManagementService, IProjectHandlerService> _projectHandlerService;
        readonly IHttpClientWrapperService _httpClientWrapperService;

        public ProjectImportedHandler(IOptions<ApplicationOptions> applicationOptions,
                                     IOptions<VSTSServiceOptions> vstsOptions,
                                     IOptions<FakeAccountServiceOptions> vstsFakeOptions,
                                     Func<ConfigurationManagementService, IProjectHandlerService> projectHandlerService,
                                     IHttpClientWrapperService httpClientWrapperService,
                                     IRealtimeService realtimeService) : base(httpClientWrapperService, applicationOptions, realtimeService)
        {
            _applicationOptions = applicationOptions;
            _vstsOptions = vstsOptions;
            _vstsFakeOptions = vstsFakeOptions;
            _projectHandlerService = projectHandlerService;
            _httpClientWrapperService = httpClientWrapperService;
        }

        public async Task Handle(ProjectImportedEvent @event)
        {
            this.userId = @event.UserId;
            string accountUrl = string.Empty;
            string accountSecret = string.Empty;
            string accountProject = string.Empty;

            if (@event.CMSType == ConfigurationManagementService.VSTS)
            {
                accountUrl = $"https://{@event.CMSAccountName}.visualstudio.com";
                accountSecret = @event.CMSAccessSecret;
                accountProject = @event.ProjectExternalName;
            }
            else
            {
                accountUrl = $"https://{_vstsFakeOptions.Value.AccountId}.visualstudio.com";
                accountSecret = _vstsFakeOptions.Value.AccessSecret;
                accountProject = @event.ProjectVSTSFake;
            }

            await ExecuteProjectActivity(@event.OrganizationId, @event.ProjectId, nameof(DomainConstants.Activities.PRCRBA), async () => 
            {
                await _projectHandlerService(@event.CMSType).ImportProject(@event, _applicationOptions.Value);
            });

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", accountSecret)));

            //Extesions
            if (@event.CPSType == CloudProviderService.AWS)
            {
                if(@event.CMSType == ConfigurationManagementService.VSTS)
                {
                    await ExecuteProjectActivity(@event.OrganizationId, @event.ProjectId, nameof(DomainConstants.Activities.PREXBA), async () =>
                    {
                        string extensionUrl = $"https://extmgmt.dev.azure.com/{@event.CMSAccountName}/_apis/extensionmanagement/installedextensionsbyname/AmazonWebServices/aws-vsts-tools?api-version=4.1-preview.1";
                        var extensionResponse = await _httpClientWrapperService.GetAsync(extensionUrl, authCredentials);
                        if (extensionResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            extensionResponse = await _httpClientWrapperService.PostAsync(extensionUrl, new { }, authCredentials);
                            extensionResponse.EnsureSuccessStatusCode();
                        }
                    });
                }

                if (@event.CMSType == ConfigurationManagementService.GitHub)
                {
                    await ExecuteProjectActivity(@event.OrganizationId, @event.ProjectId, nameof(DomainConstants.Activities.PREXBA), async () =>
                    {
                        await Task.CompletedTask;
                    });
                }
            }

            if (@event.CPSType == CloudProviderService.Azure)
            {
                await ExecuteProjectActivity(@event.OrganizationId, @event.ProjectId, nameof(DomainConstants.Activities.PREXBO), async () =>
                {
                    string extensionUrl = $"https://extmgmt.dev.azure.com/{@event.CMSAccountName}/_apis/extensionmanagement/installedextensionsbyname/keesschollaart/arm-outputs?api-version=4.1-preview.1";
                    var extensionResponse = await _httpClientWrapperService.GetAsync(extensionUrl, authCredentials);
                    if (extensionResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        extensionResponse = await _httpClientWrapperService.PostAsync(extensionUrl, new { }, authCredentials);
                        extensionResponse.EnsureSuccessStatusCode();
                    }
                });
            }


            //Create Service Endpoint - Cloud
            //######################################################################################################################################################
            Guid serviceEndpointId = Guid.NewGuid();
            CMSVSTSServiceEndpointModel service = null;

            if (@event.CPSType == CloudProviderService.AWS)
            {
                service = CMSVSTSServiceEndpointModel.Factory.CreateAWSService(serviceEndpointId, $"{@event.ProjectName}AWS", @event.CPSAccessId, @event.CPSAccessSecret);
            }

            if (@event.CPSType == CloudProviderService.Azure)
            {
                service = CMSVSTSServiceEndpointModel.Factory.CreateAzureService(serviceEndpointId, $"{@event.ProjectName}Az", @event.CPSAccessId, @event.CPSAccessName, @event.CPSAccessAppId, @event.CPSAccessAppSecret, @event.CPSAccessDirectory);
            }

            string serviceEndpointUrl = string.Empty;
            HttpResponseMessage serviceEndpointResponse = null;
            string projectPatchUrl = string.Empty;
            HttpResponseMessage projectPatchResponse = null;
            CMSVSTSServiceEndpointModel serviceEndpoint = null;

            await ExecuteProjectActivity(@event.OrganizationId, @event.ProjectId, nameof(DomainConstants.Activities.PRCLEP), async () =>
            {
                serviceEndpointUrl = $"{accountUrl}/{accountProject}/_apis/serviceendpoint/endpoints?api-version={_vstsOptions.Value.ApiVersion}-preview";
                serviceEndpointResponse = await _httpClientWrapperService.PostAsync(serviceEndpointUrl, service, authCredentials);
                serviceEndpointResponse.EnsureSuccessStatusCode();

                serviceEndpoint = await serviceEndpointResponse.MapTo<CMSVSTSServiceEndpointModel>();

                //Patch External Endpoint Id
                projectPatchUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}";
                projectPatchResponse = await _httpClientWrapperService.PatchAsync(projectPatchUrl, new { ProjectExternalEndpointId = serviceEndpoint.Id.ToString() }, InternalAuthCredentials);
                projectPatchResponse.EnsureSuccessStatusCode();
            });

            //Create Service Endpoint - Git
            //######################################################################################################################################################

            await ExecuteProjectActivity(@event.OrganizationId, @event.ProjectId, nameof(DomainConstants.Activities.PRGTEP), async () =>
            {
                if (@event.CMSType == ConfigurationManagementService.GitHub)
                {
                    var serviceConnectionModel = new
                    {
                        id = Guid.NewGuid(),
                        name = $"{@event.ProjectName}GitHub",
                        description = "PipelineSpaceGitHub",
                        authorization = new
                        {
                            parameters = new
                            {
                                accessToken = @event.CMSAccessToken
                            },
                            scheme = "PersonalAccessToken",
                        },
                        type = "github",
                        url = "https://api.github.com"
                    };

                    serviceEndpointUrl = $"{accountUrl}/{accountProject}/_apis/serviceendpoint/endpoints?api-version={_vstsOptions.Value.ApiVersion}-preview";
                    serviceEndpointResponse = await _httpClientWrapperService.PostAsync(serviceEndpointUrl, serviceConnectionModel, authCredentials);
                    serviceEndpointResponse.EnsureSuccessStatusCode();

                    serviceEndpoint = await serviceEndpointResponse.MapTo<CMSVSTSServiceEndpointModel>();

                    //Patch External Git Endpoint Id
                    projectPatchUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}";
                    projectPatchResponse = await _httpClientWrapperService.PatchAsync(projectPatchUrl, new { ProjectExternalGitEndpoint = serviceEndpoint.Id.ToString() }, InternalAuthCredentials);
                    projectPatchResponse.EnsureSuccessStatusCode();
                }

                if (@event.CMSType == ConfigurationManagementService.Bitbucket)
                {
                    // Create External Connection.
                    var serviceConnectionModel = new
                    {
                        id = Guid.NewGuid(),
                        name = $"{@event.ProjectName}BitBucket",
                        description = "PipelineSpaceBitBucket",
                        authorization = new
                        {
                            parameters = new
                            {
                                username = @event.CMSAccessId,
                                password = @event.CMSAccessSecret
                            },
                            scheme = "UsernamePassword",
                        },
                        type = "bitbucket",
                        url = "https://api.bitbucket.org"
                    };

                    serviceEndpointUrl = $"{accountUrl}/{accountProject}/_apis/serviceendpoint/endpoints?api-version={_vstsOptions.Value.ApiVersion}-preview";
                    serviceEndpointResponse = await _httpClientWrapperService.PostAsync(serviceEndpointUrl, serviceConnectionModel, authCredentials);
                    serviceEndpointResponse.EnsureSuccessStatusCode();

                    serviceEndpoint = await serviceEndpointResponse.MapTo<CMSVSTSServiceEndpointModel>();

                    //Patch External Git Endpoint Id
                    projectPatchUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}";
                    projectPatchResponse = await _httpClientWrapperService.PatchAsync(projectPatchUrl, new { ProjectExternalGitEndpoint = serviceEndpoint.Id.ToString() }, InternalAuthCredentials);
                    projectPatchResponse.EnsureSuccessStatusCode();
                }
            });

            //Activate Project External Id
            await ExecuteProjectActivity(@event.OrganizationId, @event.ProjectId, nameof(DomainConstants.Activities.PRACBA), async () =>
            {
                string projectActivateUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/activate";
                var projectActivateResponse = await _httpClientWrapperService.PatchAsync(projectActivateUrl, new { }, InternalAuthCredentials);
                projectActivateResponse.EnsureSuccessStatusCode();
            });

            //Import Services From Git
            if (@event.ProjectRepository != null && @event.ProjectRepository.Repositories != null && @event.ProjectRepository.Repositories.Count > 0)
            {
                await ExecuteProjectActivity(@event.OrganizationId, @event.ProjectId, nameof(DomainConstants.Activities.PRSTIR), async () =>
                {
                    foreach (var item in @event.ProjectRepository.Repositories)
                    {
                        string projecServicePostUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/services/imports";
                        var projecServicePostResponse = await _httpClientWrapperService.PostAsync(projecServicePostUrl,
                            new
                            {
                                ServiceExternalId = item.Id,
                                ServiceExternalUrl = item.Link,
                                ServiceExternalName = item.ExternalName,
                                Name = item.Name,
                                Description = item.Name,
                                BranchName = item.BranchName,
                                UserId = @event.UserId,
                                OrganizationCMSId = @event.OrganizationCMSId,
                                BuildDefinitionYML = @event.BuildDefinitionYML,
                                ProjectServiceTemplateId = @event.ProjectServiceTemplateId,
                                ProjectExternalId = @event.ProjectExternalId,
                                ProjectExternalName = @event.ProjectExternalName
                            }, InternalAuthCredentials);
                        projecServicePostResponse.EnsureSuccessStatusCode();
                    }
                });
            }
        }
    }
}
