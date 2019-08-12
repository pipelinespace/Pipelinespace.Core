using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using PipelineSpace.Domain.ModelUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectService : BaseEntity
    {
        public ProjectService()
        {

        }

        public Guid ProjectServiceId { get; set; }
        
        [Required]
        [RegularExpression("^[a-zA-Z0-9._]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        public string InternalName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        public virtual Project Project { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public Guid ProjectServiceTemplateId { get; set; }

        [Required]
        public PipeType PipeType { get; set; }

        public virtual ProjectServiceTemplate ProjectServiceTemplate { get; set; }

        public virtual List<ProjectFeatureService> Features { get; set; }

        public virtual List<ProjectServiceEvent> Events { get; set; }

        public virtual List<ProjectServiceActivity> Activities { get; set; }

        public virtual List<ProjectServiceEnvironment> Environments { get; set; }

        public virtual List<ProjectServiceDelivery> Deliveries { get; set; }
        
        /// <summary>
        /// External Id from CMS
        /// </summary>
        public string ProjectServiceExternalId { get; set; }

        /// <summary>
        /// External URL from CMS
        /// </summary>
        [Url]
        public string ProjectServiceExternalUrl { get; set; }

        /// <summary>
        /// External Name from CMS
        /// </summary>
        public string ProjectServiceExternalName { get; set; }

        /// <summary>
        /// External Commit Stage Id
        /// </summary>
        public int? CommitStageId { get; set; }

        /// <summary>
        /// External Release Stage Id
        /// </summary>
        public int? ReleaseStageId { get; set; }

        /// <summary>
        /// Commit External Service Hook Id
        /// </summary>
        public Guid? CommitServiceHookId { get; set; }

        /// <summary>
        /// Release External Service Hook Id
        /// </summary>
        public Guid? ReleaseServiceHookId { get; set; }

        /// <summary>
        /// Code Service Hook Id
        /// </summary>
        public Guid? CodeServiceHookId { get; set; }

        /// <summary>
        /// Release Started Service Hook Id
        /// </summary>
        public Guid? ReleaseStartedServiceHookId { get; set; }

        /// <summary>
        /// Release Pending Approval Service Hook Id
        /// </summary>
        public Guid? ReleasePendingApprovalServiceHookId { get; set; }

        /// <summary>
        /// Release Completed Approval Service Hook Id
        /// </summary>
        public Guid? ReleaseCompletedApprovalServiceHookId { get; set; }
        
        public PipelineStatus PipelineStatus { get; set; }

        public PipelineBuildStatus LastPipelineBuildStatus { get; set; }

        public PipelineReleaseStatus LastPipelineReleaseStatus { get; set; }

        public DateTime LastBuildEventDate { get; set; }

        public DateTime LasReleaseEventDate { get; set; }

        public string LastBuildVersionId { get; set; }

        public string LastBuildVersionName { get; set; }

        public string LastBuildSuccessVersionId { get; set; }

        public string LastBuildSuccessVersionName { get; set; }
        
        public Guid? OrganizationCMSId { get; set; }

        public virtual OrganizationCMS OrganizationCMS { get; set; }

        public string BranchName { get; set; }
        public bool IsImported { get; set; }

        /// <summary>
        /// Project External Name ( Import Pipe Feature )
        /// </summary>
        public string ProjectExternalName { get; set; }
        public string ProjectExternalId { get; set; }

        /// <summary>
        /// Agent pool (tasks)
        /// </summary>
        [Required]
        public string AgentPoolId { get; set; }

        /// <summary>
        /// Branch ( Import Pipe Feature )
        /// </summary>
        public string ProjectBranchServiceExternalUrl { get; set; }

        public void UpdateBasicInformation(string name, string description)
        {
            //this.Name = name;
            this.Description = description;

            this.ValidateBasicConstraints();
        }

        public void UpdateExternalInformation(string id, string url, string name)
        {
            this.ProjectServiceExternalId = id;
            this.ProjectServiceExternalUrl = url;
            this.ProjectServiceExternalName = name;
            this.ValidateBasicConstraints();
        }
        
        public void AddEvent(BaseEventType baseEventType, string eventType, string eventDescription, string eventStatus, string eventMessage, string eventDetailedMessage, string eventResource, DateTime eventDate)
        {
            if (Events == null)
                Events = new List<ProjectServiceEvent>();

            var projectServiceEvent = ProjectServiceEvent.Factory.Create(baseEventType, eventType, eventDescription, eventStatus, eventMessage, eventDetailedMessage, eventResource, eventDate);

            Events.Add(projectServiceEvent);
        }

        public void AddActivity(Guid projectId, string code, string name)
        {
            if (this.Activities == null)
                this.Activities = new List<ProjectServiceActivity>();

            var activity = ProjectServiceActivity.Factory.Create(code, name, this.CreatedBy);
            activity.ProjectId = projectId;

            this.Activities.Add(activity);
        }

        public void AddEnvironmentsAndVariables(List<ProjectServiceTemplateParameter> parameters)
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectServiceEnvironment>();

            foreach (var projectEnvironment in this.Project.Environments)
            {
                var serviceEnvironment = ProjectServiceEnvironment.Factory.Create(projectEnvironment.ProjectEnvironmentId, this.CreatedBy);
                foreach (var parameter in parameters)
                {
                    serviceEnvironment.AddVariable(parameter.VariableName, parameter.Value);
                }
                this.Environments.Add(serviceEnvironment);
            }
        }

        public List<ProjectServiceEnvironmentVariable> GetRootEnvironmentVariables()
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectServiceEnvironment>();

            var rootEnvironment = this.Environments.FirstOrDefault(x => x.ProjectEnvironment.Type == EnvironmentType.Root);
            if(rootEnvironment != null)
            {
                return rootEnvironment.Variables;
            }
            return null;
        }

        private bool ExistsEnvironment(Guid projectEnvironmentId)
        {
            return this.Environments.Any(x => x.ProjectEnvironmentId == projectEnvironmentId);
        }

        public void AddEnvironment(Guid projectEnvironmentId, List<ProjectServiceEnvironmentVariable> variables)
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectServiceEnvironment>();

            if(!ExistsEnvironment(projectEnvironmentId))
            {
                var serviceEnvironment = ProjectServiceEnvironment.Factory.Create(projectEnvironmentId, this.CreatedBy);
                foreach (var variable in variables)
                {
                    serviceEnvironment.AddVariable(variable.Name, variable.Value);
                }
                this.Environments.Add(serviceEnvironment);
            }
        }

        public ProjectServiceEnvironment GetServiceEnvironment(Guid projectServiceEnvironmentId)
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectServiceEnvironment>();

            return this.Environments.FirstOrDefault(x => x.ProjectServiceEnvironmentId == projectServiceEnvironmentId);
        }

        private ProjectServiceDelivery AddOrModifyDelivery(int versionId, string versionName)
        {
            if (this.Deliveries == null)
                this.Deliveries = new List<ProjectServiceDelivery>();

            ProjectServiceDelivery projectServiceDelivery;
            projectServiceDelivery = this.Deliveries.FirstOrDefault(x => x.VersionId == versionId);
            if(projectServiceDelivery == null)
            {
                projectServiceDelivery = ProjectServiceDelivery.Factory.Create(versionId, versionName);
                this.Deliveries.Add(projectServiceDelivery);
            }

            return projectServiceDelivery;
        }

        public void AddDeliveryBuildCompleted(int versionId, string versionName, string status, DateTime deliveryDate)
        {
            ProjectServiceDelivery projectServiceDelivery = this.AddOrModifyDelivery(versionId, versionName);
            projectServiceDelivery.UpdateBuildInformation(status, deliveryDate);
        }

        public void AddDeliveryReleaseStarted(int versionId, string versionName, List<DeliveryEnvironmentModel> environments)
        {
            ProjectServiceDelivery projectServiceDelivery = this.AddOrModifyDelivery(versionId, versionName);
            projectServiceDelivery.AddReleaseStarted(environments);
        }

        public void UpdateDeliveryReleaseStatus(int versionId, string versionName, string environmentName, string environmentStatus)
        {
            ProjectServiceDelivery projectServiceDelivery = this.AddOrModifyDelivery(versionId, versionName);
            projectServiceDelivery.UpdateReleaseStatus(environmentName, environmentStatus);
        }

        public static class Factory
        {
            public static ProjectService Create(string name, string repositoryName, string description, 
                Guid projectServiceTemplateId, PipeType pipeType, Guid projectId, Guid organizationCMSId, string agentPoolId, string createdby)
            {
                var entity = new ProjectService()
                {
                    OrganizationCMSId = organizationCMSId,
                    ProjectId = projectId,
                    Name = name,
                    InternalName = repositoryName,
                    Description = description,
                    ProjectServiceTemplateId = projectServiceTemplateId,
                    PipeType = pipeType,
                    AgentPoolId = agentPoolId,
                    CreatedBy = createdby
                };

                var validationResult = new DataValidatorManager<ProjectService>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                //add activities
                entity.AddActivity(projectId, nameof(DomainConstants.Activities.PSPRRQ), DomainConstants.Activities.PSPRRQ);
                entity.AddActivity(projectId, nameof(DomainConstants.Activities.PSCRRP), DomainConstants.Activities.PSCRRP);
                entity.AddActivity(projectId, nameof(DomainConstants.Activities.PSCRBD), DomainConstants.Activities.PSCRBD);
                entity.AddActivity(projectId, nameof(DomainConstants.Activities.PSCRRD), DomainConstants.Activities.PSCRRD);
                entity.AddActivity(projectId, nameof(DomainConstants.Activities.PSQUDB), DomainConstants.Activities.PSQUDB);
                entity.AddActivity(projectId, nameof(DomainConstants.Activities.PSACBA), DomainConstants.Activities.PSACBA);

                return entity;
            }
        }

        public void ValidateBasicConstraints()
        {
            var validationResult = new DataValidatorManager<ProjectService>().Build().Validate(this);
            if (!validationResult.IsValid)
                throw new ApplicationException(validationResult.Errors);
        }
    }
}
