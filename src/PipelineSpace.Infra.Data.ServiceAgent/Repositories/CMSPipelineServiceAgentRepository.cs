using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Infra.Data.ServiceAgent.Extentions;
using PipelineSpace.Infra.Data.ServiceAgent.Models.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CMSPipelineServiceAgentRepository : ICMSPipelineService
    {
        readonly IHttpProxyService _httpProxyService;
        public CMSPipelineServiceAgentRepository(IHttpProxyService httpProxyService)
        {
            _httpProxyService = httpProxyService;
        }

        public async Task<CMSPipelineAgentQueueResultModel> GetQueue(CMSPipelineAgentQueueParamModel @options)
        {
            string accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";

            CMSAuthCredentialModel authCredentials = new CMSAuthCredentialModel();
            authCredentials.Type = "Basic";
            authCredentials.AccessToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));
            authCredentials.Url = accountUrl;

            string queueUrl = $"/{@options.VSTSAccountProjectId}/_apis/distributedtask/queues";
            var queueResponse = await _httpProxyService.GetAsync(queueUrl, authCredentials);
            queueResponse.EnsureSuccessStatusCode();

            var queues = await queueResponse.MapTo<CMSPipelineQueueListModel>();

            var defaultQueue = queues.Items.FirstOrDefault(x => x.Pool.Id == int.Parse(@options.AgentPoolId));
            if (defaultQueue == null)
                throw new Exception($"Agent Pool with id {@options.AgentPoolId} was not found");

            return new CMSPipelineAgentQueueResultModel()
            {
                QueueId = defaultQueue.Id,
                QueueName = defaultQueue.Name,
                PoolId = defaultQueue.Pool.Id,
                PoolName = defaultQueue.Pool.Name
            };
        }

        public async Task CreateBuild(CMSPipelineBuildParamModel @options)
        {
            string accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";

            CMSAuthCredentialModel authCredentials = new CMSAuthCredentialModel();
            authCredentials.Type = "Basic";
            authCredentials.AccessToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));
            authCredentials.Url = accountUrl;

            string queueBuildUrl = $"/{@options.VSTSAccountProjectId}/_apis/build/builds?api-version={@options.VSTSAPIVersion}";

            var queueBuild = PipelineBuildModel.Create(@options.QueueId, @options.BuildDefinitionId, @options.ProjectExternalId, @options.SourceBranch);
            var queueBuildResponse = await _httpProxyService.PostAsync(queueBuildUrl, queueBuild, authCredentials);
            queueBuildResponse.EnsureSuccessStatusCode();
        }

        public async Task CreateRelease(CMSPipelineReleaseParamModel @options)
        {
            string accountUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";

            CMSAuthCredentialModel authCredentials = new CMSAuthCredentialModel();
            authCredentials.Type = "Basic";
            authCredentials.AccessToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));
            authCredentials.Url = accountUrl;

            string queueReleaseUrl = $"/{@options.VSTSAccountProjectId}/_apis/release/releases?api-version={@options.VSTSAPIVersion}";

            var queueRelease = PipelineReleaseModel.Create(@options.ReleaseDefinitionId, @options.Alias, @options.VersionId, @options.VersionName, options.Description);
            var queueReleaseResponse = await _httpProxyService.PostAsync(queueReleaseUrl, queueRelease, authCredentials);
            queueReleaseResponse.EnsureSuccessStatusCode();
        }

        public async Task CompleteApproval(CMSPipelineApprovalParamModel @options)
        {
            string accountUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";

            CMSAuthCredentialModel authCredentials = new CMSAuthCredentialModel();
            authCredentials.Type = "Basic";
            authCredentials.AccessToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));
            authCredentials.Url = accountUrl;

            string completeApprovalUrl = $"/{@options.VSTSAccountProjectId}/_apis/release/approvals/{@options.ApprovalId}?api-version={@options.VSTSAPIVersion}";

            var completeApproval = PipelineApprovalModel.Create(@options.Status, @options.Comments);
            var completeApprovalResponse = await _httpProxyService.PatchAsync(completeApprovalUrl, completeApproval, authCredentials);
            completeApprovalResponse.EnsureSuccessStatusCode();
        }
    }
}
