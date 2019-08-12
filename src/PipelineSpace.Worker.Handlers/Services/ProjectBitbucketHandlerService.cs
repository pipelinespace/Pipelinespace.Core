using Microsoft.Extensions.Options;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Extensions;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class ProjectBitbucketHandlerService : IProjectHandlerService
    {
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IHttpClientWrapperService _httpClientWrapperService;
        private const string API_VERSION = "2.0";
        private const string API_URL = "https://api.bitbucket.org";

        public ProjectBitbucketHandlerService(IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                              IHttpClientWrapperService httpClientWrapperService)
        {
            _fakeAccountOptions = fakeAccountOptions;
            _httpClientWrapperService = httpClientWrapperService;
        }

        public async Task CreateProject(ProjectCreatedEvent @event, ApplicationOptions applicationOptions)
        {
            /*Create project*/
            var oAuthToken = await _httpClientWrapperService.GetTokenFromClientCredentials($"{applicationOptions.Url}/connect/token", applicationOptions.ClientId, applicationOptions.ClientSecret, applicationOptions.Scope);

            HttpClientWrapperAuthorizationModel internalAuthCredentials = new HttpClientWrapperAuthorizationModel();
            internalAuthCredentials.Schema = "Bearer";
            internalAuthCredentials.Value = oAuthToken.access_token;

            //Post Fake Project
            string projectPostUrl = $"{applicationOptions.Url}/internalapi/organizations/{@event.OrganizationId}/fake/vsts/projects/{@event.ProjectId}";
            var response = await _httpClientWrapperService.PostAsync(projectPostUrl, new { }, internalAuthCredentials);
            response.EnsureSuccessStatusCode();

            /*Wait for project to activate*/
            string accountUrl = $"https://{_fakeAccountOptions.Value.AccountId}.visualstudio.com";
            int index = 0;
            while (true)
            {
                HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
                authCredentials.Schema = "Basic";
                authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _fakeAccountOptions.Value.AccessSecret)));

                string projectUrl = $"{accountUrl}/_apis/projects?api-version={_fakeAccountOptions.Value.ApiVersion}";
                var projectResponse = await _httpClientWrapperService.GetAsync(projectUrl, authCredentials);
                projectResponse.EnsureSuccessStatusCode();

                var projects = await projectResponse.MapTo<CMSVSTSProjectListModel>();

                var project = projects.Items.FirstOrDefault(x => x.Name.Equals(@event.ProjectVSTSFake, StringComparison.InvariantCultureIgnoreCase));
                if (project != null)
                {
                    oAuthToken = await _httpClientWrapperService.GetTokenFromClientCredentials($"{applicationOptions.Url}/connect/token", applicationOptions.ClientId, applicationOptions.ClientSecret, applicationOptions.Scope);

                    internalAuthCredentials = new HttpClientWrapperAuthorizationModel();
                    internalAuthCredentials.Schema = "Bearer";
                    internalAuthCredentials.Value = oAuthToken.access_token;

                    //Patch Project External Id
                    string projectPatchUrl = $"{applicationOptions.Url}/internalapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}";
                    var projectPatchResponse = await _httpClientWrapperService.PatchAsync(projectPatchUrl, new { ProjectVSTSFakeId = project.Id }, internalAuthCredentials);
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
            
            //Begin: check data **********
            @event.CMSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
            @event.CMSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;

            string cmsProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectExternalId : @event.ProjectVSTSFakeId;
            //End: check data **********

            string accountUrl = $"https://{@event.CMSAccountName}.visualstudio.com";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @event.CMSAccessSecret)));

            string projectUrl = $"{accountUrl}/_apis/projects/{cmsProjectId}?api-version={_fakeAccountOptions.Value.ApiVersion}";
            var projectResponse = await _httpClientWrapperService.DeleteAsync(projectUrl, authCredentials);
            projectResponse.EnsureSuccessStatusCode();

            // Delete from Bitbucket

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", @event.CMSAccountId, @event.CMSAccessToken))));
            client.BaseAddress = new Uri(API_URL);

            var response = await client.GetAsync($"/{API_VERSION}/teams?role=admin");

            var teamResult = await response.MapTo<CMSBitBucketTeamListModel>();

            var defaultTeam = teamResult.Teams.FirstOrDefault(c => c.TeamId.Equals(@event.OrganizationExternalId));

            response = await client.DeleteAsync($"/{API_VERSION}/teams/{defaultTeam.UserName}/projects/{@event.ProjectExternalId}");

            response.EnsureSuccessStatusCode();

        }

        public Task ImportProject(ProjectImportedEvent @event, ApplicationOptions options)
        {
            return Task.CompletedTask;
        }
    }
}
