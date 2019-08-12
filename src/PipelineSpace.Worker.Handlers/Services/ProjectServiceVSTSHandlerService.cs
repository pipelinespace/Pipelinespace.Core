using Microsoft.Extensions.Options;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class ProjectServiceVSTSHandlerService : IProjectServiceHandlerService
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IHttpClientWrapperService _httpClientWrapperService;

        public ProjectServiceVSTSHandlerService(IOptions<VSTSServiceOptions> vstsOptions,
                                                IHttpClientWrapperService httpClientWrapperService)
        {
            _vstsOptions = vstsOptions;
            _httpClientWrapperService = httpClientWrapperService;
        }

        public async Task DeleteRepository(ProjectServiceDeletedEvent @event)
        {
            string accountUrl = $"https://{@event.CMSAccountName}.visualstudio.com";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @event.CMSAccessSecret)));

            string projectUrl = $"{accountUrl}/{@event.ProjectExternalId}/_apis/git/repositories/{@event.ProjectServiceExternalId}?api-version={_vstsOptions.Value.ApiVersion}";
            var projectResponse = await _httpClientWrapperService.DeleteAsync(projectUrl, authCredentials);
            projectResponse.EnsureSuccessStatusCode();
        }
    }
}
