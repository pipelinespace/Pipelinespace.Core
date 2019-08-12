using Microsoft.Extensions.Options;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class ProjectFeatureGitHubHandlerService : IProjectFeatureHandlerService
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IHttpClientWrapperService _httpClientWrapperService;
        readonly Func<CloudProviderService, ICPSService> _cpsService;

        public ProjectFeatureGitHubHandlerService(IOptions<VSTSServiceOptions> vstsOptions,
            IHttpClientWrapperService httpClientWrapperService,
                                                Func<CloudProviderService, ICPSService> cpsService)
        {
            _vstsOptions = vstsOptions;
            _httpClientWrapperService = httpClientWrapperService;
            _cpsService = cpsService;
        }

        private const string API_Accept = "application/vnd.github.shadow-cat-preview";
        private const string API_UserAgent = "HttpClientFactory-Sample";

        public Dictionary<string, string> Headers
        {
            get
            {

                var dic = new Dictionary<string, string>();
                dic.Add("Accept", API_Accept);
                dic.Add("User-Agent", API_UserAgent);

                return dic;

            }
        }

        public async Task CompleteProjectFeature(ProjectFeatureCompletedEvent @event)
        {
            foreach (var item in @event.Services)
            {
                /*Delete Infrastructure*/
                if (@event.DeleteInfrastructure)
                {
                    CPSAuthModel authModel = new CPSAuthModel();
                    authModel.AccessId = @event.CPSAccessId;
                    authModel.AccessName = @event.CPSAccessName;
                    authModel.AccessSecret = @event.CPSAccessSecret;
                    authModel.AccessRegion = @event.CPSAccessRegion;
                    authModel.AccessAppId = @event.CPSAccessAppId;
                    authModel.AccessAppSecret = @event.CPSAccessAppSecret;
                    authModel.AccessDirectory = @event.CPSAccessDirectory;

                    string cloudServiceName = $"{@event.OrganizationName}{@event.ProjectName}{item.ServiceName}development{@event.FeatureName}".ToLower();
                    await _cpsService(@event.CPSType).DeleteService(cloudServiceName, authModel);
                }

                CMSGitHubPullRequestCreateModel pullRequestModel = new CMSGitHubPullRequestCreateModel();
                pullRequestModel.SourceRefName = $"{@event.FeatureName.ToLower()}";
                pullRequestModel.TargetRefName = "master";
                pullRequestModel.Title = $"feature {@event.FeatureName}";
                pullRequestModel.Description = $"The feature {@event.FeatureName} requests merge operation";

                HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
                authCredentials.Schema = "Bearer";
                authCredentials.Value = @event.CMSAccessToken;

                string pullRequestUrl = $"https://api.github.com/repos/{@event.CMSAccountName}/{item.ServiceName}/pulls";
                var pullRequestResponse = await _httpClientWrapperService.PostAsync(pullRequestUrl, pullRequestModel, authCredentials, headers: Headers);
                pullRequestResponse.EnsureSuccessStatusCode();
            }
        }

    }
}
