using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSVSTSServiceHookModel
    {
        public Guid Id { get; set; }
    }

    public class CMSVSTSServiceHookParamsModel
    {
        public string ProjectId { get; set; }
        public string Definition { get; set; }
        public string Url { get; set; }

        //Code
        public string Branch { get; set; }
        public string Repository { get; set; }

    }

    public class CMSVSTSServiceHookCodeCreateModel : CMSVSTSServiceHookCreateBaseModel
    {
        [JsonProperty("publisherInputs")]
        public CMSVSTSServiceHookCodePublisherInputCreateModel PublisherInputs { get; set; }

        public CMSVSTSServiceHookCodeCreateModel Build(CMSVSTSServiceHookParamsModel @params)
        {
            var @base = this.BuildBase(@params);

            var entity = new CMSVSTSServiceHookCodeCreateModel();
            entity.ConsumerActionId = @base.ConsumerActionId;
            entity.ConsumerId = @base.ConsumerId;
            entity.CustomerInput = @base.CustomerInput;
            entity.Scope = @base.Scope;

            entity.EventType = "git.push";
            entity.PublisherId = "tfs";
            entity.ResourceVersion = "1.0";
            entity.PublisherInputs = new CMSVSTSServiceHookCodePublisherInputCreateModel()
            {
                Branch = @params.Branch,
                PushedBy = "",
                Repository = @params.Repository,
                ProjectId = @params.ProjectId
            };

            return entity;
        }
    }

    public class CMSVSTSServiceHookBuildCreateModel : CMSVSTSServiceHookCreateBaseModel
    {
        [JsonProperty("publisherInputs")]
        public CMSVSTSServiceHookBuildPublisherInputCreateModel PublisherInputs { get; set; }

        public CMSVSTSServiceHookBuildCreateModel Build(CMSVSTSServiceHookParamsModel @params)
        {
            var @base = this.BuildBase(@params);

            var entity = new CMSVSTSServiceHookBuildCreateModel();
            entity.ConsumerActionId = @base.ConsumerActionId;
            entity.ConsumerId = @base.ConsumerId;
            entity.CustomerInput = @base.CustomerInput;
            entity.Scope = @base.Scope;

            entity.EventType = "build.complete";
            entity.PublisherId = "tfs";
            entity.ResourceVersion = "1.0";
            entity.PublisherInputs = new CMSVSTSServiceHookBuildPublisherInputCreateModel()
            {
                BuildStatus = "",
                DefinitionName = @params.Definition,
                ProjectId = @params.ProjectId
            };

            return entity;
        }
    }

    public class CMSVSTSServiceHookReleaseStartedCreateModel : CMSVSTSServiceHookCreateBaseModel
    {
        [JsonProperty("publisherInputs")]
        public CMSVSTSServiceHookReleaseStartedPublisherInputCreateModel PublisherInputs { get; set; }

        public CMSVSTSServiceHookReleaseStartedCreateModel Build(CMSVSTSServiceHookParamsModel @params)
        {
            var @base = this.BuildBase(@params);

            var entity = new CMSVSTSServiceHookReleaseStartedCreateModel();
            entity.ConsumerActionId = @base.ConsumerActionId;
            entity.ConsumerId = @base.ConsumerId;
            entity.CustomerInput = @base.CustomerInput;
            entity.Scope = @base.Scope;

            entity.EventType = "ms.vss-release.deployment-started-event";
            entity.PublisherId = "rm";
            entity.ResourceVersion = "3.0-preview.1";
            entity.PublisherInputs = new CMSVSTSServiceHookReleaseStartedPublisherInputCreateModel()
            {
                ReleaseDefinitionId = @params.Definition,
                ReleaseEnvironmentId = "",
                ProjectId = @params.ProjectId
            };

            return entity;
        }
    }
    
    public class CMSVSTSServiceHookReleasePendingApprovalCreateModel : CMSVSTSServiceHookCreateBaseModel
    {
        [JsonProperty("publisherInputs")]
        public CMSVSTSServiceHookReleasePendingApprovalPublisherInputCreateModel PublisherInputs { get; set; }

        public CMSVSTSServiceHookReleasePendingApprovalCreateModel Build(CMSVSTSServiceHookParamsModel @params)
        {
            var @base = this.BuildBase(@params);

            var entity = new CMSVSTSServiceHookReleasePendingApprovalCreateModel();
            entity.ConsumerActionId = @base.ConsumerActionId;
            entity.ConsumerId = @base.ConsumerId;
            entity.CustomerInput = @base.CustomerInput;
            entity.Scope = @base.Scope;

            entity.EventType = "ms.vss-release.deployment-approval-pending-event";
            entity.PublisherId = "rm";
            entity.ResourceVersion = "3.0-preview.1";
            entity.PublisherInputs = new CMSVSTSServiceHookReleasePendingApprovalPublisherInputCreateModel()
            {
                ReleaseDefinitionId = @params.Definition,
                ReleaseEnvironmentId = "",
                ReleaseApprovalType = "",
                ProjectId = @params.ProjectId
            };

            return entity;
        }
    }

    public class CMSVSTSServiceHookReleaseCompletedApprovalCreateModel : CMSVSTSServiceHookCreateBaseModel
    {
        [JsonProperty("publisherInputs")]
        public CMSVSTSServiceHookReleaseCompletedApprovalPublisherInputCreateModel PublisherInputs { get; set; }

        public CMSVSTSServiceHookReleaseCompletedApprovalCreateModel Build(CMSVSTSServiceHookParamsModel @params)
        {
            var @base = this.BuildBase(@params);

            var entity = new CMSVSTSServiceHookReleaseCompletedApprovalCreateModel();
            entity.ConsumerActionId = @base.ConsumerActionId;
            entity.ConsumerId = @base.ConsumerId;
            entity.CustomerInput = @base.CustomerInput;
            entity.Scope = @base.Scope;

            entity.EventType = "ms.vss-release.deployment-approval-completed-event";
            entity.PublisherId = "rm";
            entity.ResourceVersion = "3.0-preview.1";
            entity.PublisherInputs = new CMSVSTSServiceHookReleaseCompletedApprovalPublisherInputCreateModel()
            {
                ReleaseDefinitionId = @params.Definition,
                ReleaseEnvironmentId = "",
                ReleaseApprovalType = "",
                ReleaseApprovalStatus = "",
                ProjectId = @params.ProjectId
            };

            return entity;
        }
    }

    public class CMSVSTSServiceHookReleaseCreateModel : CMSVSTSServiceHookCreateBaseModel
    {
        [JsonProperty("publisherInputs")]
        public CMSVSTSServiceHookReleasePublisherInputCreateModel PublisherInputs { get; set; }

        public CMSVSTSServiceHookReleaseCreateModel Build(CMSVSTSServiceHookParamsModel @params)
        {
            var @base = this.BuildBase(@params);

            var entity = new CMSVSTSServiceHookReleaseCreateModel();
            entity.ConsumerActionId = @base.ConsumerActionId;
            entity.ConsumerId = @base.ConsumerId;
            entity.CustomerInput = @base.CustomerInput;
            entity.Scope = @base.Scope;

            entity.EventType = "ms.vss-release.deployment-completed-event";
            entity.PublisherId = "rm";
            entity.ResourceVersion = "3.0-preview.1";
            entity.PublisherInputs = new CMSVSTSServiceHookReleasePublisherInputCreateModel()
            {
                ReleaseDefinitionId = @params.Definition,
                ReleaseEnvironmentId = "",
                ReleaseEnvironmentStatus = "",
                ProjectId = @params.ProjectId
            };

            return entity;
        }
    }

    public class CMSVSTSServiceHookCreateBaseModel
    {
        [JsonProperty("consumerActionId")]
        public string ConsumerActionId { get; set; }

        [JsonProperty("consumerId")]
        public string ConsumerId { get; set; }

        [JsonProperty("consumerInputs")]
        public CMSVSTSServiceHookCustomerInputCreateModel CustomerInput { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        [JsonProperty("resourceVersion")]
        public string ResourceVersion { get; set; }

        [JsonProperty("scope")]
        public int Scope { get; set; }
        
        public CMSVSTSServiceHookCreateBaseModel BuildBase(CMSVSTSServiceHookParamsModel @params)
        {
            var entity = new CMSVSTSServiceHookCreateBaseModel()
            {
                ConsumerActionId = "httpRequest",
                ConsumerId = "webHooks",
                CustomerInput = new CMSVSTSServiceHookCustomerInputCreateModel()
                {
                    Url = @params.Url
                },
                Scope = 1
            };

            return entity;
        }
    }

    public class CMSVSTSServiceHookCustomerInputCreateModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class CMSVSTSServiceHookCodePublisherInputCreateModel
    {
        [JsonProperty("branch")]
        public string Branch { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("pushedBy")]
        public string PushedBy { get; set; }

        [JsonProperty("repository")]
        public string Repository { get; set; }
    }

    public class CMSVSTSServiceHookBuildPublisherInputCreateModel
    {
        [JsonProperty("buildStatus")]
        public string BuildStatus { get; set; }

        [JsonProperty("definitionName")]
        public string DefinitionName { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }
    }

    public class CMSVSTSServiceHookReleasePublisherInputCreateModel
    {
        [JsonProperty("releaseDefinitionId")]
        public string ReleaseDefinitionId { get; set; }

        [JsonProperty("releaseEnvironmentId")]
        public string ReleaseEnvironmentId { get; set; }

        [JsonProperty("releaseEnvironmentStatus")]
        public string ReleaseEnvironmentStatus { get; set; }
        
        [JsonProperty("projectId")]
        public string ProjectId { get; set; }
    }

    public class CMSVSTSServiceHookReleaseStartedPublisherInputCreateModel
    {
        [JsonProperty("releaseDefinitionId")]
        public string ReleaseDefinitionId { get; set; }

        [JsonProperty("releaseEnvironmentId")]
        public string ReleaseEnvironmentId { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }
    }

    public class CMSVSTSServiceHookReleasePendingApprovalPublisherInputCreateModel
    {
        [JsonProperty("releaseDefinitionId")]
        public string ReleaseDefinitionId { get; set; }

        [JsonProperty("releaseEnvironmentId")]
        public string ReleaseEnvironmentId { get; set; }

        [JsonProperty("releaseApprovalType")]
        public string ReleaseApprovalType { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }
    }

    public class CMSVSTSServiceHookReleaseCompletedApprovalPublisherInputCreateModel
    {
        [JsonProperty("releaseDefinitionId")]
        public string ReleaseDefinitionId { get; set; }

        [JsonProperty("releaseEnvironmentId")]
        public string ReleaseEnvironmentId { get; set; }

        [JsonProperty("releaseApprovalType")]
        public string ReleaseApprovalType { get; set; }

        [JsonProperty("releaseApprovalStatus")]
        public string ReleaseApprovalStatus { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }
    }
    

}
