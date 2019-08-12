using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Infra.Data.ServiceAgent.Extentions;
using PipelineSpace.Infra.Data.ServiceAgent.Models.BitBucket;
using PipelineSpace.Infra.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CMSBitBucketQueryServiceAgentRepository : ICMSQueryService
    {
        private const string API_VERSION = "2.0";

        readonly IHttpProxyService _httpProxyService;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountServiceOptions;
        public CMSBitBucketQueryServiceAgentRepository(IHttpProxyService httpProxyService,
                                                    IOptions<FakeAccountServiceOptions> fakeAccountServiceOptions)
        {
            _httpProxyService = httpProxyService;
            _fakeAccountServiceOptions = fakeAccountServiceOptions;
        }

        public async Task<CMSAccountListModel> GetAccounts(CMSAuthCredentialModel authCredential)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authCredential.AccessToken);
            client.BaseAddress = new Uri(authCredential.Url);

            var response = await client.GetAsync($"/{API_VERSION}/teams?role=admin");

            var teamResult = await response.MapTo<CMSBitBucketTeamListModel>();

            CMSAccountListModel list = new CMSAccountListModel();
            list.Items = teamResult.Teams.Select(c => new CMSAccountListItemModel
            {
                 AccountId = c.TeamId,
                 Description = c.DisplayName,
                 Name = c.UserName
            }).ToList();

            return list;
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

        public Task<CMSBranchListModel> GetBranches(string projectId, string repositoryId, CMSAuthCredentialModel authCredential)
        {
            throw new NotImplementedException();
        }

        public Task<CMSProjectModel> GetProject(string accountId, string projectId, CMSAuthCredentialModel authCredential)
        {
            throw new NotImplementedException();
        }

        public async Task<CMSProjectListModel> GetProjects(string accountId, CMSAuthCredentialModel authCredential)
        {

            
             var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authCredential.AccessToken);
            client.BaseAddress = new Uri(authCredential.Url);

            var response = await client.GetAsync($"/{API_VERSION}/teams?role=admin");
            
            var teamResult = await response.MapTo<CMSBitBucketTeamListModel>();

            CMSProjectListModel list = new CMSProjectListModel();
            var projectList = new List<CMSProjectListItemModel>();
            foreach (var item in teamResult.Teams)
            {
                response = await client.GetAsync($"/{API_VERSION}/teams/{item.UserName}/projects/");

                var projectResult = await response.MapTo<CMSBitBucketProjectListModel>();

                foreach (var project in projectResult.Projects)
                {
                    projectList.Add(new CMSProjectListItemModel
                    {
                        Id = project.Key,
                        DisplayName = $@"{item.DisplayName}\{project.Name}",
                        Name = project.Name
                    });
                }
            }
            
            list.Items = projectList.ToList();
            
            return list;
            
        }

        public async Task<CMSRepositoryListModel> GetRepositories(string accountId, string projectId, CMSAuthCredentialModel authCredential)
        {
            return new CMSRepositoryListModel { };
        }

        public async Task<CMSRepositoryListModel> GetRepositories(string projectId, CMSAuthCredentialModel authCredential)
        {
            return new CMSRepositoryListModel();
        }

        public async Task<CMSRepositoryListItemModel> GetRepository(string projectId, string repositoryId, CMSAuthCredentialModel authCredential)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authCredential.AccessToken);
            client.BaseAddress = new Uri(authCredential.Url);

            var response = await client.GetAsync($"/{API_VERSION}/teams?role=admin");

            var teamResult = await response.MapTo<CMSBitBucketTeamListModel>();

            var defaultTeam = teamResult.Teams.FirstOrDefault(c => c.TeamId.Equals(projectId));

            response = await client.GetAsync($"/{API_VERSION}/repositories/{defaultTeam.UserName}/{repositoryId}");

            var serviceResult = await response.MapTo<CMSBitBucketRepositoryModel>();

            return new CMSRepositoryListItemModel
            {
                AccountId = authCredential.AccountId,
                Name = serviceResult.Name,
                Link = serviceResult.Links.Clone.FirstOrDefault(c => c.Name.Equals("https", StringComparison.OrdinalIgnoreCase)).Href,
                SSHUrl = serviceResult.Links.Clone.FirstOrDefault(c => c.Name.Equals("ssh", StringComparison.OrdinalIgnoreCase)).Href,
                Description = serviceResult.Name,
                ServiceId = serviceResult.Id.ToString(),
                DefaultBranch = serviceResult.MainBranch.Name
            };
        }
    }
}
