using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Infra.Data.ServiceAgent.Extentions;
using PipelineSpace.Infra.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class PipelineSpaceTemplateRepository : ITemplateService
    {
        readonly IHttpProxyService _httpProxyService;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        public PipelineSpaceTemplateRepository(IHttpProxyService httpProxyService,
                                                  IOptions<VSTSServiceOptions> vstsOptions)
        {
            _httpProxyService = httpProxyService;
            _vstsOptions = vstsOptions;
        }

        public async Task<CMSVSTSObjectRepositoryModel> GetTemplateBuildDefinition(string repository, string templateName, string definitionName, CMSAuthCredentialModel authCredential)
        {

            string accountUrl = $"https://{authCredential.AccountId}.visualstudio.com";
            string repositoryUrl = $"{accountUrl}/{this._vstsOptions.Value.ProjectName}/_apis/git/repositories/{repository}/items?path={templateName}/scripts/{definitionName}&$format=json&includeContent=true&versionDescriptor.version=master&versionDescriptor.versionType=branch&api-version=4.1&api-version={_vstsOptions.Value.ApiVersion}";

            var response = await _httpProxyService.GetAsync(repositoryUrl, authCredential);

            var result = new CMSVSTSObjectRepositoryModel();

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

            result = await response.MapTo<CMSVSTSObjectRepositoryModel>();

            return result;
        }

        public async Task<CMSVSTSObjectRepositoryModel> GetTemplateInfraDefinition(string repository, string templateName, string definitionName, CMSAuthCredentialModel authCredential)
        {

            string accountUrl = $"https://{authCredential.AccountId}.visualstudio.com";
            string repositoryUrl = $"{accountUrl}/{this._vstsOptions.Value.ProjectName}/_apis/git/repositories/{repository}/items?path={templateName}/scripts/{definitionName}&$format=json&includeContent=true&versionDescriptor.version=master&versionDescriptor.versionType=branch&api-version=4.1&api-version={_vstsOptions.Value.ApiVersion}";

            var response = await _httpProxyService.GetAsync(repositoryUrl, authCredential);

            var result = new CMSVSTSObjectRepositoryModel();

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

            result = await response.MapTo<CMSVSTSObjectRepositoryModel>();

            return result;
        }
    }
}
