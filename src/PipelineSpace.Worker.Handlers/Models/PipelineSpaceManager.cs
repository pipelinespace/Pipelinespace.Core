using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class GetQueueOptions
    {
        public ConfigurationManagementService CMSType { get; set; }

        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public string ProjectName { get; set; }

        public string AgentPoolId { get; set; }
    }

    public class GetQueueResult
    {
        public int QueueId { get; set; }
        public string QueueName { get; set; }
        public int PoolId { get; set; }
        public string PoolName { get; set; }
    }

    public class CreateRepositoryOptions
    {
        public string OrganizationName { get; set; }
        public string ProjectName { get; set; }
        public string ServiceName { get; set; }
        public string RepositoryName { get; set; }

        public string VSTSAccessId { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSRepositoryTemplateUrl { get; set; }
        public string VSTSRepositoryTemplatePath { get; set; }


        public ConfigurationManagementService GitProviderType { get; set; }
        public string GitProviderAccessId { get; set; }
        public string GitProviderAccessSecret { get; set; }
        public string GitProviderAccessToken { get; set; }
        public string GitProviderRepositoryUrl { get; set; }

        public string Branch { get; set; }
        
        public TemplateAccess TemplateAccess { get; set; }
        public bool NeedCredentials { get; set; }
        public ConfigurationManagementService RepositoryCMSType { get; set; }
        public string RepositoryAccessId { get; set; }
        public string RepositoryAccessSecret { get; set; }
        public string RepositoryAccessToken { get; set; }
    }

    public class CreateBranchOptions
    {
        public string FeatureName { get; set; }
        public bool IsImported { get; set; }
        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessId { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSRepositoryTemplateUrl { get; set; }

        public string ProjectName { get; set; }
        public string ServiceName { get; set; }

        public string ServiceExternalId { get; set; }

        public ConfigurationManagementService GitProviderType { get; set; }
        public string GitProviderAccessId { get; set; }
        public string GitProviderAccessSecret { get; set; }
        public string GitProviderAccessToken { get; set; }
        public string GitProviderRepositoryUrl { get; set; }

        public TemplateAccess TemplateAccess { get; set; }
        public bool NeedCredentials { get; set; }
        public ConfigurationManagementService RepositoryCMSType { get; set; }
        public string RepositoryAccessId { get; set; }
        public string RepositoryAccessSecret { get; set; }
        public string RepositoryAccessToken { get; set; }
    }
    
    public class DeleteBranchOptions
    {
        public string FeatureName { get; set; }

        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessId { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSRepositoryTemplateUrl { get; set; }

        public string ProjectName { get; set; }
        public string ServiceName { get; set; }

        public string ServiceExternalId { get; set; }

        public ConfigurationManagementService GitProviderType { get; set; }
        public string GitProviderAccessId { get; set; }
        public string GitProviderAccessSecret { get; set; }
        public string GitProviderAccessToken { get; set; }
        public string GitProviderRepositoryUrl { get; set; }
    }

    public class CreateBuildDefinitionOptions
    {
        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public string ProjectName { get; set; }
        public string ServiceName { get; set; }
        public string CommitStageName { get; set; }

        public string ProjectExternalGitEndpoint { get; set; }

        public ConfigurationManagementService GitProviderType { get; set; }
        public string GitProviderAccountId { get; set; }
        public string GitProviderAccountName { get; set; }
        public string GitProviderAccessId { get; set; }
        public string GitProviderAccessSecret { get; set; }
        public string GitProviderAccessToken { get; set; }

        public string GitProviderRepositoryId { get; set; }
        public string GitProviderRepositoryUrl { get; set; }
        public string GitProviderRepositoryBranch { get; set; }

        public int QueueId { get; set; }
        public string QueueName { get; set; }
        public int PoolId { get; set; }
        public string PoolName { get; set; }
        public string YamlFilename { get; set; }
    }

    public class CreateReleaseDefinitionOptions
    {
        public int? BaseReleaseStageId { get; set; }

        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public string OrganizationName { get; set; }
        public string ProjectName { get; set; }
        public string ServiceName { get; set; }

        public string CommitStageName { get; set; }
        public string BuildDefinitionName { get; set; }
        public string ReleaseStageName { get; set; }

        public string ReleaseDefinition { get; set; }
        public string CloudProviderEndpointId { get; set; }

        public string ProjectExternalId { get; set; }
        public int CommitStageId { get; set; }
        public int QueueId { get; set; }

        public string CloudProviderAccessId { get; set; }
        public string CloudProviderAccessSecret { get; set; }
        public string CloudProviderAccessRegion { get; set; }

        public string WorkFeature { get; set; }

        public List<ProjectServiceTemplateParameterOptions> TemplateParameters { get; set; }
    }

    public class ProjectServiceTemplateParameterOptions
    {
        public string VariableName { get; set; }
        public string Value { get; set; }
        public string Scope { get; set; }
    }

    public class ReadReleaseDefinitionOptions
    {
        public int ReleaseStageId { get; set; }

        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public string OrganizationName { get; set; }
        public string ProjectName { get; set; }
    }

    public class UpdateReleaseDefinitionOptions
    {
        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public string OrganizationName { get; set; }
        public string ProjectName { get; set; }

        public CMSVSTSReleaseDefinitionReadModel Model { get; set; }
    }

    public class QueueBuildOptions
    {
        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public string ProjectName { get; set; }
        public string ProjectExternalId { get; set; }

        public int QueueId { get; set; }
        public int BuildDefinitionId { get; set; }

        public string SourceBranch { get; set; }
    }

    public class QueueReleaseOptions
    {
        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public int ReleaseDefinitionId { get; set; }
        public string Alias { get; set; }
        public int VersionId { get; set; }
        public string VersionName { get; set; }

        public string Description { get; set; }
    }

    public class DeleteBuildDefinitionOptions
    {
        public string ProjectExternalId { get; set; }
        public int? CommitStageId { get; set; }

        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }
    }

    public class DeleteReleaseDefinitionOptions
    {
        public string ProjectExternalId { get; set; }
        public int? ReleaseStageId { get; set; }

        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }
    }

    public class CreateServiceHookOptions
    {
        public string ProjectExternalId { get; set; }

        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public Guid OrganizationId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ServiceId { get; set; }

        public string EventType { get; set; }
        public string Definition { get; set; }
        public string Url { get; set; }

        //Code
        public string Branch { get; set; }
        public string Repository { get; set; }
    }

    public class DeleteServiceHookOptions
    {
        public string ProjectExternalId { get; set; }

        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public Guid ServiceHookId { get; set; }

        public string EventType { get; set; }
    }

    public class CreateOrganizationRepositoryOptions
    {
        public string VSTSAccessId { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSRepositoryTemplateUrl { get; set; }

        public TemplateAccess TemplateAccess { get; set; }
        public bool NeedCredentials { get; set; }
        public ConfigurationManagementService RepositoryCMSType { get; set; }
        public string RepositoryAccessId { get; set; }
        public string RepositoryAccessSecret { get; set; }
        public string RepositoryAccessToken { get; set; }
        public string RepositoryUrl { get; set; }
        public string Branch { get; set; }
    }
}
