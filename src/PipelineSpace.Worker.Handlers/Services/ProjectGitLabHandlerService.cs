using Microsoft.Extensions.Options;
using PipelineSpace.Domain.Models;
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
    public class ProjectGitLabHandlerService : IProjectHandlerService
    {
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IHttpClientWrapperService _httpClientWrapperService;

        public ProjectGitLabHandlerService(IOptions<FakeAccountServiceOptions> fakeAccountOptions,
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
                    var projectPatchResponse = await _httpClientWrapperService.PatchAsync(projectPatchUrl, new {
                        ProjectVSTSFakeId = project.Id,
                        ProjectVSTSFakeName = project.Name
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
        }

        public async Task ImportProject(ProjectImportedEvent @event, ApplicationOptions options)
        {
            await this.CreateProject(new ProjectCreatedEvent(@event.CorrelationId) {
                AgentPoolId = @event.AgentPoolId,
                CMSAccessId = @event.CMSAccessId,
                CMSAccessSecret = @event.CMSAccessSecret,
                CMSAccessToken = @event.CMSAccessToken,
                CMSAccountId = @event.CMSAccountId,
                CMSAccountName = @event.CMSAccountName,
                CMSType = @event.CMSType,
                CPSAccessAppId = @event.CPSAccessAppId,
                CPSAccessAppSecret = @event.CPSAccessAppSecret,
                CPSAccessDirectory = @event.CPSAccessDirectory,
                CPSAccessId = @event.CPSAccessId,
                CPSAccessName = @event.CPSAccessName,
                CPSAccessSecret = @event.CPSAccessSecret,
                CPSType = @event.CPSType,
                OrganizationId = @event.OrganizationId,
                ProjectId = @event.ProjectId,
                ProjectName = @event.ProjectName,
                ProjectVSTSFake = @event.ProjectVSTSFake,
                UserId = @event.UserId
            }, options);
        }
    }
}
