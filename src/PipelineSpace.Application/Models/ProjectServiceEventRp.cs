using Newtonsoft.Json;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipelineSpace.Application.Models
{
    //Build
    public class ProjectServiceEventBuildModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("finishTime")]
        public DateTime FinishTime { get; set; }

        [JsonProperty("buildNumber")]
        public string BuildNumber { get; set; }
    }

    //Release Started
    public class ProjectServiceEventReleaseStartedModel
    {
        [JsonProperty("environment")]
        public ProjectServiceEventReleaseStartedEnvironmentModel Environment { get; set; }

        [JsonProperty("release")]
        public ProjectServiceEventReleaseStartedReleaseModel Release { get; set; }

        public string VersionId
        {
            get
            {
                if (!Release.Artifacts.Any())
                    return string.Empty;

                return Release.Artifacts[0].DefinitionReference.Version.Id;
            }
        }

        public string VersionName
        {
            get
            {
                if (!Release.Artifacts.Any())
                    return string.Empty;

                return Release.Artifacts[0].DefinitionReference.Version.Name;
            }
        }
    }

    public class ProjectServiceEventReleaseStartedEnvironmentModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class ProjectServiceEventReleaseStartedReleaseModel
    {
        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("artifacts")]
        public List<ProjectServiceEventReleaseStartedReleaseArtifactModel> Artifacts { get; set; }

        [JsonProperty("environments")]
        public List<ProjectServiceEventReleaseStartedReleaseEnvironmentModel> Environments { get; set; }
    }

    public class ProjectServiceEventReleaseStartedReleaseEnvironmentModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class ProjectServiceEventReleaseStartedReleaseArtifactModel
    {
        [JsonProperty("definitionReference")]
        public ProjectServiceEventReleaseStartedReleaseArtifactDefinitionReferenceModel DefinitionReference { get; set; }
    }

    public class ProjectServiceEventReleaseStartedReleaseArtifactDefinitionReferenceModel
    {
        [JsonProperty("version")]
        public ProjectServiceEventReleaseStartedReleaseArtifactDefinitionReferenceVersionModel Version { get; set; }
    }

    public class ProjectServiceEventReleaseStartedReleaseArtifactDefinitionReferenceVersionModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    //Release 
    public class ProjectServiceEventReleaseModel
    {
        [JsonProperty("environment")]
        public ProjectServiceEventReleaseEnvironmentModel Environment { get; set; }

        [JsonProperty("deployment")]
        public ProjectServiceEventReleaseDeploymentModel Deployment { get; set; }
    }

    public class ProjectServiceEventReleaseEnvironmentModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class ProjectServiceEventReleaseDeploymentModel
    {
        [JsonProperty("release")]
        public ProjectServiceEventReleaseDeploymentReleaseModel Release { get; set; }

        [JsonProperty("completedOn")]
        public DateTime CompletedOn { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime LastModifiedOn { get; set; }
        
        public string VersionId
        {
            get
            {
                if (!Release.Artifacts.Any())
                    return string.Empty;

                return Release.Artifacts[0].DefinitionReference.Version.Id;
            }
        }

        public string VersionName
        {
            get
            {
                if (!Release.Artifacts.Any())
                    return string.Empty;

                return Release.Artifacts[0].DefinitionReference.Version.Name;
            }
        }
    }

    public class ProjectServiceEventReleaseDeploymentReleaseModel
    {
        [JsonProperty("artifacts")]
        public List<ProjectServiceEventReleaseDeploymentReleaseArtifactModel> Artifacts { get; set; }
    }

    public class ProjectServiceEventReleaseDeploymentReleaseArtifactModel
    {
        [JsonProperty("definitionReference")]
        public ProjectServiceEventReleaseDeploymentReleaseArtifactDefinitionReferenceModel DefinitionReference { get; set; }
    }

    public class ProjectServiceEventReleaseDeploymentReleaseArtifactDefinitionReferenceModel
    {
        [JsonProperty("version")]
        public ProjectServiceEventReleaseDeploymentReleaseArtifactDefinitionReferenceVersionModel Version { get; set; }
    }

    public class ProjectServiceEventReleaseDeploymentReleaseArtifactDefinitionReferenceVersionModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    //Approval
    public class ProjectServiceEventReleaseApprovalModel
    {
        [JsonProperty("release")]
        public ProjectServiceEventReleaseApprovalReleaseBaseModel Release { get; set; }

        [JsonProperty("approval")]
        public ProjectServiceEventReleaseApprovalReleaseModel Approval { get; set; }

        public string VersionId
        {
            get
            {
                if (!Release.Artifacts.Any())
                    return string.Empty;

                return Release.Artifacts[0].DefinitionReference.Version.Id;
            }
        }

        public string VersionName
        {
            get
            {
                if (!Release.Artifacts.Any())
                    return string.Empty;

                return Release.Artifacts[0].DefinitionReference.Version.Name;
            }
        }
    }

    public class ProjectServiceEventReleaseApprovalReleaseBaseModel
    {
        [JsonProperty("artifacts")]
        public List<ProjectServiceEventReleaseApprovalReleaseArtifactBaseModel> Artifacts { get; set; }
    }

    public class ProjectServiceEventReleaseApprovalReleaseArtifactBaseModel
    {
        [JsonProperty("definitionReference")]
        public ProjectServiceEventReleaseApprovalReleaseArtifactBaseDefinitionReferenceModel DefinitionReference { get; set; }
    }

    public class ProjectServiceEventReleaseApprovalReleaseArtifactBaseDefinitionReferenceModel
    {
        [JsonProperty("version")]
        public ProjectServiceEventReleaseApprovalReleaseArtifactBaseDefinitionReferenceVersionModel Version { get; set; }
    }

    public class ProjectServiceEventReleaseApprovalReleaseArtifactBaseDefinitionReferenceVersionModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class ProjectServiceEventReleaseApprovalReleaseModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("releaseEnvironment")]
        public ProjectServiceEventReleaseApprovalEnvironmentModel ReleaseEnvironment { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("modifiedOn")]
        public DateTime ModifiedOn { get; set; }
    }

    public class ProjectServiceEventReleaseApprovalEnvironmentModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
