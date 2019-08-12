using Microsoft.Extensions.Options;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class RealtimeService : IRealtimeService
    {
        readonly IHttpClientWrapperService _httpClientWrapperService;
        IOptions<ApplicationOptions> _applicationOptions;

        public RealtimeService(IHttpClientWrapperService httpClientWrapperService, IOptions<ApplicationOptions> applicationOptions)
        {
            this._httpClientWrapperService = httpClientWrapperService;
            this._applicationOptions = applicationOptions;
        }

        public async Task NotifyFeaturePipeStatus(string userId, Guid organizationId, Guid projectId, Guid featureId, Guid projectServiceId, string activityName, string status, HttpClientWrapperAuthorizationModel internalAuthCredentials)
        {
            string notificationUrl = $"{_applicationOptions.Value.Url}/internalapi/realtime/organizations/{organizationId}/projects/{projectId}/features/{featureId}/services/{projectServiceId}/activities";

            var response = await _httpClientWrapperService.PostAsync(notificationUrl, new
            {
                userId = userId,
                activityName = activityName,
                status = status
            }, internalAuthCredentials);

            response.EnsureSuccessStatusCode();
        }

        public async Task NotifyProjectPipeStatus(string userId, Guid organizationId, Guid projectId, Guid projectServiceId, string activityName, string status, HttpClientWrapperAuthorizationModel internalAuthCredentials)
        {
            string notificationUrl = $"{_applicationOptions.Value.Url}/internalapi/realtime/organizations/{organizationId}/projects/{projectId}/services/{projectServiceId}/activities";

            var response = await _httpClientWrapperService.PostAsync(notificationUrl, new
            {
                userId = userId,
                activityName = activityName,
                status = status
            }, internalAuthCredentials);

            response.EnsureSuccessStatusCode();
        }

        public async Task NotifyProjectStatus(string userId, Guid organizationId, Guid projectId, string activityName, string status, HttpClientWrapperAuthorizationModel internalAuthCredentials)
        {
            string notificationUrl = $"{_applicationOptions.Value.Url}/internalapi/realtime/organizations/{organizationId}/projects/{projectId}/activities";

            var response = await _httpClientWrapperService.PostAsync(notificationUrl, new
            {
                userId = userId,
                activityName = activityName,
                status = status
            }, internalAuthCredentials);

            response.EnsureSuccessStatusCode();
        }
        
    }
}
