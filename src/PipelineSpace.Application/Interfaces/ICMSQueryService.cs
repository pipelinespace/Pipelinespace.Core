using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Interfaces
{
    public interface ICMSQueryService
    {
        Task<CMSAccountListModel> GetAccounts(CMSAuthCredentialModel authCredential);
        Task<CMSProjectListModel> GetProjects(string accountId, CMSAuthCredentialModel authCredential);
        Task<CMSProjectModel> GetProject(string accountId, string projectId, CMSAuthCredentialModel authCredential);
        Task<CMSRepositoryListModel> GetRepositories(string projectId, CMSAuthCredentialModel authCredential);
        Task<CMSBranchListModel> GetBranches(string projectId, string repositoryId, CMSAuthCredentialModel authCredential);
        Task<CMSAgentPoolListModel> GetAgentPools(CMSAuthCredentialModel authCredential);
        Task<CMSRepositoryListItemModel> GetRepository(string projectId, string repositoryId, CMSAuthCredentialModel authCredential);
    }
}
