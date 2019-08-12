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
    public class CMSVSTSServiceAgentRepository : ICMSService
    {
        readonly IHttpProxyService _httpProxyService;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        public CMSVSTSServiceAgentRepository(IHttpProxyService httpProxyService,
                                             IOptions<VSTSServiceOptions> vstsOptions)
        {
            _httpProxyService = httpProxyService;
            _vstsOptions = vstsOptions;
        }

        public async Task<CMSProjectAvailabilityResultModel> ValidateProjectAvailability(CMSAuthCredentialModel authCredential, string organization, string name)
        {
            CMSProjectAvailabilityResultModel result = new CMSProjectAvailabilityResultModel();
            var response = await _httpProxyService.GetAsync($"/_apis/projects?api-version={_vstsOptions.Value.ApiVersion}", authCredential);

            if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NonAuthoritativeInformation)
            {
                if(response.StatusCode == System.Net.HttpStatusCode.NonAuthoritativeInformation)
                {
                    result.Fail($"Code: {response.StatusCode}, Reason: The credentials are not correct");
                    return result;
                }

                result.Fail($"Code: {response.StatusCode}, Reason: {await response.Content.ReadAsStringAsync()}");
                return result;
            }

            var projectResult = await response.MapTo<CMSVSTSProjectListModel>();
            var existsProject = projectResult.Items.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (existsProject)
            {
                result.Fail($"The project {name} has already been taken in the CMS service");
            }

            return result;
        }

        public async Task<CMSProjectCreateResultModel> CreateProject(CMSAuthCredentialModel authCredential, CMSProjectCreateModel model)
        {
            //default data
            var vstsmodel = new CMSVSTSProjectCreateModel();
            vstsmodel.Name = model.Name;
            vstsmodel.Description = model.Description;
            vstsmodel.Visibility = model.ProjectVisibility == Domain.Models.ProjectVisibility.Private ? "private" : "public";
            vstsmodel.Capabilities = new CMSVSTSProjectCapabilityModel();
            vstsmodel.Capabilities.VersionControl = new CMSVSTSProjectVersionControlModel();
            vstsmodel.Capabilities.VersionControl.SourceControlType = "Git";
            vstsmodel.Capabilities.ProcessTemplate = new CMSVSTSProjectProcessTemplateModel();
            vstsmodel.Capabilities.ProcessTemplate.TemplateTypeId = "adcc42ab-9882-485e-a3ed-7678f01f66bc";

            CMSProjectCreateResultModel result = new CMSProjectCreateResultModel();

            var response = await _httpProxyService.PostAsync($"/_apis/projects?api-version={_vstsOptions.Value.ApiVersion}", vstsmodel, authCredential);
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

            return result;
        }

        public async Task<CMSServiceAvailabilityResultModel> ValidateServiceAvailability(CMSAuthCredentialModel authCredential, string teamId, string projectExternalId, string projectName, string name)
        {
            CMSServiceAvailabilityResultModel result = new CMSServiceAvailabilityResultModel();
            var response = await _httpProxyService.GetAsync($"{projectExternalId}/_apis/git/repositories?api-version={_vstsOptions.Value.ApiVersion}", authCredential);

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

            var serviceResult = await response.MapTo<CMSVSTSServiceListModel>();
            var existsService = serviceResult.Items.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (existsService)
            {
                result.Fail($"The service {name} has already been taken in the VSTS service (Repository)");
            }

            return result;
        }

        public async Task<CMSServiceCreateResultModel> CreateService(CMSAuthCredentialModel authCredential, CMSServiceCreateModel model)
        {
            var vstsmodel = new CMSVSTSServiceCreateModel();
            vstsmodel.Name = model.Name;
            vstsmodel.Project = new CMSVSTSServiceProjectCreateModel();
            vstsmodel.Project.Id = model.ProjectExternalId;

            CMSServiceCreateResultModel result = new CMSServiceCreateResultModel();

            var response = await _httpProxyService.PostAsync($"{model.ProjectExternalId}/_apis/git/repositories?api-version={_vstsOptions.Value.ApiVersion}", vstsmodel, authCredential);

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

            var responseModel = await response.MapTo<CMSVSTSServiceCreateResultModel>();
            result.ServiceExternalId = responseModel.Id;
            result.ServiceExternalUrl = responseModel.RemoteUrl;

            return result;
        }
    }
}
