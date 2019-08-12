using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSVSTSServiceBuildDefinitionModel
    {
        public int Id { get; set; }
    }

    public class CMSVSTSBuildDefinitionParamsModel
    {
        public string Name { get; set; }

        public string ServiceExternalId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceUrl { get; set; }
        public string Branch { get; set; }
        
        public string ServiceEndpointId { get; set; }

        public int QueueId { get; set; }
        public string QueueName { get; set; }

        public int PoolId { get; set; }
        public string PoolName { get; set; }
        public string RepositoryType { get; set; }
        
        public string ApiUrl { get; set; }
        public string BranchesUrl { get; set; }
        public string CloneUrl { get; set; }
        public string ConnectedServiceId { get; set; }
        public string DefaultBranch { get; set; }
        public string FullName { get; set; }
        public bool IsFork { get; set; }
        public bool IsPrivate { get; set; }
        public string LastUpdated { get; set; }
        public string ManageUrl { get; set; }
        public string OwnerAvatarUrl { get; set; }
        public string OwnerIsAUser { get; set; }
        public string RefsUrl { get; set; }
        public string SafeOwnerId { get; set; }
        public string SafeRepository { get; set; }

        public string YamlFilename { get; set; }
    }

    public class CMSVSTSServiceBuildDefinitionCreateModel
    {
        [JsonProperty("process")]
        public CMSVSTSServiceBuildDefinitionProcessCreateModel Process { get; set; }

        [JsonProperty("repository")]
        public CMSVSTSServiceBuildDefinitionRepositoryCreateModel Repository { get; set; }

        [JsonProperty("queue")]
        public CMSVSTSServiceBuildDefinitionQueueCreateModel Queue { get; set; }

        [JsonProperty("triggers")]
        public List<CMSVSTSServiceBuildDefinitionTriggerCreateModel> Triggers { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("queueStatus")]
        public string QueueStatus { get; set; }

        public static class Factory
        {
            public static CMSVSTSServiceBuildDefinitionCreateModel Create(CMSVSTSBuildDefinitionParamsModel @params)
            {
                var entity = new CMSVSTSServiceBuildDefinitionCreateModel()
                {
                    Name = $"{@params.Name}",
                    Type = "build",
                    QueueStatus = "enabled",
                    Process = new CMSVSTSServiceBuildDefinitionProcessCreateModel()
                    {
                        Type = 2,
                        YamlFilename = string.IsNullOrEmpty(@params.YamlFilename) ? "scripts/build.definition.yml" : @params.YamlFilename
                    },
                    Repository = new CMSVSTSServiceBuildDefinitionRepositoryCreateModel()
                    {
                        Type = @params.RepositoryType,
                        Id = @params.ServiceExternalId,
                        Name = @params.ServiceName,
                        Url = @params.ServiceUrl,
                        DefaultBranch = @params.Branch, 
                        Clean = false,
                        checkoutSubmodules = false,
                        Properties = new CMSVSTSServiceBuildDefinitionRepositoryPropertiesCreateModel()
                        {
                            Branch = @params.Branch,
                            DefaultBranch = @params.DefaultBranch,
                            CleanOptions = "0",
                            LabelSources = "0",
                            LabelSourcesFormat = "$(build.buildNumber)",
                            ReportBuildStatus = true,
                            GitLfsSupport = false,
                            SkipSyncSource = false,
                            CheckoutNestedSubmodules = false,
                            FetchDepth = "0",
                            ServiceEndpointId = @params.ServiceEndpointId,
                            FullName = @params.FullName,
                            ApiUrl = @params.ApiUrl,
                            BranchesUrl = @params.BranchesUrl,
                            CloneUrl = @params.CloneUrl,
                            ManageUrl = @params.ManageUrl,
                            RefsUrl = @params.RefsUrl,
                            IsFork = @params.IsFork,
                            IsPrivate = @params.IsPrivate,
                            LastUpdated = @params.LastUpdated,
                            OwnerAvatarUrl = @params.OwnerAvatarUrl,
                            OwnerIsAUser = @params.OwnerIsAUser,
                            SafeOwnerId = @params.SafeOwnerId,
                            SafeRepository = @params.SafeRepository
                        }
                    },
                    Queue = new CMSVSTSServiceBuildDefinitionQueueCreateModel() {
                        Id = @params.QueueId,
                        Name = @params.QueueName,
                        Pool = new CMSVSTSServiceBuildDefinitionQueuePoolCreateModel()
                        {
                            Id = @params.PoolId,
                            Name = @params.PoolName
                        }
                    },
                    Triggers = new List<CMSVSTSServiceBuildDefinitionTriggerCreateModel>() {
                        new CMSVSTSServiceBuildDefinitionTriggerCreateModel()
                        {
                            BatchChanges = false,
                            BranchFilters = new string[] { $"+{@params.Branch}" },
                            MaxConcurrentBuildsPerBranch = 1,
                            PollingInterval = 0,
                            TriggerType = "continuousIntegration"
                        }
                    }
                };

                return entity;
            }
        }
    }

    public class CMSVSTSServiceBuildDefinitionProcessCreateModel
    {
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("yamlFilename")]
        public string YamlFilename { get; set; }
    }

    public class CMSVSTSServiceBuildDefinitionRepositoryCreateModel
    {
        [JsonProperty("properties")]
        public CMSVSTSServiceBuildDefinitionRepositoryPropertiesCreateModel Properties { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("defaultBranch")]
        public string DefaultBranch { get; set; }

        [JsonProperty("clean")]
        public bool Clean { get; set; }

        [JsonProperty("checkoutSubmodules")]
        public bool checkoutSubmodules { get; set; }
    }

    public class CMSVSTSServiceBuildDefinitionRepositoryPropertiesCreateModel
    {
        [JsonProperty("cleanOptions")]
        public string CleanOptions { get; set; }

        [JsonProperty("labelSources")]
        public string LabelSources { get; set; }

        [JsonProperty("labelSourcesFormat")]
        public string LabelSourcesFormat { get; set; }

        [JsonProperty("reportBuildStatus")]
        public bool ReportBuildStatus { get; set; }

        [JsonProperty("gitLfsSupport")]
        public bool GitLfsSupport { get; set; }

        [JsonProperty("skipSyncSource")]
        public bool SkipSyncSource { get; set; }

        [JsonProperty("fullName", NullValueHandling = NullValueHandling.Ignore)]
        public string FullName { get; set; }

        [JsonProperty("checkoutNestedSubmodules")]
        public bool CheckoutNestedSubmodules { get; set; }

        [JsonProperty("fetchDepth")]
        public string FetchDepth { get; set; }

        [JsonProperty("connectedServiceId", NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceEndpointId { get; set; }

        [JsonProperty("branch", NullValueHandling = NullValueHandling.Ignore)]
        public string Branch { get; set; }

        [JsonProperty("defaultBranch", NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultBranch { get; set; }

        /*
                 public string ApiUrl { get; set; }
        public string BranchesUrl { get; set; }
        public string CloneUrl { get; set; }
        public string ConnectedServiceId { get; set; }
        public string DefaultBranch { get; set; }
        public string FullName { get; set; }
        public bool IsFork { get; set; }
        public bool IsPrivate { get; set; }
        public string LastUpdated { get; set; }
        public string ManageUrl { get; set; }
        public string OwnerAvatarUrl { get; set; }
        public string OwnerIsAUser { get; set; }
        public string RefsUrl { get; set; }
        public string SafeOwnerId { get; set; }
        public string SafeRepository { get; set; }
             */

        [JsonProperty("apiUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string ApiUrl { get; set; }

        [JsonProperty("branchesUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string BranchesUrl { get; set; }

        [JsonProperty("cloneUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string CloneUrl { get; set; }

        [JsonProperty("manageUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string ManageUrl { get; set; }

        [JsonProperty("refsUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string RefsUrl { get; set; }

        [JsonProperty("isFork", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsFork { get; set; }

        [JsonProperty("isPrivate", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsPrivate { get; set; }

        [JsonProperty("lastUpdated", NullValueHandling = NullValueHandling.Ignore)]
        public string LastUpdated { get; set; }
        
        [JsonProperty("ownerAvatarUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerAvatarUrl { get; set; }

        [JsonProperty("ownerIsAUser", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerIsAUser { get; set; }

        [JsonProperty("safeOwnerId", NullValueHandling = NullValueHandling.Ignore)]
        public string SafeOwnerId { get; set; }

        [JsonProperty("safeRepository", NullValueHandling = NullValueHandling.Ignore)]
        public string SafeRepository { get; set; }
    }

    public class CMSVSTSServiceBuildDefinitionQueueCreateModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pool")]
        public CMSVSTSServiceBuildDefinitionQueuePoolCreateModel Pool { get; set; }
    }

    public class CMSVSTSServiceBuildDefinitionQueuePoolCreateModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class CMSVSTSServiceBuildDefinitionResultModel
    {
        public CMSVSTSServiceBuildDefinitionResultModel()
        {
            this.Success = true;
        }

        public bool Success { get; set; }
        public string ReasonForNoSuccess { get; set; }

        public void Fail(string reason)
        {
            this.Success = false;
            this.ReasonForNoSuccess = reason;
        }
    }

    public class CMSVSTSServiceBuildDefinitionTriggerCreateModel
    {
        [JsonProperty("batchChanges")]
        public bool BatchChanges { get; set; }

        [JsonProperty("branchFilters")]
        public string[] BranchFilters { get; set; }

        [JsonProperty("maxConcurrentBuildsPerBranch")]
        public int MaxConcurrentBuildsPerBranch { get; set; }

        [JsonProperty("pollingInterval")]
        public int PollingInterval { get; set; }

        [JsonProperty("triggerType")]
        public string TriggerType { get; set; }
    }

   
}
