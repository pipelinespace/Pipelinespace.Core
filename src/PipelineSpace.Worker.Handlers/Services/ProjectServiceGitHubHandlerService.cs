using Microsoft.Extensions.Options;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class ProjectServiceGitHubHandlerService : IProjectServiceHandlerService
    {
        private const string API_Accept = "application/vnd.github.mercy-preview+json";
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

        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IHttpClientWrapperService _httpClientWrapperService;

        public ProjectServiceGitHubHandlerService(IOptions<VSTSServiceOptions> vstsOptions,
                                                  IHttpClientWrapperService httpClientWrapperService)
        {
            _vstsOptions = vstsOptions;
            _httpClientWrapperService = httpClientWrapperService;
        }

        public async Task DeleteRepository(ProjectServiceDeletedEvent @event)
        {
            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Bearer";
            authCredentials.Value = @event.CMSAccessToken;

            string deleteRepositoryUrl = $"https://api.github.com/repos/{@event.CMSAccountName}/{@event.ServiceName}";
            var deleteRepositoryResponse = await _httpClientWrapperService.DeleteAsync(deleteRepositoryUrl, authCredentials, Headers);
            deleteRepositoryResponse.EnsureSuccessStatusCode();
        }
    }
}
