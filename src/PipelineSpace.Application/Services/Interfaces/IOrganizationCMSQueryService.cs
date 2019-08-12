using PipelineSpace.Application.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IOrganizationCMSQueryService
    {
        Task<OrganizationCMSListRp> GetOrganizationConfigurationManagementServices(Guid organizationId, CMSConnectionType connectionType);
        Task<OrganizationCMSGetRp> GetOrganizationConfigurationManagementServiceById(Guid organizationId, Guid organizationCMSId);
        Task<OrganizationCMSAgentPoolListRp> GetOrganizationConfigurationManagementServiceAgentPools(Guid organizationId, Guid organizationCMSId);
        Task<OrganizationCMSTeamListRp> GetOrganizationConfigurationManagementServiceTeams(Guid organizationId, Guid organizationCMSId);
        Task<OrganizationCMSProjectListRp> GetOrganizationConfigurationManagementServiceProjects(Guid organizationId, Guid organizationCMSId);
        Task<OrganizationCMSRepositoryListRp> GetOrganizationConfigurationManagementServiceRepositories(Guid organizationId, Guid organizationCMSId, string projectId);
        Task<OrganizationCMSBranchListRp> GetOrganizationConfigurationManagementServiceRepositoriesBranches(Guid organizationId, Guid organizationCMSId, string projectId, string repositoryId);

    }
}
