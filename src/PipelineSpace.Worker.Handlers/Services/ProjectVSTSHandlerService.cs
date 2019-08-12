using Microsoft.Extensions.Options;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Extensions;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class ProjectVSTSHandlerService : IProjectHandlerService
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IHttpClientWrapperService _httpClientWrapperService;
        public ProjectVSTSHandlerService(IOptions<VSTSServiceOptions> vstsOptions,
                                         IHttpClientWrapperService httpClientWrapperService)
        {
            _vstsOptions = vstsOptions;
            _httpClientWrapperService = httpClientWrapperService;
        }

        public async Task CreateProject(ProjectCreatedEvent @event, ApplicationOptions applicationOptions)
        {
            string accountUrl = $"https://{@event.CMSAccountName}.visualstudio.com";
            
            int index = 0;
            while (true)
            {
                HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
                authCredentials.Schema = "Basic";
                authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @event.CMSAccessSecret)));

                string projectUrl = $"{accountUrl}/_apis/projects?api-version={_vstsOptions.Value.ApiVersion}";
                var projectResponse = await _httpClientWrapperService.GetAsync(projectUrl, authCredentials);
                projectResponse.EnsureSuccessStatusCode();

                var projects = await projectResponse.MapTo<CMSVSTSProjectListModel>();

                var project = projects.Items.FirstOrDefault(x => x.Name.Equals(@event.ProjectName, StringComparison.InvariantCultureIgnoreCase));
                if (project != null)
                {
                    var oAuthToken = await _httpClientWrapperService.GetTokenFromClientCredentials($"{applicationOptions.Url}/connect/token", applicationOptions.ClientId, applicationOptions.ClientSecret, applicationOptions.Scope);

                    HttpClientWrapperAuthorizationModel internalAuthCredentials = new HttpClientWrapperAuthorizationModel();
                    internalAuthCredentials.Schema = "Bearer";
                    internalAuthCredentials.Value = oAuthToken.access_token;

                    //Patch Project External Id
                    string projectPatchUrl = $"{applicationOptions.Url}/internalapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}";
                    var projectPatchResponse = await _httpClientWrapperService.PatchAsync(projectPatchUrl, new {
                        ProjectExternalId = project.Id,
                        ProjectExternalName = project.Name
                    }, internalAuthCredentials);
                    projectPatchResponse.EnsureSuccessStatusCode();

                    break;
                }
                else
                {
                    index++;
                    //this means after 20 seconds it didn't create the project
                    if (index == 6)
                    {
                        throw new Exception($"After 20 seconds it could be possible to retreive the external project id: Ref: {@event.ProjectId} - {@event.ProjectName}");
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }

            }
        }

        public async Task DeleteProject(ProjectDeletedEvent @event)
        {
            string accountUrl = $"https://{@event.CMSAccountName}.visualstudio.com";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @event.CMSAccessSecret)));

            string projectUrl = $"{accountUrl}/_apis/projects/{@event.ProjectExternalId}?api-version={_vstsOptions.Value.ApiVersion}";
            var projectResponse = await _httpClientWrapperService.DeleteAsync(projectUrl, authCredentials);
            projectResponse.EnsureSuccessStatusCode();
        }

        public Task ImportProject(ProjectImportedEvent @event, ApplicationOptions options)
        {
            return Task.CompletedTask;
        }
    }
}
