using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Infra.Data.ServiceAgent.Extentions;
using PipelineSpace.Infra.Data.ServiceAgent.Models.GitLab;
using PipelineSpace.Infra.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CMSGitLabServiceAgentRepository : ICMSService
    {

        readonly IHttpProxyService _httpProxyService;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountServiceOptions;
        public CMSGitLabServiceAgentRepository(IHttpProxyService httpProxyService,
                                                    IOptions<FakeAccountServiceOptions> fakeAccountServiceOptions)
        {
            _httpProxyService = httpProxyService;
            _fakeAccountServiceOptions = fakeAccountServiceOptions;
        }

        public async Task<CMSProjectAvailabilityResultModel> ValidateProjectAvailability(CMSAuthCredentialModel authCredential, string organization, string name)
        {
            CMSProjectAvailabilityResultModel result = new CMSProjectAvailabilityResultModel();
            
            var httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{authCredential.Url}/groups?owned=true");
            request.Headers.Add("Private-Token", authCredential.AccessToken);

            var response = await httpClient.SendAsync(request);

            var data = response.Content.ReadAsStringAsync().Result;
            var projectResult = await response.MapTo<List<CMSGitLabTeamModel>>();

            var existsProject = projectResult.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (existsProject)
            {
                result.Fail($"The project {name} has already been taken in the {authCredential.Provider} service");
            }

            return result;
        }

        public async Task<CMSProjectCreateResultModel> CreateProject(CMSAuthCredentialModel authCredential, CMSProjectCreateModel model)
        {
            CMSProjectCreateResultModel result = new CMSProjectCreateResultModel();

            var httpClient = new HttpClient();
            
            var organizationKey = model.OrganizationName.Replace(" ", "").ToLower();
            var projectKey = model.Name.Replace(" ", "").ToLower();
            
            var gitLabModel = new
            {
                name = model.Name,
                path = $"{organizationKey}-{projectKey}",
                description = model.Description,
                visibility = model.ProjectVisibility == Domain.Models.ProjectVisibility.Private ? "private" : "public",
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{authCredential.Url}/groups");

            request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(gitLabModel), Encoding.UTF8, "application/json");

            request.Headers.Add("Private-Token", authCredential.AccessToken);

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                result.Fail($"Code: {response.StatusCode}, Reason: {await response.Content.ReadAsStringAsync()}");
                return result;
            }

            var projectCreated = await response.MapTo<CMSGitLabProjectModel>();

            result.ProjectExternalId = projectCreated.Id.ToString();

            return result;
        }

        public async Task<CMSServiceCreateResultModel> CreateService(CMSAuthCredentialModel authCredential, CMSServiceCreateModel model)
        {
            CMSServiceCreateResultModel result = new CMSServiceCreateResultModel();

            var httpClient = new HttpClient();
            
            var gitLabModel = new
            {
                name = model.Name,
                description = model.ProjectName,
                namespace_id = model.ProjectExternalId,
                issues_enabled = true,
                merge_requests_enabled = true,
                ci_config_path = "scripts/build.definition.yml"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{authCredential.Url}/projects");

            request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(gitLabModel), Encoding.UTF8, "application/json");

            request.Headers.Add("Private-Token", authCredential.AccessToken);

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                result.Fail($"Code: {response.StatusCode}, Reason: {await response.Content.ReadAsStringAsync()}");
                return result;
            }

            var serviceCreated = await response.MapTo<CMSGitLabProjectModel>();

            result.ServiceExternalId = serviceCreated.Id.ToString();
            result.ServiceExternalUrl = serviceCreated.CloneUrl;
            
            return result;
        }


        public async Task<CMSServiceAvailabilityResultModel> ValidateServiceAvailability(CMSAuthCredentialModel authCredential, string teamId, string projectExternalId, string projectName, string name)
        {
            CMSServiceAvailabilityResultModel result = new CMSServiceAvailabilityResultModel();

            var httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{authCredential.Url}/projects?owned=true");
            request.Headers.Add("Private-Token", authCredential.AccessToken);

            var response = await httpClient.SendAsync(request);

            var data = response.Content.ReadAsStringAsync().Result;
            var projectResult = await response.MapTo<List<CMSGitLabProjectModel>>();

            var existsProject = projectResult.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (existsProject)
            {
                result.Fail($"The service {name} has already been taken in the {authCredential.Provider} service");
            }

            return result;
        }
    }
}
