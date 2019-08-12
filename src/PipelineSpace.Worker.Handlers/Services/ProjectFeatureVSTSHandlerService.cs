using Microsoft.Extensions.Options;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class ProjectFeatureVSTSHandlerService : IProjectFeatureHandlerService
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IHttpClientWrapperService _httpClientWrapperService;
        readonly Func<CloudProviderService, ICPSService> _cpsService;

        public ProjectFeatureVSTSHandlerService(IOptions<VSTSServiceOptions> vstsOptions,
                                                IHttpClientWrapperService httpClientWrapperService,
                                                Func<CloudProviderService, ICPSService> cpsService)
        {
            _vstsOptions = vstsOptions;
            _httpClientWrapperService = httpClientWrapperService;
            _cpsService = cpsService;
        }

        public async Task CompleteProjectFeature(ProjectFeatureCompletedEvent @event)
        {
            string accountUrl = $"https://{@event.CMSAccountName}.visualstudio.com";

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

                HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
                authCredentials.Schema = "Basic";
                authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @event.CMSAccessSecret)));

                CMSVSTSPullRequestCreateModel pullRequestModel = new CMSVSTSPullRequestCreateModel();
                pullRequestModel.SourceRefName = $"refs/heads/{@event.FeatureName.ToLower()}";
                pullRequestModel.TargetRefName = "refs/heads/master";
                pullRequestModel.Title = $"feature {@event.FeatureName}";
                pullRequestModel.Description = $"The feature {@event.FeatureName} requests merge operation";

                string pullRequestUrl = $"{accountUrl}/{@event.ProjectExternalId}/_apis/git/repositories/{item.ServiceExternalId}/pullrequests?api-version={_vstsOptions.Value.ApiVersion}";
                var pullRequestResponse = await _httpClientWrapperService.PostAsync(pullRequestUrl, pullRequestModel, authCredentials);
                pullRequestResponse.EnsureSuccessStatusCode();
            }
        }

    }
}
