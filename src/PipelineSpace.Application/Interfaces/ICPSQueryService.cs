using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Interfaces
{
    public interface ICPSQueryService
    {
        Task<CPSCloudResourceSummaryModel> GetSummary(string organization, string project, string service, List<string> environments, string feature, CPSAuthCredentialModel authCredential);
        Task<CPSCloudResourceEnvironmentSummaryModel> GetEnvironmentSummary(string organization, string project, string service, string environmentName, string feature, CPSAuthCredentialModel authCredential);
    }
}
