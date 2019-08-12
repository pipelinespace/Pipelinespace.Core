using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Infra.Data.ServiceAgent.Extentions;
using PipelineSpace.Infra.Data.ServiceAgent.Models.VSTS;
using PipelineSpace.Infra.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CMSVSTSQueryServiceAgentRepository : ICMSQueryService
    {
        readonly IHttpProxyService _httpProxyService;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        public CMSVSTSQueryServiceAgentRepository(IHttpProxyService httpProxyService,
                                                  IOptions<VSTSServiceOptions> vstsOptions)
        {
            _httpProxyService = httpProxyService;
            _vstsOptions = vstsOptions;
        }

        public async Task<CMSAccountListModel> GetAccounts(CMSAuthCredentialModel authCredential)
        {
            CMSAccountListModel result = new CMSAccountListModel();
            var response = await _httpProxyService.GetAsync($"/_apis/accounts", authCredential);

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

            var accountResult = await response.MapTo<List<CMSVSTSAccountListItemModel>>();
            result.Items = accountResult.Select(x=> new CMSAccountListItemModel() {
                AccountId = x.AccountId,
                Name = x.AccountName
            }).ToList();

            return result;
        }

        public async Task<CMSProjectListModel> GetProjects(string accountId, CMSAuthCredentialModel authCredential)
        {
            CMSProjectListModel result = new CMSProjectListModel();
            var response = await _httpProxyService.GetAsync($"/_apis/projects?api-version=5.0", authCredential);

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

            var projectListResult = await response.MapTo<CMSProjectListModel>();
            result.Items = projectListResult.Items.Select(x => new CMSProjectListItemModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return result;
        }
        
        public async Task<CMSAgentPoolListModel> GetAgentPools(CMSAuthCredentialModel authCredential)
        {
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
            result.Items = agentoPoolResult.Items.Where(x=> x.Size > 0).Select(x => new CMSAgentPoolListItemModel()
            {
                Id = x.Id,
                Name = x.Name,
                IsHosted = x.IsHosted
            }).ToList();

            return result;
        }

        public async Task<CMSRepositoryListModel> GetRepositories(string projectId, CMSAuthCredentialModel authCredential)
        {
            CMSRepositoryListModel result = new CMSRepositoryListModel();
            
            string repositoryUrl = $"{authCredential.Url}/{projectId}/_apis/git/repositories?api-version=4.1&api-version={_vstsOptions.Value.ApiVersion}";

            var response = await _httpProxyService.GetAsync(repositoryUrl, authCredential);

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

            var serviceResult = await response.MapTo<CMSVSTSRepositoryListModel>();

            result.Items = serviceResult.Items.Select(c => new CMSRepositoryListItemModel {
                AccountId = authCredential.AccountId,
                Name = c.Name,
                Link = c.RemoteUrl,
                Description = c.Name,
                ServiceId = c.Id
            }).ToList();

            return result;
        }

        public async Task<CMSProjectModel> GetProject(string accountId, string projectId, CMSAuthCredentialModel authCredential)
        {
            CMSProjectModel result = new CMSProjectModel();

            string accountUrl = $"https://{accountId}.visualstudio.com";
            var response = await _httpProxyService.GetAsync($"{accountUrl}/_apis/projects/{projectId}?api-version={this._vstsOptions.Value.ApiVersion}", authCredential);

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

            result = await response.MapTo<CMSProjectModel>();

            return result;
        }

        public async Task<CMSBranchListModel> GetBranches(string projectId, string repositoryId, CMSAuthCredentialModel authCredential)
        {
            CMSBranchListModel result = new CMSBranchListModel();

            string repositoryUrl = $"{authCredential.Url}/{projectId}/_apis/git/repositories/{repositoryId}/refs?api-version=4.1&api-version={_vstsOptions.Value.ApiVersion}";

            var response = await _httpProxyService.GetAsync(repositoryUrl, authCredential);

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

            var serviceResult = await response.MapTo<CMSVSTSBranchListModel>();

            result.Items = serviceResult.Items.Select(c => new CMSBranchListItemModel {
                Id = c.Name,
                Name = c.Name
            }).ToList();

            return result;
        }

        public async Task<CMSRepositoryListItemModel> GetRepository(string projectId, string repositoryId, CMSAuthCredentialModel authCredential)
        {
            string repositoryUrl = $"{authCredential.Url}/{projectId}/_apis/git/repositories/{repositoryId}?api-version=4.1&api-version={_vstsOptions.Value.ApiVersion}";

            var response = await _httpProxyService.GetAsync(repositoryUrl, authCredential);

            var serviceResult = await response.MapTo<CMSVSTSRepositoryListItemModel>();

            return new CMSRepositoryListItemModel {
                AccountId = authCredential.AccountId,
                Name = serviceResult.Name,
                Link = serviceResult.RemoteUrl,
                SSHUrl = serviceResult.SSHUrl,
                Description = serviceResult.Name,
                ServiceId = serviceResult.Id,
                DefaultBranch = serviceResult.DefaultBranch
            };
        }
    }
}
