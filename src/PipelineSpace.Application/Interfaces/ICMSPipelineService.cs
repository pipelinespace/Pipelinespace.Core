using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Interfaces
{
    public interface ICMSPipelineService
    {
        Task<CMSPipelineAgentQueueResultModel> GetQueue(CMSPipelineAgentQueueParamModel @options);
        Task CreateBuild(CMSPipelineBuildParamModel @options);
        Task CreateRelease(CMSPipelineReleaseParamModel @options);
        Task CompleteApproval(CMSPipelineApprovalParamModel @options);
    }
}
