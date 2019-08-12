using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PipelineSpace.Domain.Models.Enums;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public abstract class BaseHandler
    {
        readonly IHttpClientWrapperService _httpClientWrapperService;
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly IRealtimeService _realtimeService;

        protected string userId;
        
        public BaseHandler(IHttpClientWrapperService httpClientWrapperService,
                           IOptions<ApplicationOptions> applicationOptions,
                           IRealtimeService realtimeService)
        {
            _httpClientWrapperService = httpClientWrapperService;
            _applicationOptions = applicationOptions;
            this._realtimeService = realtimeService;

            var oAuthToken = _httpClientWrapperService.GetTokenFromClientCredentials($"{_applicationOptions.Value.Url}/connect/token", _applicationOptions.Value.ClientId, _applicationOptions.Value.ClientSecret, _applicationOptions.Value.Scope).GetAwaiter().GetResult();

            InternalAuthCredentials = new HttpClientWrapperAuthorizationModel();
            InternalAuthCredentials.Schema = "Bearer";
            InternalAuthCredentials.Value = oAuthToken.access_token;
        }
        
        public async Task ExecuteProjectActivity(Guid organizationId, Guid projectId , string code, Func<Task> action)
        {
            string activityUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{organizationId}/projects/{projectId}/activities";

            try
            {
                var log = "In Progress";
                await _httpClientWrapperService.PutAsync($"{activityUrl}/{code}", new { ActivityStatus = ActivityStatus.InProgress, Log = log }, InternalAuthCredentials);
                await this._realtimeService.NotifyProjectStatus(userId, organizationId, projectId, (string)typeof(Domain.DomainConstants.Activities).GetField(code).GetValue(null), log, this.InternalAuthCredentials);

                Thread.Sleep(TimeSpan.FromSeconds(3));

                await action.Invoke();

                log = "Succeed";
                await _httpClientWrapperService.PutAsync($"{activityUrl}/{code}", new { ActivityStatus = ActivityStatus.Succeed, Log = log }, InternalAuthCredentials);
                await this._realtimeService.NotifyProjectStatus(userId, organizationId, projectId, (string)typeof(Domain.DomainConstants.Activities).GetField(code).GetValue(null), log, this.InternalAuthCredentials);

                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                 await _httpClientWrapperService.PutAsync($"{activityUrl}/{code}", new { ActivityStatus = ActivityStatus.Failed, Log = ex.ToString() }, InternalAuthCredentials);
                await this._realtimeService.NotifyProjectStatus(userId, organizationId, projectId, (string)typeof(Domain.DomainConstants.Activities).GetField(code).GetValue(null), "Failed", this.InternalAuthCredentials);
            }
        }

        public async Task ExecuteProjectServiceActivity(Guid organizationId, Guid projectId, Guid serviceId, string code, Func<Task> action)
        {
            string activityUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{organizationId}/projects/{projectId}/services/{serviceId}/activities";

            try
            {
                var log = "In Progress";
                await _httpClientWrapperService.PutAsync($"{activityUrl}/{code}", new { ActivityStatus = ActivityStatus.InProgress, Log = log }, InternalAuthCredentials);
                await this._realtimeService.NotifyProjectPipeStatus(userId, organizationId, projectId, serviceId, (string)typeof(Domain.DomainConstants.Activities).GetField(code).GetValue(null), log, this.InternalAuthCredentials);

                Thread.Sleep(TimeSpan.FromSeconds(2));

                await action.Invoke();

                log = "Succeed";
                await _httpClientWrapperService.PutAsync($"{activityUrl}/{code}", new { ActivityStatus = ActivityStatus.Succeed, Log = log }, InternalAuthCredentials);
                await this._realtimeService.NotifyProjectPipeStatus(userId, organizationId, projectId, serviceId, (string)typeof(Domain.DomainConstants.Activities).GetField(code).GetValue(null), log, this.InternalAuthCredentials);

                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                await _httpClientWrapperService.PutAsync($"{activityUrl}/{code}", new { ActivityStatus = ActivityStatus.Failed, Log = ex.ToString() }, InternalAuthCredentials);
                await this._realtimeService.NotifyProjectPipeStatus(userId, organizationId, projectId, serviceId, (string)typeof(Domain.DomainConstants.Activities).GetField(code).GetValue(null), "Failed", this.InternalAuthCredentials);
            }
        }

        public async Task ExecuteProjectFeatureServiceActivity(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, string code, Func<Task> action)
        {
            string activityUrl = $"{_applicationOptions.Value.Url}/internalapi/organizations/{organizationId}/projects/{projectId}/features/{featureId}/services/{serviceId}/activities";

            try
            {
                var log = "In Progress";
                await _httpClientWrapperService.PutAsync($"{activityUrl}/{code}", new { ActivityStatus = ActivityStatus.InProgress, Log = log }, InternalAuthCredentials);
                await this._realtimeService.NotifyFeaturePipeStatus(userId, organizationId, projectId, featureId, serviceId, (string)typeof(Domain.DomainConstants.Activities).GetField(code).GetValue(null), log, this.InternalAuthCredentials);

                Thread.Sleep(TimeSpan.FromSeconds(2));

                await action.Invoke();

                log = "Succeed";
                await _httpClientWrapperService.PutAsync($"{activityUrl}/{code}", new { ActivityStatus = ActivityStatus.Succeed, Log = log }, InternalAuthCredentials);
                await this._realtimeService.NotifyFeaturePipeStatus(userId, organizationId, projectId, featureId, serviceId, (string)typeof(Domain.DomainConstants.Activities).GetField(code).GetValue(null), log, this.InternalAuthCredentials);

                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                await _httpClientWrapperService.PutAsync($"{activityUrl}/{code}", new { ActivityStatus = ActivityStatus.Failed, Log = ex.ToString() }, InternalAuthCredentials);
                await this._realtimeService.NotifyFeaturePipeStatus(userId, organizationId, projectId, featureId, serviceId, (string)typeof(Domain.DomainConstants.Activities).GetField(code).GetValue(null), "Failed", this.InternalAuthCredentials);
            }
        }

        public HttpClientWrapperAuthorizationModel InternalAuthCredentials { get; set; }
    }
}
