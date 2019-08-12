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
    public class ProjectServiceTemplateCreatedHandler : IEventHandler<ProjectServiceTemplateCreatedEvent>
    {
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IHttpClientWrapperService _httpClientWrapperService;
        readonly IPipelineSpaceManagerService _pipelineSpaceManagerService;
        
        public ProjectServiceTemplateCreatedHandler(IOptions<ApplicationOptions> applicationOptions,
                                                    IOptions<VSTSServiceOptions> vstsOptions,
                                                    IHttpClientWrapperService httpClientWrapperService,
                                                    IPipelineSpaceManagerService pipelineSpaceManagerService)
        {
            _applicationOptions = applicationOptions;
            _vstsOptions = vstsOptions;
            _httpClientWrapperService = httpClientWrapperService;
            _pipelineSpaceManagerService = pipelineSpaceManagerService;
        }

        public async Task Handle(ProjectServiceTemplateCreatedEvent @event)
        {
            CreateOrganizationRepositoryOptions createOrganizationRepositoryOptions = new CreateOrganizationRepositoryOptions();
            createOrganizationRepositoryOptions.VSTSAccessId = _vstsOptions.Value.AccessId;
            createOrganizationRepositoryOptions.VSTSAccessSecret = _vstsOptions.Value.AccessSecret;
            createOrganizationRepositoryOptions.VSTSRepositoryTemplateUrl = @event.SourceTemplateUrl;
            
            createOrganizationRepositoryOptions.TemplateAccess = @event.TemplateAccess;
            createOrganizationRepositoryOptions.NeedCredentials = @event.NeedCredentials;
            createOrganizationRepositoryOptions.RepositoryCMSType = @event.RepositoryCMSType;
            createOrganizationRepositoryOptions.RepositoryAccessId = @event.RepositoryAccessId;
            createOrganizationRepositoryOptions.RepositoryAccessSecret = @event.RepositoryAccessSecret;
            createOrganizationRepositoryOptions.RepositoryAccessToken = @event.RepositoryAccessToken;
            createOrganizationRepositoryOptions.RepositoryUrl = @event.RepositoryUrl;
            createOrganizationRepositoryOptions.Branch = @"refs/heads/master";

            await _pipelineSpaceManagerService.CreateOrganizationRepository(createOrganizationRepositoryOptions);

            var oAuthToken = _httpClientWrapperService.GetTokenFromClientCredentials($"{_applicationOptions.Value.Url}/connect/token", _applicationOptions.Value.ClientId, _applicationOptions.Value.ClientSecret, _applicationOptions.Value.Scope).GetAwaiter().GetResult();

            var InternalAuthCredentials = new HttpClientWrapperAuthorizationModel();
            InternalAuthCredentials.Schema = "Bearer";
            InternalAuthCredentials.Value = oAuthToken.access_token;

            string projectActivateUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{@event.OrganizationId}/servicetemplates/{@event.ProjectServiceTemplateId}/activate";
            var projectActivateResponse = await _httpClientWrapperService.PatchAsync(projectActivateUrl, new { }, InternalAuthCredentials);
            projectActivateResponse.EnsureSuccessStatusCode();
        }
    }
}
