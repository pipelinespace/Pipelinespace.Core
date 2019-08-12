using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PipelineSpace.Infra.Data.ServiceAgent.Extentions;
using PipelineSpace.Infra.Data.ServiceAgent.Models.GitHub;
using System.Linq;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CMSGitHubServiceAgentRepository : ICMSService
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

        readonly IHttpProxyService _httpProxyService;

        public CMSGitHubServiceAgentRepository(IHttpProxyService httpProxyService)
        {
            _httpProxyService = httpProxyService;
        }

        public async Task<CMSProjectAvailabilityResultModel> ValidateProjectAvailability(CMSAuthCredentialModel authCredential, string organization, string name)
        {
            CMSProjectAvailabilityResultModel result = new CMSProjectAvailabilityResultModel();

            var dic = new Dictionary<string, string>();
            dic.Add("Accept", "application/vnd.github.inertia-preview+json");
            dic.Add("User-Agent", API_UserAgent);

            var accountList = await GetAccounts(authCredential);

            var defaultTeam = accountList.Items.FirstOrDefault(c => c.AccountId.Equals(authCredential.AccountId));
            if (defaultTeam.IsOrganization)
            {
                var response = await _httpProxyService.GetAsync($"/orgs/{defaultTeam.Name}/projects", authCredential, dic);

                var projectResult = await response.MapTo<List<CMSGitHubProjectModel>>();

                var existsProject = projectResult.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (existsProject)
                {
                    result.Fail($"The project {name} has already been taken in the CMS service");
                }
            }

            return result;
        }

        public async Task<CMSProjectCreateResultModel> CreateProject(CMSAuthCredentialModel authCredential, CMSProjectCreateModel model)
        {
            CMSProjectCreateResultModel result = new CMSProjectCreateResultModel();

            var dic = new Dictionary<string, string>();
            dic.Add("Accept", "application/vnd.github.inertia-preview+json");
            dic.Add("User-Agent", API_UserAgent);

            var accountList = await GetAccounts(authCredential);

            var defaultTeam = accountList.Items.FirstOrDefault(c => c.AccountId.Equals(authCredential.AccountId));

            var gitHubModel = new
            {
                name = model.Name,
                body = model.Description,
            };

            if (defaultTeam.IsOrganization)
            {
                //var response = await _httpProxyService.PostAsync($"/orgs/{defaultTeam.Name}/projects", gitHubModel, authCredential, dic);
                //if (!response.IsSuccessStatusCode)
                //{
                //    result.Fail($"Code: {response.StatusCode}, Reason: {await response.Content.ReadAsStringAsync()}");
                //    return result;
                //}
                //
                //var projectCreated = await response.MapTo<CMSGitHubProjectModel>();
                //
                //result.ProjectExternalId = projectCreated.Id;
                result.ProjectExternalId = $"{Guid.NewGuid()}";
            }
            else {
                result.ProjectExternalId = $"{Guid.NewGuid()}";
            }

            return result;
        }

        public async Task<CMSServiceAvailabilityResultModel> ValidateServiceAvailability(CMSAuthCredentialModel authCredential, string teamId, string projectExternalId, string projectName, string name)
        {
            CMSServiceAvailabilityResultModel result = new CMSServiceAvailabilityResultModel();

            var accountList = await GetAccounts(authCredential);
            var defaultTeam = accountList.Items.FirstOrDefault(c => c.AccountId.Equals(authCredential.AccountId));

            var urlRepo = "";
            if (defaultTeam != null && defaultTeam.IsOrganization)
            {
                urlRepo = $"/orgs/{defaultTeam.Name}/repos";
            }
            else
            {
                urlRepo = $"/user/repos";
            }

            var response = await _httpProxyService.GetAsync(urlRepo, authCredential, Headers);

            var serviceResult = await response.MapTo<List<CMSGitHubRepositoryModel>>();

            var serviceName = GetServiceName(projectName, name);

            var existsService = serviceResult.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (existsService)
            {
                result.Fail($"The service {name} has already been taken in the CMS service");
            }

            return result;
        }

        public async Task<CMSServiceCreateResultModel> CreateService(CMSAuthCredentialModel authCredential, CMSServiceCreateModel model)
        {
            
            CMSServiceCreateResultModel result = new CMSServiceCreateResultModel();

            var accountList = await GetAccounts(authCredential);
            var defaultTeam = accountList.Items.FirstOrDefault(c => c.AccountId.Equals(authCredential.AccountId));

            var gitHubModel = new
            {
                name = GetServiceName(model.ProjectName, model.Name),
                description = model.ProjectName,
                Private = model.IsPublic ? false : true,
                has_issues = true,
                has_projects = true,
                has_wiki = true
            };

            var urlRepo = "";
            if (defaultTeam != null && defaultTeam.IsOrganization)
            {
                urlRepo = $"/orgs/{defaultTeam.Name}/repos";
            }
            else {
                urlRepo = $"/user/repos";
            }

            var response = await _httpProxyService.PostAsync(urlRepo, gitHubModel, authCredential, Headers);

            if (!response.IsSuccessStatusCode)
            {
                result.Fail($"Code: {response.StatusCode}, Reason: {await response.Content.ReadAsStringAsync()}");
                return result;
            }

            var serviceCreated = await response.MapTo<CMSGitHubRepositoryModel>();

            result.ServiceExternalId = serviceCreated.Id;
            result.ServiceExternalUrl = serviceCreated.CloneUrl;

            return result;
        }

        private async Task<CMSAccountListModel> GetAccounts(CMSAuthCredentialModel authCredential)
        {
            var list = new CMSAccountListModel() { Items = new List<CMSAccountListItemModel>() };
            var response = await _httpProxyService.GetAsync($"/user", authCredential, Headers);

            var user = await response.MapTo<CMSGitHubUserModel>();

            list.Items.Add(new CMSAccountListItemModel
            {
                AccountId = user.AccountId,
                Name = user.UserName,
                Description = user.UserName,
                IsOrganization = false,
            });

            response = await _httpProxyService.GetAsync($"/user/orgs", authCredential, Headers);

            var teamResult = await response.MapTo<List<CMSGitHubTeamModel>>();

            list.Items.AddRange(teamResult.Select(c => new CMSAccountListItemModel
            {
                AccountId = c.TeamId,
                Description = c.DisplayName,
                Name = c.DisplayName,
                IsOrganization = true
            }));

            return list;
        }

        private string GetServiceName(string projectName, string name)
        {
            return name;
        }
    }
}
