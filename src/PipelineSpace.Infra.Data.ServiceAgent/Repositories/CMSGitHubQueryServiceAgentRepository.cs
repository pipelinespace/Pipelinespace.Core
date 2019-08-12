using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Infra.Data.ServiceAgent.Extentions;
using PipelineSpace.Infra.Data.ServiceAgent.Models.GitHub;
using PipelineSpace.Infra.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CMSGitHubQueryServiceAgentRepository : ICMSQueryService
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
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountServiceOptions;
        public CMSGitHubQueryServiceAgentRepository(IHttpProxyService httpProxyService,
                                                    IOptions<FakeAccountServiceOptions> fakeAccountServiceOptions)
        {
            _httpProxyService = httpProxyService;
            _fakeAccountServiceOptions = fakeAccountServiceOptions;
        }

        public async Task<CMSAccountListModel> GetAccounts(CMSAuthCredentialModel authCredential)
        {
            var response = await _httpProxyService.GetAsync($"/user", authCredential, Headers);
            var user = await response.MapTo<CMSGitHubUserModel>();

            CMSAccountListModel list = new CMSAccountListModel();

            list.Items = new List<CMSAccountListItemModel>();
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

        public Task<CMSProjectListModel> GetProjects(string accountId, CMSAuthCredentialModel authCredential)
        {
            throw new NotImplementedException();
        }
        
        public async Task<CMSAgentPoolListModel> GetAgentPools(CMSAuthCredentialModel authCredential)
        {
            authCredential.Url = $"https://{_fakeAccountServiceOptions.Value.AccountId}.visualstudio.com";
            authCredential.Type = "Basic";
            authCredential.AccessToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _fakeAccountServiceOptions.Value.AccessSecret)));

            CMSAgentPoolListModel result = new CMSAgentPoolListModel();
            var response = await _httpProxyService.GetAsync($"/_apis/distributedtask/pools", authCredential);

            if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NonAuthoritativeInformation)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NonAuthoritativeInformation)
                {
                    result.Fail($"Code: {response.StatusCode}, Reason: The credentials are not correct");
                    return result;
                }

                result.Fail($"Code: {response.StatusCode}, Reason: {await response.Content.ReadAsStringAsync()}");
                return result;
            }

            var agentoPoolResult = await response.MapTo<CMSAgentPoolListModel>();
            result.Items = agentoPoolResult.Items.Where(x => x.Size > 0).Select(x => new CMSAgentPoolListItemModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return result;
        }

        public async Task<CMSRepositoryListModel> GetRepositories(string projectId, CMSAuthCredentialModel authCredential)
        {
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

            var model = new CMSRepositoryListModel();

            model.Items = serviceResult.Select(c => new CMSRepositoryListItemModel {
                ServiceId = c.Name,
                Name = c.FullName,
                Description = c.FullName,
                Link = c.CloneUrl
            }).ToList();

            return model;
        }

        public Task<CMSProjectModel> GetProject(string accountId, string projectId, CMSAuthCredentialModel authCredential)
        {
            throw new NotImplementedException();
        }

        public async Task<CMSBranchListModel> GetBranches(string projectId, string repositoryId, CMSAuthCredentialModel authCredential)
        {
            var accountList = await GetAccounts(authCredential);
            var defaultTeam = accountList.Items.FirstOrDefault(c => c.AccountId.Equals(authCredential.AccountId));

            var urlRepo = "";
            if (defaultTeam != null && defaultTeam.IsOrganization)
            {
                urlRepo = $"/orgs/{defaultTeam.Name}/repos/{repositoryId}/branches";
            }
            else
            {
                urlRepo = $"/repos/{defaultTeam.Name}/{repositoryId}/branches";
            }

            var response = await _httpProxyService.GetAsync(urlRepo, authCredential, Headers);

            var serviceResult = await response.MapTo<List<CMSGitHubBranchModel>>();

            var model = new CMSBranchListModel();

            model.Items = serviceResult.Select(c => new CMSBranchListItemModel
            {
                 Id = c.Name, 
                Name = c.Name
            }).ToList();

            return model;
        }

        public async Task<CMSRepositoryListItemModel> GetRepository(string projectId, string repositoryId, CMSAuthCredentialModel authCredential)
        {
            var accountList = await GetAccounts(authCredential);
            var defaultTeam = accountList.Items.FirstOrDefault(c => c.AccountId.Equals(authCredential.AccountId));

            var urlRepo = "";
            if (defaultTeam != null && defaultTeam.IsOrganization)
            {
                urlRepo = $"/orgs/{defaultTeam.Name}/repos/{repositoryId}";
            }
            else
            {
                urlRepo = $"/repos/{defaultTeam.Name}/{repositoryId}";
            }

            var response = await _httpProxyService.GetAsync(urlRepo, authCredential, Headers);

            var serviceResult = await response.MapTo<CMSGitHubRepositoryModel>();
            
            return new CMSRepositoryListItemModel {
                AccountId = authCredential.AccountId,
                Name = serviceResult.Name,
                Link = serviceResult.CloneUrl,
                SSHUrl = serviceResult.SSHUrl,
                Description = serviceResult.Name,
                ServiceId = serviceResult.Id,
                DefaultBranch = serviceResult.DefaultBranch
            };
        }
    }
}
