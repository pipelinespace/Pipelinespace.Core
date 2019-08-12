using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSVSTSServiceReleaseDefinitionModel
    {
        public int Id { get; set; }
    }

    public class CMSVSTSeReleaseDefinitionParamsModel
    {
        public string ReleaseStageName { get; set; }

        public string ProjectExternalId { get; set; }
        public string ProjectName { get; set; }

        public string ServiceName { get; set; }

        public string CommitStageId { get; set; }
        public string CommitStageName { get; set; }

        public string EnvironmentDevelopmentName { get; set; }
        public string EnvironmentProductionName { get; set; }
        
        public object EnvironmentDevelopmentVariables { get; set; }
        public object EnvironmentProductionVariables { get; set; }
        
        public int QueueId { get; set; }

        public CMSVSTSReleaseDefinitionInputModel ReleaseDefinitionInput { get; set; }

        public object Owner { get; set; }
    }

    public class CMSVSTSReleaseDefinitionCreateModel : CMSVSTSReleaseDefinitionModel
    {
        public static class Factory
        {
            public static CMSVSTSReleaseDefinitionCreateModel Create(CMSVSTSeReleaseDefinitionParamsModel @params)
            {
                DateTime createdOn = DateTime.UtcNow;
                var entity = new CMSVSTSReleaseDefinitionCreateModel()
                {
                    Id = 0,
                    Name = @params.ReleaseStageName,
                    Source = "2",
                    Comment = $"Init release definition for {@params.ServiceName}",
                    CreatedOn = createdOn,
                    CreatedBy = null,
                    ModifiedBy = null,
                    ModifiedOn = createdOn,
                    Environments = new List<CMSVSTSReleaseDefinitionEnvironmentCreateModel>()
                    {
                        new CMSVSTSReleaseDefinitionEnvironmentCreateModel()
                        {
                            Id = -1,
                            Name = @params.EnvironmentDevelopmentName,
                            Rank = 1,
                            Variables = @params.EnvironmentDevelopmentVariables,
                            VariableGroups = new List<CMSVSTSReleaseDefinitionEnvironmentVariableGroupCreateModel>(),
                            PreDeployApprovals = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalCreateModel()
                            {
                                Approvals = new List<CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel>()
                                {
                                    new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel()
                                    {
                                        Rank = 1,
                                        IsAutomated = true,
                                        IsNotificationOn = false,
                                        Id = 0
                                    }
                                },
                                ApprovalOptions = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalOptionCreateModel()
                                {
                                    ExecutionOrder = "1"
                                }
                            },
                            DeployStep = new CMSVSTSReleaseDefinitionEnvironmentDeployStepCreateModel()
                            {
                                Tasks = new List<CMSVSTSReleaseDefinitionEnvironmentDeployStepTaskCreateModel>(),
                                Id = 0
                            },
                            PostDeployApprovals = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalCreateModel()
                            {
                                Approvals = new List<CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel>()
                                {
                                    new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel()
                                    {
                                        Rank = 1,
                                        IsAutomated = true,
                                        IsNotificationOn = false,
                                        Id = 0
                                    }
                                },
                                ApprovalOptions = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalOptionCreateModel()
                                {
                                    ExecutionOrder = "2"
                                }
                            },
                            DeployPhases = new List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseCreateModel>()
                            {
                                new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseCreateModel()
                                {
                                    DeploymentInput = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputCreateModel()
                                    {
                                        ParallelExecution = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputParallelExecutionCreateModel()
                                        {
                                            ParallelExecutionType = "0"
                                        },
                                        SkipArtifactsDownload = false,
                                        ArtifactsDownloadInput = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputArtifactsDownloadInputCreateModel(),
                                        QueueId = @params.QueueId,
                                        Demands = new List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputDemandCreateModel>(),
                                        EnableAccessToken = false,
                                        TimeoutInMinutes = 0,
                                        JobCancelTimeoutInMinutes = 1,
                                        //Condition = "succeeded()",
                                        Condition = $"and(not(contains(variables['Release.ReleaseDescription'], 'PS_SKIP_ENVIRONMENT_{@params.EnvironmentDevelopmentName}')), eq(variables['PS_ENVIRONMENT_ENABLE'], 'True'))",
                                        OverrideInputs = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputOverrideCreateModel(),
                                        Dependencies = new List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputDependencyCreateModel>()
                                    },
                                    Rank = 1,
                                    PhaseType = "1",
                                    Name = "Agent phase",
                                    WorkflowTasks = @params.ReleaseDefinitionInput.Tasks.Select(t=> new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskCreateModel()
                                    {
                                        Name = t.Name,
                                        RefName = null,
                                        Enabled = true,
                                        TimeoutInMinutes = 0,
                                        Inputs = t.Inputs,
                                        TaskId = t.TaskId,
                                        Version = t.Version,
                                        DefinitionType = "task",
                                        AlwaysRun = false,
                                        ContinueOnError = false,
                                        OverrideInputs = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskOverrideInputCreateModel(),
                                        Condition = "succeeded()",
                                        Environment = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskEnvironmentCreateModel()
                                    }).ToList(),
                                    PhaseInputs = new CMSVSTSReleaseDefinitionEnvironmentDeployPhasePhaseInputCreateModel()
                                    {
                                        ArtifactDownloadInput = new CMSVSTSReleaseDefinitionEnvironmentDeployPhasePhaseInputArtifactDownloadInputCreateModel()
                                        {
                                            ArtifactsDownloadInput = { },
                                            SkipArtifactsDownload = false
                                    }
                                }
                                }
                            },
                            RunOptions = new CMSVSTSReleaseDefinitionEnvironmentRunOptionCreateModel(),
                            EnvironmentOptions = new CMSVSTSReleaseDefinitionEnvironmentEnvironemtnOptionCreateModel()
                            {
                                EmailNotificationType = "OnlyOnFailure",
                                EmailRecipients = "release.environment.owner;release.creator",
                                SkipArtifactsDownload = false,
                                TimeoutInMinutes = 0,
                                EnableAccessToken = false,
                                PublishDeploymentStatus = true,
                                BadgeEnabled = false,
                                AutoLinkWorkItems = false,
                                PullRequestDeploymentEnabled = false
                            },
                            Demands = new List<CMSVSTSReleaseDefinitionEnvironmentDemandCreateModel>(),
                            Conditions = new List<CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel>()
                            {
                                new CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel()
                                {
                                    ConditionType = "1",
                                    Name = "ReleaseStarted",
                                    Value = string.Empty
                                }
                            },
                            ExecutionPolicy = new CMSVSTSReleaseDefinitionEnvironmentExecutionPolicyCreateModel()
                            {
                                ConcurrencyCount = 1,
                                QueueDepthCount = 1
                            },
                            Schedules = new List<CMSVSTSReleaseDefinitionEnvironmentScheduleCreateModel>(),
                            Properties = new CMSVSTSReleaseDefinitionEnvironmentPropertiesCreateModel(),
                            PreDeploymentGates = new CMSVSTSReleaseDefinitionEnvironmentDeploymentGateCreateModel()
                            {
                                Id = 0,
                                GatesOptions = null,
                                Gates = new List<CMSVSTSReleaseDefinitionEnvironmentDeploymentGateItemCreateModel>()
                            },
                            PostDeploymentGates = new CMSVSTSReleaseDefinitionEnvironmentDeploymentGateCreateModel()
                            {
                                Id = 0,
                                GatesOptions = null,
                                Gates = new List<CMSVSTSReleaseDefinitionEnvironmentDeploymentGateItemCreateModel>()
                            },
                            EnvironmentTriggers = new List<CMSVSTSReleaseDefinitionEnvironmentTriggerCreateModel>(),
                            RetentionPolicy = new CMSVSTSReleaseDefinitionEnvironmentRetentionPolicyCreateModel()
                            {
                                DaysToKeep = 30,
                                ReleasesToKeep = 3,
                                RetainBuild = true
                            },
                            ProcessParameters = new CMSVSTSReleaseDefinitionEnvironmentProcessParameterCreateModel()
                        },
                        new CMSVSTSReleaseDefinitionEnvironmentCreateModel()
                        {
                            Id = -2,
                            Name = @params.EnvironmentProductionName,
                            Rank = 2,
                            Variables = @params.EnvironmentProductionVariables,
                            VariableGroups = new List<CMSVSTSReleaseDefinitionEnvironmentVariableGroupCreateModel>(),
                            PreDeployApprovals = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalCreateModel()
                            {
                                Approvals = new List<CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel>()
                                {
                                    new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel()
                                    {
                                        IsAutomated = false,
                                        IsNotificationOn = false,
                                        Rank = 1,
                                        Approver = @params.Owner
                                    }
                                },
                                ApprovalOptions = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalOptionCreateModel()
                                {
                                    AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped = false,
                                    EnforceIdentityRevalidation = false,
                                    ExecutionOrder = "1",
                                    releaseCreatorCanBeApprover = true,
                                    TimeoutInMinutes = 0
                                }
                            },
                            DeployStep = new CMSVSTSReleaseDefinitionEnvironmentDeployStepCreateModel()
                            {
                                Tasks = new List<CMSVSTSReleaseDefinitionEnvironmentDeployStepTaskCreateModel>(),
                                Id = 0
                            },
                            PostDeployApprovals = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalCreateModel()
                            {
                                Approvals = new List<CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel>()
                                {
                                    new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel()
                                    {
                                        Rank = 1,
                                        IsAutomated = true,
                                        IsNotificationOn = false,
                                        Id = 0
                                    }
                                },
                                ApprovalOptions = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalOptionCreateModel()
                                {
                                    ExecutionOrder = "2"
                                }
                            },
                            DeployPhases = new List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseCreateModel>()
                            {
                                new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseCreateModel()
                                {
                                    DeploymentInput = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputCreateModel()
                                    {
                                        ParallelExecution = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputParallelExecutionCreateModel()
                                        {
                                            ParallelExecutionType = "0"
                                        },
                                        SkipArtifactsDownload = false,
                                        ArtifactsDownloadInput = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputArtifactsDownloadInputCreateModel(),
                                        QueueId = @params.QueueId,
                                        Demands = new List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputDemandCreateModel>(),
                                        EnableAccessToken = false,
                                        TimeoutInMinutes = 0,
                                        JobCancelTimeoutInMinutes = 1,
                                        Condition = $"and(not(contains(variables['Release.ReleaseDescription'], 'PS_SKIP_ENVIRONMENT_{@params.EnvironmentProductionName}')), eq(variables['PS_ENVIRONMENT_ENABLE'], 'True'))",
                                        OverrideInputs = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputOverrideCreateModel(),
                                        Dependencies = new List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputDependencyCreateModel>()
                                    },
                                    Rank = 1,
                                    PhaseType = "1",
                                    Name = "Agent phase",
                                    WorkflowTasks = @params.ReleaseDefinitionInput.Tasks.Select(t=> new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskCreateModel()
                                    {
                                        Name = t.Name,
                                        RefName = null,
                                        Enabled = true,
                                        TimeoutInMinutes = 0,
                                        Inputs = t.Inputs,
                                        TaskId = t.TaskId,
                                        Version = t.Version,
                                        DefinitionType = "task",
                                        AlwaysRun = false,
                                        ContinueOnError = false,
                                        OverrideInputs = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskOverrideInputCreateModel(),
                                        Condition = "succeeded()",
                                        Environment = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskEnvironmentCreateModel()
                                    }).ToList(),
                                    PhaseInputs = new CMSVSTSReleaseDefinitionEnvironmentDeployPhasePhaseInputCreateModel()
                                    {
                                        ArtifactDownloadInput = new CMSVSTSReleaseDefinitionEnvironmentDeployPhasePhaseInputArtifactDownloadInputCreateModel()
                                        {
                                            ArtifactsDownloadInput = { },
                                            SkipArtifactsDownload = false
                                    }
                                }
                                }
                            },
                            RunOptions = new CMSVSTSReleaseDefinitionEnvironmentRunOptionCreateModel(),
                            EnvironmentOptions = new CMSVSTSReleaseDefinitionEnvironmentEnvironemtnOptionCreateModel()
                            {
                                EmailNotificationType = "OnlyOnFailure",
                                EmailRecipients = "release.environment.owner;release.creator",
                                SkipArtifactsDownload = false,
                                TimeoutInMinutes = 0,
                                EnableAccessToken = false,
                                PublishDeploymentStatus = true,
                                BadgeEnabled = false,
                                AutoLinkWorkItems = false,
                                PullRequestDeploymentEnabled = false
                            },
                            Demands = new List<CMSVSTSReleaseDefinitionEnvironmentDemandCreateModel>(),
                            Conditions = new List<CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel>()
                            {
                                new CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel()
                                {
                                    ConditionType = "2",
                                    EnvironmentId = -1,
                                    Name = @params.EnvironmentDevelopmentName,
                                    Value = "4"
                                }
                            },
                            ExecutionPolicy = new CMSVSTSReleaseDefinitionEnvironmentExecutionPolicyCreateModel()
                            {
                                ConcurrencyCount = 1,
                                QueueDepthCount = 1
                            },
                            Schedules = new List<CMSVSTSReleaseDefinitionEnvironmentScheduleCreateModel>(),
                            Properties = new CMSVSTSReleaseDefinitionEnvironmentPropertiesCreateModel(),
                            PreDeploymentGates = new CMSVSTSReleaseDefinitionEnvironmentDeploymentGateCreateModel()
                            {
                                Id = 0,
                                GatesOptions = null,
                                Gates = new List<CMSVSTSReleaseDefinitionEnvironmentDeploymentGateItemCreateModel>()
                            },
                            PostDeploymentGates = new CMSVSTSReleaseDefinitionEnvironmentDeploymentGateCreateModel()
                            {
                                Id = 0,
                                GatesOptions = null,
                                Gates = new List<CMSVSTSReleaseDefinitionEnvironmentDeploymentGateItemCreateModel>()
                            },
                            EnvironmentTriggers = new List<CMSVSTSReleaseDefinitionEnvironmentTriggerCreateModel>(),
                            RetentionPolicy = new CMSVSTSReleaseDefinitionEnvironmentRetentionPolicyCreateModel()
                            {
                                DaysToKeep = 30,
                                ReleasesToKeep = 3,
                                RetainBuild = true
                            },
                            ProcessParameters = new CMSVSTSReleaseDefinitionEnvironmentProcessParameterCreateModel()
                        }
                    },
                    Artifacts = new List<CMSVSTSReleaseDefinitionArtifactCreateModel>()
                    {
                        new CMSVSTSReleaseDefinitionArtifactCreateModel()
                        {
                            Type = "Build",
                            DefinitionReference = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceCreateModel()
                            {
                                Project = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = @params.ProjectName,
                                    Id = @params.ProjectExternalId
                                },
                                Definition = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = @params.CommitStageName,
                                    Id = @params.CommitStageId
                                },
                                DefaultVersionType = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = "Latest",
                                    Id = "latestType"
                                },
                                DefaultVersionBranch = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = string.Empty,
                                    Id = string.Empty
                                },
                                DefaultVersionTags = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = string.Empty,
                                    Id = string.Empty
                                },
                                DefaultVersionSpecific = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = string.Empty,
                                    Id = string.Empty,
                                },
                                ArtifactSourceDefinitionUrl = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = string.Empty,
                                    Id = string.Empty,
                                }
                            },
                            Alias = @params.CommitStageName,
                            IsPrimary = true,
                            SourceId = string.Empty,
                            IsRetained = false
                        }
                    },
                    Variables = @params.ReleaseDefinitionInput.Variables,
                    VariableGroups = new List<CMSVSTSReleaseDefinitionVariableGroupCreateModel>(),
                    Triggers = new List<CMSVSTSReleaseDefinitionTriggerCreateModel>()
                    {
                        new CMSVSTSReleaseDefinitionTriggerCreateModel()
                        {
                            TriggerType = "1",
                            TriggerConditions = null,
                            ArtifactAlias = @params.CommitStageName
                        }
                    },
                    LastRelease = null,
                    Tags = new List<CMSVSTSReleaseDefinitionTagCreateModel>(),
                    Path = "\\\\",
                    Properties = new CMSVSTSReleaseDefinitionPropertiesCreateModel()
                    {
                        DefinitionCreationSource = new CMSVSTSReleaseDefinitionPropertiesItemCreateModel()
                        {
                            Type = "System.String",
                            Value = "ReleaseNew"
                        }
                    },
                    ReleaseNameFormat = "$(Date:yyyyMMdd)$(Rev:.r)",
                    Description = string.Empty
                };

                return entity;
            }

            public static CMSVSTSReleaseDefinitionCreateModel CreateForFeature(CMSVSTSeReleaseDefinitionParamsModel @params)
            {
                DateTime createdOn = DateTime.UtcNow;
                var entity = new CMSVSTSReleaseDefinitionCreateModel()
                {
                    Id = 0,
                    Name = @params.ReleaseStageName,
                    Source = "2",
                    Comment = $"Init release definition for {@params.ServiceName}",
                    CreatedOn = createdOn,
                    CreatedBy = null,
                    ModifiedBy = null,
                    ModifiedOn = createdOn,
                    Environments = new List<CMSVSTSReleaseDefinitionEnvironmentCreateModel>()
                    {
                        new CMSVSTSReleaseDefinitionEnvironmentCreateModel()
                        {
                            Id = -1,
                            Name = @params.EnvironmentDevelopmentName,
                            Rank = 1,
                            Variables = @params.EnvironmentDevelopmentVariables,
                            VariableGroups = new List<CMSVSTSReleaseDefinitionEnvironmentVariableGroupCreateModel>(),
                            PreDeployApprovals = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalCreateModel()
                            {
                                Approvals = new List<CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel>()
                                {
                                    new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel()
                                    {
                                        Rank = 1,
                                        IsAutomated = true,
                                        IsNotificationOn = false,
                                        Id = 0
                                    }
                                },
                                ApprovalOptions = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalOptionCreateModel()
                                {
                                    ExecutionOrder = "1"
                                }
                            },
                            DeployStep = new CMSVSTSReleaseDefinitionEnvironmentDeployStepCreateModel()
                            {
                                Tasks = new List<CMSVSTSReleaseDefinitionEnvironmentDeployStepTaskCreateModel>(),
                                Id = 0
                            },
                            PostDeployApprovals = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalCreateModel()
                            {
                                Approvals = new List<CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel>()
                                {
                                    new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel()
                                    {
                                        Rank = 1,
                                        IsAutomated = true,
                                        IsNotificationOn = false,
                                        Id = 0
                                    }
                                },
                                ApprovalOptions = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalOptionCreateModel()
                                {
                                    ExecutionOrder = "2"
                                }
                            },
                            DeployPhases = new List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseCreateModel>()
                            {
                                new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseCreateModel()
                                {
                                    DeploymentInput = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputCreateModel()
                                    {
                                        ParallelExecution = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputParallelExecutionCreateModel()
                                        {
                                            ParallelExecutionType = "0"
                                        },
                                        SkipArtifactsDownload = false,
                                        ArtifactsDownloadInput = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputArtifactsDownloadInputCreateModel(),
                                        QueueId = @params.QueueId,
                                        Demands = new List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputDemandCreateModel>(),
                                        EnableAccessToken = false,
                                        TimeoutInMinutes = 0,
                                        JobCancelTimeoutInMinutes = 1,
                                        Condition = "succeeded()",
                                        OverrideInputs = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputOverrideCreateModel(),
                                        Dependencies = new List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputDependencyCreateModel>()
                                    },
                                    Rank = 1,
                                    PhaseType = "1",
                                    Name = "Agent phase",
                                    WorkflowTasks = @params.ReleaseDefinitionInput.Tasks.Select(t=> new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskCreateModel()
                                    {
                                        Name = t.Name,
                                        RefName = null,
                                        Enabled = true,
                                        TimeoutInMinutes = 0,
                                        Inputs = t.Inputs,
                                        TaskId = t.TaskId,
                                        Version = t.Version,
                                        DefinitionType = "task",
                                        AlwaysRun = false,
                                        ContinueOnError = false,
                                        OverrideInputs = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskOverrideInputCreateModel(),
                                        Condition = "succeeded()",
                                        Environment = new CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskEnvironmentCreateModel()
                                    }).ToList(),
                                    PhaseInputs = new CMSVSTSReleaseDefinitionEnvironmentDeployPhasePhaseInputCreateModel()
                                    {
                                        ArtifactDownloadInput = new CMSVSTSReleaseDefinitionEnvironmentDeployPhasePhaseInputArtifactDownloadInputCreateModel()
                                        {
                                            ArtifactsDownloadInput = { },
                                            SkipArtifactsDownload = false
                                    }
                                }
                                }
                            },
                            RunOptions = new CMSVSTSReleaseDefinitionEnvironmentRunOptionCreateModel(),
                            EnvironmentOptions = new CMSVSTSReleaseDefinitionEnvironmentEnvironemtnOptionCreateModel()
                            {
                                EmailNotificationType = "OnlyOnFailure",
                                EmailRecipients = "release.environment.owner;release.creator",
                                SkipArtifactsDownload = false,
                                TimeoutInMinutes = 0,
                                EnableAccessToken = false,
                                PublishDeploymentStatus = true,
                                BadgeEnabled = false,
                                AutoLinkWorkItems = false,
                                PullRequestDeploymentEnabled = false
                            },
                            Demands = new List<CMSVSTSReleaseDefinitionEnvironmentDemandCreateModel>(),
                            Conditions = new List<CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel>()
                            {
                                new CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel()
                                {
                                    ConditionType = "1",
                                    Name = "ReleaseStarted",
                                    Value = string.Empty
                                }
                            },
                            ExecutionPolicy = new CMSVSTSReleaseDefinitionEnvironmentExecutionPolicyCreateModel()
                            {
                                ConcurrencyCount = 1,
                                QueueDepthCount = 1
                            },
                            Schedules = new List<CMSVSTSReleaseDefinitionEnvironmentScheduleCreateModel>(),
                            Properties = new CMSVSTSReleaseDefinitionEnvironmentPropertiesCreateModel(),
                            PreDeploymentGates = new CMSVSTSReleaseDefinitionEnvironmentDeploymentGateCreateModel()
                            {
                                Id = 0,
                                GatesOptions = null,
                                Gates = new List<CMSVSTSReleaseDefinitionEnvironmentDeploymentGateItemCreateModel>()
                            },
                            PostDeploymentGates = new CMSVSTSReleaseDefinitionEnvironmentDeploymentGateCreateModel()
                            {
                                Id = 0,
                                GatesOptions = null,
                                Gates = new List<CMSVSTSReleaseDefinitionEnvironmentDeploymentGateItemCreateModel>()
                            },
                            EnvironmentTriggers = new List<CMSVSTSReleaseDefinitionEnvironmentTriggerCreateModel>(),
                            RetentionPolicy = new CMSVSTSReleaseDefinitionEnvironmentRetentionPolicyCreateModel()
                            {
                                DaysToKeep = 30,
                                ReleasesToKeep = 3,
                                RetainBuild = true
                            },
                            ProcessParameters = new CMSVSTSReleaseDefinitionEnvironmentProcessParameterCreateModel()
                        }
                    },
                    Artifacts = new List<CMSVSTSReleaseDefinitionArtifactCreateModel>()
                    {
                        new CMSVSTSReleaseDefinitionArtifactCreateModel()
                        {
                            Type = "Build",
                            DefinitionReference = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceCreateModel()
                            {
                                Project = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = @params.ProjectName,
                                    Id = @params.ProjectExternalId
                                },
                                Definition = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = @params.CommitStageName,
                                    Id = @params.CommitStageId
                                },
                                DefaultVersionType = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = "Latest",
                                    Id = "latestType"
                                },
                                DefaultVersionBranch = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = string.Empty,
                                    Id = string.Empty
                                },
                                DefaultVersionTags = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = string.Empty,
                                    Id = string.Empty
                                },
                                DefaultVersionSpecific = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = string.Empty,
                                    Id = string.Empty,
                                },
                                ArtifactSourceDefinitionUrl = new CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel()
                                {
                                    Name = string.Empty,
                                    Id = string.Empty,
                                }
                            },
                            Alias = @params.CommitStageName,
                            IsPrimary = true,
                            SourceId = string.Empty,
                            IsRetained = false
                        }
                    },
                    Variables = @params.ReleaseDefinitionInput.Variables,
                    VariableGroups = new List<CMSVSTSReleaseDefinitionVariableGroupCreateModel>(),
                    Triggers = new List<CMSVSTSReleaseDefinitionTriggerCreateModel>()
                    {
                        new CMSVSTSReleaseDefinitionTriggerCreateModel()
                        {
                            TriggerType = "1",
                            TriggerConditions = null,
                            ArtifactAlias = @params.CommitStageName
                        }
                    },
                    LastRelease = null,
                    Tags = new List<CMSVSTSReleaseDefinitionTagCreateModel>(),
                    Path = "\\\\",
                    Properties = new CMSVSTSReleaseDefinitionPropertiesCreateModel()
                    {
                        DefinitionCreationSource = new CMSVSTSReleaseDefinitionPropertiesItemCreateModel()
                        {
                            Type = "System.String",
                            Value = "ReleaseNew"
                        }
                    },
                    ReleaseNameFormat = "$(Date:yyyyMMdd)$(Rev:.r)",
                    Description = string.Empty
                };

                return entity;
            }
        }
    }

    public class CMSVSTSReleaseDefinitionUpdateModel : CMSVSTSReleaseDefinitionModel
    {
        
    }

    public class CMSVSTSReleaseDefinitionReadModel : CMSVSTSReleaseDefinitionModel
    {
        [JsonProperty("pipelineProcess")]
        public CMSVSTSReleaseDefinitionPipelineProcessCreateModel PipelineProcess { get; set; }

        [JsonProperty("projectReference")]
        public string ProjectReference { get; set; }
    }

    public class CMSVSTSReleaseDefinitionPipelineProcessCreateModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public abstract class CMSVSTSReleaseDefinitionModel : ICloneable
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("createdBy")]
        public object CreatedBy { get; set; }

        [JsonProperty("modifiedBy")]
        public object ModifiedBy { get; set; }

        [JsonProperty("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [JsonProperty("environments")]
        public List<CMSVSTSReleaseDefinitionEnvironmentCreateModel> Environments { get; set; }

        [JsonProperty("artifacts")]
        public List<CMSVSTSReleaseDefinitionArtifactCreateModel> Artifacts { get; set; }

        [JsonProperty("variables")]
        public object Variables { get; set; }

        [JsonProperty("variableGroups")]
        public List<CMSVSTSReleaseDefinitionVariableGroupCreateModel> VariableGroups { get; set; }

        [JsonProperty("triggers")]
        public List<CMSVSTSReleaseDefinitionTriggerCreateModel> Triggers { get; set; }
        
        [JsonProperty("lastRelease")]
        public string LastRelease { get; set; }
        
        [JsonProperty("tags")]
        public List<CMSVSTSReleaseDefinitionTagCreateModel> Tags { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("properties")]
        public CMSVSTSReleaseDefinitionPropertiesCreateModel Properties { get; set; }
        
        [JsonProperty("releaseNameFormat")]
        public string ReleaseNameFormat { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
        
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentCreateModel : ICloneable
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }
        
        [JsonProperty("variables")]
        public object Variables { get; set; }

        [JsonProperty("variableGroups")]
        public List<CMSVSTSReleaseDefinitionEnvironmentVariableGroupCreateModel> VariableGroups { get; set; }
        
        [JsonProperty("preDeployApprovals")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployAprovalCreateModel PreDeployApprovals { get; set; }

        [JsonProperty("DeployStep")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployStepCreateModel DeployStep { get; set; }
        
        [JsonProperty("postDeployApprovals")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployAprovalCreateModel PostDeployApprovals { get; set; }

        [JsonProperty("deployPhases")]
        public List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseCreateModel> DeployPhases { get; set; }

        [JsonProperty("runOptions")]
        public CMSVSTSReleaseDefinitionEnvironmentRunOptionCreateModel RunOptions { get; set; }

        [JsonProperty("environmentOptions")]
        public CMSVSTSReleaseDefinitionEnvironmentEnvironemtnOptionCreateModel EnvironmentOptions { get; set; }

        [JsonProperty("demands")]
        public List<CMSVSTSReleaseDefinitionEnvironmentDemandCreateModel> Demands { get; set; }

        [JsonProperty("conditions")]
        public List<CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel> Conditions { get; set; }

        [JsonProperty("executionPolicy")]
        public CMSVSTSReleaseDefinitionEnvironmentExecutionPolicyCreateModel ExecutionPolicy { get; set; }

        [JsonProperty("owner")]
        public object Owner { get; set; }

        [JsonProperty("schedules")]
        public List<CMSVSTSReleaseDefinitionEnvironmentScheduleCreateModel> Schedules { get; set; }

        [JsonProperty("properties")]
        public CMSVSTSReleaseDefinitionEnvironmentPropertiesCreateModel Properties { get; set; }

        [JsonProperty("preDeploymentGates")]
        public CMSVSTSReleaseDefinitionEnvironmentDeploymentGateCreateModel PreDeploymentGates { get; set; }

        [JsonProperty("postDeploymentGates")]
        public CMSVSTSReleaseDefinitionEnvironmentDeploymentGateCreateModel PostDeploymentGates { get; set; }

        [JsonProperty("environmentTriggers")]
        public List<CMSVSTSReleaseDefinitionEnvironmentTriggerCreateModel> EnvironmentTriggers { get; set; }

        [JsonProperty("retentionPolicy")]
        public CMSVSTSReleaseDefinitionEnvironmentRetentionPolicyCreateModel RetentionPolicy { get; set; }

        [JsonProperty("processParameters")]
        public CMSVSTSReleaseDefinitionEnvironmentProcessParameterCreateModel ProcessParameters { get; set; }

        [JsonProperty("badgeUrl")]
        public string BadgeUrl { get; set; }
        
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    public class CMSVSTSReleaseDefinitionEnvironmentVariableGroupCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployAprovalCreateModel
    {
        [JsonProperty("approvals")]
        public List<CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel> Approvals { get; set; }

        [JsonProperty("approvalOptions")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployAprovalOptionCreateModel ApprovalOptions { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel
    {
        [JsonProperty("approver")]
        public object Approver { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("isAutomated")]
        public bool IsAutomated { get; set; }

        [JsonProperty("isNotificationOn")]
        public bool IsNotificationOn { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployAprovalOptionCreateModel
    {
        [JsonProperty("executionOrder")]
        public string ExecutionOrder { get; set; }

        [JsonProperty("autoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped")]
        public bool AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped { get; set; }

        [JsonProperty("enforceIdentityRevalidation")]
        public bool EnforceIdentityRevalidation { get; set; }

        [JsonProperty("ReleaseCreatorCanBeApprover")]
        public bool releaseCreatorCanBeApprover { get; set; }

        [JsonProperty("requiredApproverCount")]
        public int RequiredApproverCount { get; set; }

        [JsonProperty("timeoutInMinutes")]
        public int TimeoutInMinutes { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployStepCreateModel
    {
        [JsonProperty("tasks")]
        public List<CMSVSTSReleaseDefinitionEnvironmentDeployStepTaskCreateModel> Tasks { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployStepTaskCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentRunOptionCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentEnvironemtnOptionCreateModel
    {
        [JsonProperty("emailNotificationType")]
        public string EmailNotificationType { get; set; }

        [JsonProperty("emailRecipients")]
        public string EmailRecipients { get; set; }

        [JsonProperty("skipArtifactsDownload")]
        public bool SkipArtifactsDownload { get; set; }

        [JsonProperty("timeoutInMinutes")]
        public int TimeoutInMinutes { get; set; }

        [JsonProperty("enableAccessToken")]
        public bool EnableAccessToken { get; set; }

        [JsonProperty("publishDeploymentStatus")]
        public bool PublishDeploymentStatus { get; set; }

        [JsonProperty("badgeEnabled")]
        public bool BadgeEnabled { get; set; }

        [JsonProperty("autoLinkWorkItems")]
        public bool AutoLinkWorkItems { get; set; }

        [JsonProperty("pullRequestDeploymentEnabled")]
        public bool PullRequestDeploymentEnabled { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDemandCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel
    {
        [JsonProperty("conditionType")]
        public string ConditionType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("environmentId")]
        public int EnvironmentId { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentExecutionPolicyCreateModel
    {
        [JsonProperty("concurrencyCount")]
        public int ConcurrencyCount { get; set; }

        [JsonProperty("queueDepthCount")]
        public int QueueDepthCount { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentScheduleCreateModel
    {
        
    }

    public class CMSVSTSReleaseDefinitionEnvironmentPropertiesCreateModel
    {
        
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeploymentGateCreateModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("gatesOptions")]
        public object GatesOptions { get; set; }

        [JsonProperty("gates")]
        public List<CMSVSTSReleaseDefinitionEnvironmentDeploymentGateItemCreateModel> Gates { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeploymentGateItemCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentTriggerCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentRetentionPolicyCreateModel
    {
        [JsonProperty("daysToKeep")]
        public int DaysToKeep { get; set; }

        [JsonProperty("releasesToKeep")]
        public int ReleasesToKeep { get; set; }

        [JsonProperty("retainBuild")]
        public bool RetainBuild { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentProcessParameterCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhaseCreateModel
    {
        [JsonProperty("deploymentInput")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputCreateModel DeploymentInput { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("phaseType")]
        public string PhaseType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("workflowTasks")]
        public List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskCreateModel> WorkflowTasks { get; set; }

        [JsonProperty("phaseInputs")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployPhasePhaseInputCreateModel PhaseInputs { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputCreateModel
    {
        [JsonProperty("parallelExecution")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputParallelExecutionCreateModel ParallelExecution { get; set; }

        [JsonProperty("skipArtifactsDownload")]
        public bool SkipArtifactsDownload { get; set; }

        [JsonProperty("ArtifactsDownloadInput")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputArtifactsDownloadInputCreateModel ArtifactsDownloadInput { get; set; }

        [JsonProperty("queueId")]
        public int QueueId { get; set; }

        [JsonProperty("demands")]
        public List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputDemandCreateModel> Demands { get; set; }

        [JsonProperty("enableAccessToken")]
        public bool EnableAccessToken { get; set; }

        [JsonProperty("timeoutInMinutes")]
        public int TimeoutInMinutes { get; set; }

        [JsonProperty("jobCancelTimeoutInMinutes")]
        public int JobCancelTimeoutInMinutes { get; set; }

        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("overrideInputs")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputOverrideCreateModel OverrideInputs { get; set; }

        [JsonProperty("dependencies")]
        public List<CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputDependencyCreateModel> Dependencies { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputParallelExecutionCreateModel
    {
        [JsonProperty("parallelExecutionType")]
        public string ParallelExecutionType { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputArtifactsDownloadInputCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputDemandCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputOverrideCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhaseInputDependencyCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskCreateModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("refName")]
        public string RefName { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("timeoutInMinutes")]
        public int TimeoutInMinutes { get; set; }

        [JsonProperty("inputs")]
        public object Inputs { get; set; }

        [JsonProperty("taskId")]
        public string TaskId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("definitionType")]
        public string DefinitionType { get; set; }

        [JsonProperty("alwaysRun")]
        public bool AlwaysRun { get; set; }

        [JsonProperty("continueOnError")]
        public bool ContinueOnError { get; set; }

        [JsonProperty("overrideInputs")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskOverrideInputCreateModel OverrideInputs { get; set; }

        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("environment")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskEnvironmentCreateModel Environment { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskOverrideInputCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhaseWorkflowTaskEnvironmentCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhasePhaseInputCreateModel
    {
        [JsonProperty("phaseinput_artifactdownloadinput")]
        public CMSVSTSReleaseDefinitionEnvironmentDeployPhasePhaseInputArtifactDownloadInputCreateModel ArtifactDownloadInput { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnvironmentDeployPhasePhaseInputArtifactDownloadInputCreateModel
    {
        [JsonProperty("artifactsDownloadInput")]
        public object ArtifactsDownloadInput { get; set; }

        [JsonProperty("skipArtifactsDownload")]
        public bool SkipArtifactsDownload { get; set; }
    }

    public class CMSVSTSReleaseDefinitionArtifactCreateModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("definitionReference")]
        public CMSVSTSReleaseDefinitionArtifactDefinitionReferenceCreateModel DefinitionReference { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("isPrimary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("sourceId")]
        public string SourceId { get; set; }

        [JsonProperty("isRetained")]
        public bool IsRetained { get; set; }
    }

    public class CMSVSTSReleaseDefinitionArtifactDefinitionReferenceCreateModel
    {
        [JsonProperty("project")]
        public CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel Project { get; set; }

        [JsonProperty("definition")]
        public CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel Definition { get; set; }

        [JsonProperty("defaultVersionType")]
        public CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel DefaultVersionType { get; set; }

        [JsonProperty("defaultVersionBranch")]
        public CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel DefaultVersionBranch { get; set; }

        [JsonProperty("defaultVersionTags")]
        public CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel DefaultVersionTags { get; set; }

        [JsonProperty("defaultVersionSpecific")]
        public CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel DefaultVersionSpecific { get; set; }

        [JsonProperty("artifactSourceDefinitionUrl")]
        public CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel ArtifactSourceDefinitionUrl { get; set; }
    }

    public class CMSVSTSReleaseDefinitionArtifactDefinitionReferenceItemCreateModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class CMSVSTSReleaseDefinitionVariableGroupCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionTriggerCreateModel
    {
        [JsonProperty("triggerType")]
        public string TriggerType { get; set; }

        [JsonProperty("triggerConditions")]
        public object TriggerConditions { get; set; }

        [JsonProperty("artifactAlias")]
        public string ArtifactAlias { get; set; }
    }

    public class CMSVSTSReleaseDefinitionTagCreateModel
    {

    }

    public class CMSVSTSReleaseDefinitionPropertiesCreateModel
    {
        [JsonProperty("DefinitionCreationSource")]
        public CMSVSTSReleaseDefinitionPropertiesItemCreateModel DefinitionCreationSource { get; set; }
    }

    public class CMSVSTSReleaseDefinitionPropertiesItemCreateModel
    {
        [JsonProperty("$type")]
        public string Type { get; set; }

        [JsonProperty("$value")]
        public string Value { get; set; }
    }

    public class CMSVSTSReleaseDefinitionInputModel
    {
        public List<CMSVSTSReleaseDefinitionTemplateTasksCreateModel> Tasks { get; set; }
        public object Variables { get; set; }
    }

    public class CMSVSTSReleaseDefinitionTemplateTasksCreateModel
    {
        public string TaskId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public object Inputs { get; set; }
    }

    public class CMSVSTSReleaseDefinitionEnviromentVariableModel
    {
        [JsonProperty("allowOverride")]
        public bool AllowOverride { get; set; }

        [JsonProperty("isSecret")]
        public bool IsSecret { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("scope")]
        public CMSVSTSReleaseDefinitionEnviromentVariableScopeModel Scope { get; set; }

        public static class Factory
        {
            public static CMSVSTSReleaseDefinitionEnviromentVariableModel Create(string environment, string value)
            {
                var entity = new CMSVSTSReleaseDefinitionEnviromentVariableModel()
                {
                    IsSecret = false,
                    AllowOverride = false,
                    Value = value,
                    Scope = new CMSVSTSReleaseDefinitionEnviromentVariableScopeModel()
                    {
                        Key = -1,
                        Value = environment
                    }
                };

                return entity;
            }
        }
    }

    public class CMSVSTSReleaseDefinitionEnviromentVariableScopeModel
    {
        [JsonProperty("key")]
        public int Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class CMSVSTSBuildDefinitionReadModel
    {
        [JsonProperty("authoredBy")]
        public object AuthoredBy { get; set; }
    }
}
