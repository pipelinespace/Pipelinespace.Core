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
    public class CMSGitLabQueryServiceAgentRepository : ICMSQueryService
    {
        readonly IHttpProxyService _httpProxyService;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountServiceOptions;
        public CMSGitLabQueryServiceAgentRepository(IHttpProxyService httpProxyService,
                                                    IOptions<FakeAccountServiceOptions> fakeAccountServiceOptions)
        {
            _httpProxyService = httpProxyService;
            _fakeAccountServiceOptions = fakeAccountServiceOptions;
        }

        public Task<CMSAccountListModel> GetAccounts(CMSAuthCredentialModel authCredential)
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

        public Task<CMSBranchListModel> GetBranches(string projectId, string repositoryId, CMSAuthCredentialModel authCredential)
        {
            throw new NotImplementedException();
        }

        public Task<CMSProjectModel> GetProject(string accountId, string projectId, CMSAuthCredentialModel authCredential)
        {
            throw new NotImplementedException();
        }

        public Task<CMSProjectListModel> GetProjects(string accountId, CMSAuthCredentialModel authCredential)
        {
            throw new NotImplementedException();
        }

        public async Task<CMSRepositoryListModel> GetRepositories(string projectId, CMSAuthCredentialModel authCredential)
        {
            var httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{authCredential.Url}/projects/{projectId}");
            request.Headers.Add("Private-Token", authCredential.AccessToken);

            var response = await httpClient.SendAsync(request);

            var serviceResult = await response.MapTo<List<CMSGitLabRepositoryModel>>();

            var model = new CMSRepositoryListModel();

            model.Items = serviceResult.Select(c => new CMSRepositoryListItemModel
            {
                ServiceId = c.Name,
                Name = c.FullName,
                Description = c.FullName,
                Link = c.CloneUrl
            }).ToList();

            return model;
        }

        public async Task<CMSRepositoryListItemModel> GetRepository(string projectId, string repositoryId, CMSAuthCredentialModel authCredential)
        {
            var httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{authCredential.Url}/projects/{repositoryId}");
            request.Headers.Add("Private-Token", authCredential.AccessToken);

            var response = await httpClient.SendAsync(request);

            var serviceResult = await response.MapTo<CMSGitLabRepositoryModel>();
            
            return new CMSRepositoryListItemModel
            {
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
