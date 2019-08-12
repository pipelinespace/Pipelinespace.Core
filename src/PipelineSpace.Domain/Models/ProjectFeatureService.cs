using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using PipelineSpace.Domain.ModelUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectFeatureService : BaseEntity
    {
        public Guid ProjectFeatureServiceId { get; set; }

        [Required]
        public Guid ProjectFeatureId { get; set; }

        public virtual ProjectFeature ProjectFeature { get; set; }

        [Required]
        public Guid ProjectServiceId { get; set; }

        public virtual ProjectService ProjectService { get; set; }

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

        public virtual List<ProjectFeatureServiceEvent> Events { get; set; }

        public virtual List<ProjectFeatureServiceActivity> Activities { get; set; }

        public virtual List<ProjectFeatureServiceEnvironment> Environments { get; set; }

        public virtual List<ProjectFeatureServiceDelivery> Deliveries { get; set; }

        public void AddEvent(BaseEventType baseEventType, string eventType, string eventDescription, string eventStatus, string eventMessage, string eventDetailedMessage, string eventResource, DateTime eventDate)
        {
            if (Events == null)
                Events = new List<ProjectFeatureServiceEvent>();

            var projectServiceEvent = ProjectFeatureServiceEvent.Factory.Create(baseEventType, eventType, eventDescription, eventStatus, eventMessage, eventDetailedMessage, eventResource, eventDate);

            Events.Add(projectServiceEvent);
        }

        public void AddActivity(Guid projectId, Guid featureId, Guid serviceId, string code, string name)
        {
            if (this.Activities == null)
                this.Activities = new List<ProjectFeatureServiceActivity>();

            var activity = ProjectFeatureServiceActivity.Factory.Create(code, name, this.CreatedBy);
            activity.ProjectId = projectId;
            activity.ProjectFeatureId = featureId;
            activity.ProjectServiceId = serviceId;

            this.Activities.Add(activity);
        }

        public void AddEnvironmentsAndVariables(List<ProjectFeatureEnvironment> environments, List<ProjectServiceEnvironmentVariable> parameters)
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectFeatureServiceEnvironment>();

            foreach (var projectEnvironment in environments)
            {
                var serviceEnvironment = ProjectFeatureServiceEnvironment.Factory.Create(projectEnvironment.ProjectFeatureEnvironmentId, this.CreatedBy);
                foreach (var parameter in parameters)
                {
                    serviceEnvironment.AddVariable(parameter.Name, parameter.Value);
                }
                this.Environments.Add(serviceEnvironment);
            }
        }

        public List<ProjectFeatureServiceEnvironmentVariable> GetRootEnvironmentVariables()
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectFeatureServiceEnvironment>();

            var rootEnvironment = this.Environments.FirstOrDefault(x => x.ProjectFeatureEnvironment.Type == EnvironmentType.Root);
            if (rootEnvironment != null)
            {
                return rootEnvironment.Variables;
            }
            return null;
        }

        private bool ExistsEnvironment(Guid projectFeatureEnvironmentId)
        {
            return this.Environments.Any(x => x.ProjectFeatureEnvironmentId == projectFeatureEnvironmentId);
        }

        public void AddEnvironment(Guid projectFeatureEnvironmentId, List<ProjectServiceEnvironmentVariable> variables)
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectFeatureServiceEnvironment>();

            if (!ExistsEnvironment(projectFeatureEnvironmentId))
            {
                var serviceEnvironment = ProjectFeatureServiceEnvironment.Factory.Create(projectFeatureEnvironmentId, this.CreatedBy);
                foreach (var variable in variables)
                {
                    serviceEnvironment.AddVariable(variable.Name, variable.Value);
                }
                this.Environments.Add(serviceEnvironment);
            }
        }

        public ProjectFeatureServiceEnvironment GetServiceEnvironment(Guid projectFeatureServiceEnvironmentId)
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectFeatureServiceEnvironment>();

            return this.Environments.FirstOrDefault(x => x.ProjectFeatureServiceEnvironmentId == projectFeatureServiceEnvironmentId);
        }

        private ProjectFeatureServiceDelivery AddOrModifyDelivery(int versionId, string versionName)
        {
            if (this.Deliveries == null)
                this.Deliveries = new List<ProjectFeatureServiceDelivery>();

            ProjectFeatureServiceDelivery projectFeatureServiceDelivery;
            projectFeatureServiceDelivery = this.Deliveries.FirstOrDefault(x => x.VersionId == versionId);
            if (projectFeatureServiceDelivery == null)
            {
                projectFeatureServiceDelivery = ProjectFeatureServiceDelivery.Factory.Create(versionId, versionName);
                this.Deliveries.Add(projectFeatureServiceDelivery);
            }

            return projectFeatureServiceDelivery;
        }

        public void AddDeliveryBuildCompleted(int versionId, string versionName, string status, DateTime deliveryDate)
        {
            ProjectFeatureServiceDelivery projectFeatureServiceDelivery = this.AddOrModifyDelivery(versionId, versionName);
            projectFeatureServiceDelivery.UpdateBuildInformation(status, deliveryDate);
        }

        public void AddDeliveryReleaseStarted(int versionId, string versionName, List<DeliveryEnvironmentModel> environments)
        {
            ProjectFeatureServiceDelivery projectFeatureServiceDelivery = this.AddOrModifyDelivery(versionId, versionName);
            projectFeatureServiceDelivery.AddReleaseStarted(environments);
        }

        public void UpdateDeliveryReleaseStatus(int versionId, string versionName, string environmentName, string environmentStatus)
        {
            ProjectFeatureServiceDelivery projectFeatureServiceDelivery = this.AddOrModifyDelivery(versionId, versionName);
            projectFeatureServiceDelivery.UpdateReleaseStatus(environmentName, environmentStatus);
        }
        
        public static class Factory
        {
            public static ProjectFeatureService Create(Guid projectId, Guid projectFeatureId, Guid projectServiceId, string createdBy)
            {
                var entity = new ProjectFeatureService()
                {
                    ProjectFeatureServiceId = Guid.NewGuid(),
                    ProjectFeatureId = projectFeatureId,
                    ProjectServiceId = projectServiceId,
                    CreatedBy = createdBy
                };

                var validationResult = new DataValidatorManager<ProjectFeatureService>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                //add activities
                entity.AddActivity(projectId, projectFeatureId, projectServiceId, nameof(DomainConstants.Activities.PSPRRQ), DomainConstants.Activities.PSPRRQ);
                entity.AddActivity(projectId, projectFeatureId, projectServiceId, nameof(DomainConstants.Activities.PSCRBR), DomainConstants.Activities.PSCRBR);
                entity.AddActivity(projectId, projectFeatureId, projectServiceId, nameof(DomainConstants.Activities.PSCRBD), DomainConstants.Activities.PSCRBD);
                entity.AddActivity(projectId, projectFeatureId, projectServiceId, nameof(DomainConstants.Activities.PSCRRD), DomainConstants.Activities.PSCRRD);
                entity.AddActivity(projectId, projectFeatureId, projectServiceId, nameof(DomainConstants.Activities.PSQUDB), DomainConstants.Activities.PSQUDB);
                entity.AddActivity(projectId, projectFeatureId, projectServiceId, nameof(DomainConstants.Activities.PSACBA), DomainConstants.Activities.PSACBA);


                return entity;
            }
        }
    }
}
