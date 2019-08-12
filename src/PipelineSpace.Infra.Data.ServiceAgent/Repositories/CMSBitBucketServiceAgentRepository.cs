using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PipelineSpace.Infra.Data.ServiceAgent.Extentions;
using PipelineSpace.Infra.Data.ServiceAgent.Models.BitBucket;
using System.Linq;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CMSBitBucketServiceAgentRepository : ICMSService
    {
        private const string API_VERSION = "2.0";
        
        readonly IHttpProxyService _httpProxyService;
        public CMSBitBucketServiceAgentRepository(IHttpProxyService httpProxyService)
        {
            _httpProxyService = httpProxyService;

        }

        public async Task<CMSProjectCreateResultModel> CreateProject(CMSAuthCredentialModel authCredential, CMSProjectCreateModel model)
        {
            CMSProjectCreateResultModel result = new CMSProjectCreateResultModel();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authCredential.AccessToken);
            client.BaseAddress = new Uri(authCredential.Url);
            
            var teamResult = await GetAccountTeams(client, authCredential);

            var defaultTeam = teamResult.Teams.FirstOrDefault(c=> c.TeamId.Equals(model.TeamId));

            var projectKey = model.Name.Replace(" ", "").ToLower();
            var bitbucketModel = new {
                name = model.Name,
                key = projectKey,
                description = model.Description,
                is_private = model.ProjectVisibility == Domain.Models.ProjectVisibility.Public ? true  : false
            };

            var response = await client.PostAsync($"/{API_VERSION}/teams/{defaultTeam.UserName}/projects/", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(bitbucketModel), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                result.Fail($"Code: {response.StatusCode}, Reason: {await response.Content.ReadAsStringAsync()}");
                return result;
            }

            var projectCreated = await response.MapTo<CMSBitBucketProjectModel>();

            result.ProjectExternalId = projectKey;

            return result;
        }

        public async Task<CMSServiceCreateResultModel> CreateService(CMSAuthCredentialModel authCredential, CMSServiceCreateModel model)
        {
            CMSServiceCreateResultModel result = new CMSServiceCreateResultModel();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authCredential.AccessToken);
            client.BaseAddress = new Uri(authCredential.Url);
            
            var teamResult = await GetAccountTeams(client, authCredential);

            var defaultTeam = teamResult.Teams.FirstOrDefault(c=> c.TeamId.Equals(model.TeamId));

            var response = await client.GetAsync($"/{API_VERSION}/teams/{defaultTeam.UserName}/projects/");

            var projectResult = await response.MapTo<CMSBitBucketProjectListModel>();
            var defaultProject = projectResult.Projects.FirstOrDefault(c => c.Key.Equals(model.ProjectExternalId));

            var bitbucketModel = new
            {
                scm = "git",
                is_private = true,
                name = model.Name,
                description = model.Name,
                project = new
                {
                    key = defaultProject.Key
                }
            };

            var repositoryKey = model.Name.ToLower().Replace(" ", "");
            response = await client.PostAsync($"/{API_VERSION}/repositories/{defaultTeam.UserName}/{repositoryKey}", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(bitbucketModel), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                result.Fail($"Code: {response.StatusCode}, Reason: {await response.Content.ReadAsStringAsync()}");
                return result;
            }

            var serviceCreated = await response.MapTo<CMSBitBucketRepositoryModel>();

            result.ServiceExternalId = repositoryKey;
            result.ServiceExternalUrl = serviceCreated.Links.Clone.FirstOrDefault(c=> c.Name.Equals("https", StringComparison.OrdinalIgnoreCase)).Href;

            return result;
        }

        public async Task<CMSProjectAvailabilityResultModel> ValidateProjectAvailability(CMSAuthCredentialModel authCredential, string organization, string name)
        {
            CMSProjectAvailabilityResultModel result = new CMSProjectAvailabilityResultModel();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authCredential.AccessToken);
            client.BaseAddress = new Uri(authCredential.Url);
            
            var teamResult = await GetAccountTeams(client, authCredential);

            var defaultTeam = teamResult.Teams.FirstOrDefault(c=> c.TeamId.Equals(organization));

            if (defaultTeam.IsTeam) {
                var response = await client.GetAsync($"/{API_VERSION}/teams/{defaultTeam.UserName}/projects/");

                var projectResult = await response.MapTo<CMSBitBucketProjectListModel>();

                var existsProject = projectResult.Projects.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (existsProject)
                {
                    result.Fail($"The project {name} has already been taken in the CMS service");
                }

                return result;
            }

            var responseUser = await client.GetAsync($"/{API_VERSION}/users/{defaultTeam.UserName}/projects/");

            //var projectResult = await response.MapTo<CMSBitBucketProjectListModel>();

            //var existsProject = projectResult.Projects.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            //if (existsProject)
            //{
            //    result.Fail($"The project {name} has already been taken in the CMS service");
            //}

            //return result;

            return new CMSProjectAvailabilityResultModel { };
        }

        public async Task<CMSServiceAvailabilityResultModel> ValidateServiceAvailability(CMSAuthCredentialModel authCredential, string teamId, string projectExternalId, string projectName, string name)
        {
            CMSServiceAvailabilityResultModel result = new CMSServiceAvailabilityResultModel();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authCredential.AccessToken);
            client.BaseAddress = new Uri(authCredential.Url);
            
            var teamResult = await GetAccountTeams(client, authCredential);

            var defaultTeam = teamResult.Teams.FirstOrDefault(c=> c.TeamId.Equals(teamId));

            var response = await client.GetAsync(defaultTeam.Links.Repositories.Href);

            var serviceResult = await response.MapTo<CMSBitBucketRepositoryListModel>();
            var existsService = serviceResult.Repositories.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (existsService)
            {
                result.Fail($"The service {name} has already been taken in the {authCredential.Provider} service (Repository)");
            }

            return result;
        }

        private async Task<CMSBitBucketTeamListModel> GetAccountTeams(HttpClient client, CMSAuthCredentialModel authCredential) {
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"/{API_VERSION}/teams?role=admin");

            var response = await client.SendAsync(request);

            var teamResult = await response.MapTo<CMSBitBucketTeamListModel>();
            
            var userResponse = await client.GetAsync($"/{API_VERSION}/user");
            var userResult = await userResponse.MapTo<CMSBitBucketUserModel>();

            if (teamResult.Teams != null && teamResult.Teams.Count > 0) {
                teamResult.Teams.ForEach(x => {
                    x.IsTeam = true;
                });
            }

            //if (teamResult.Teams == null)
            //    teamResult.Teams = new List<CMSBitBucketTeamModel>();

            //teamResult.Teams.Add(new CMSBitBucketTeamModel { TeamId = userResult.Id, UserName = userResult.UserName, DisplayName = userResult.DisplayName });
            
            return teamResult;
        }

    }
}
