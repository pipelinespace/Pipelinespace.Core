using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class Project : BaseEntity
    {
        public Project()
        {

        }

        public Guid ProjectId { get; set; }
        
        [Required]
        [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        public string InternalName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        public virtual User Owner { get; set; }

        [Required]
        public ProjectType ProjectType { get; set; }

        [Required]
        public ProjectVisibility ProjectVisibility { get; set; }

        [Required]
        [StringLength(450)]
        public string OwnerId { get; set; }

        public string OrganizationExternalId { get; set; }

        public virtual Organization Organization { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        public virtual List<ProjectUser> Users { get; set; }

        public virtual List<ProjectService> Services { get; set; }

        public virtual List<ProjectFeature> Features { get; set; }

        public virtual List<ProjectEnvironment> Environments { get; set; }

        public virtual List<ProjectActivity> Activities { get; set; }

        public virtual List<ProjectUserInvitation> UserInvitations { get; set; }

        [Required]
        public Guid OrganizationCMSId { get; set; }

        public virtual OrganizationCMS OrganizationCMS { get; set; }

        //[Required]
        public Guid? OrganizationCPSId { get; set; }

        public virtual OrganizationCPS OrganizationCPS { get; set; }

        public Guid? ProjectTemplateId { get; set; }

        public virtual ProjectTemplate ProjectTemplate { get; set; }
        
        /// <summary>
        /// External Id from CMS
        /// </summary>
        public string ProjectExternalId { get; set; }
        public string ProjectExternalName { get; set; }
        public bool IsImported { get; set; }

        /// <summary>
        /// Fake VSTS Project Name
        /// </summary>
        public string ProjectVSTSFakeName { get; set; }

        /// <summary>
        /// Fake VSTS Project Id
        /// </summary>
        public string ProjectVSTSFakeId { get; set; }

        /// <summary>
        /// Cloud Endpoint Id to deploy infrastructure
        /// </summary>
        public string ProjectExternalEndpointId { get; set; }

        /// <summary>
        /// Git Endpoint Id to store repositories
        /// </summary>
        public string ProjectExternalGitEndpoint { get; set; }

        /// <summary>
        /// Agent pool (tasks)
        /// </summary>
        [Required]
        public string AgentPoolId { get; set; }
        
        public void UpdateExternalServiceInformation(string projectExternalId)
        {
            this.ProjectExternalId = projectExternalId;
        }
        
        public void GrantUserAccess(string userId, PipelineRole role)
        {
            if (this.Users == null)
                this.Users = new List<ProjectUser>();

            var userProject = ProjectUser.Factory.Create(this.ProjectId, userId, role, userId);

            this.Users.Add(userProject);
        }

        public void AddService(ProjectService service)
        {
            if (this.Services == null)
                this.Services = new List<ProjectService>();

            this.Services.Add(service);
        }

        public void AddFeature(ProjectFeature feature)
        {
            if (this.Features == null)
                this.Features = new List<ProjectFeature>();

            this.Features.Add(feature);
        }

        public ProjectEnvironment GetDevelopmentEnvironment()
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectEnvironment>();

            return this.Environments.FirstOrDefault(x => x.Name.Equals(DomainConstants.Environments.Development));

        }

        public ProjectEnvironment GetProductionEnvironment()
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectEnvironment>();

            return this.Environments.FirstOrDefault(x => x.Name.Equals(DomainConstants.Environments.Production));
        }

        public void AddEnvironment(ProjectEnvironment environment)
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectEnvironment>();

            this.Environments.Add(environment);
        }

        public void AddActivity(string code, string name)
        {
            if (this.Activities == null)
                this.Activities = new List<ProjectActivity>();

            var activity = ProjectActivity.Factory.Create(code, name, this.CreatedBy);

            this.Activities.Add(activity);
        }

        public void UpdateBasicInformation(string name, string description)
        {
            //this.Name = name;
            this.Description = description;

            this.ValidateBasicConstraints();
        }

        public void UpdateExternalInformation(string id, string name)
        {
            this.ProjectExternalId = id;
            this.ProjectExternalName = name;
            
            this.ValidateBasicConstraints();
        }

        public ProjectService GetServiceByName(string name)
        {
            if (this.Services == null)
                this.Services = new List<ProjectService>();

            return this.Services.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public ProjectService GetServiceById(Guid serviceId)
        {
            if (this.Services == null)
                this.Services = new List<ProjectService>();

            return this.Services.FirstOrDefault(x => x.ProjectServiceId == serviceId);
        }

        public ProjectFeature GetFeatureByName(string name)
        {
            if (this.Features == null)
                this.Features = new List<ProjectFeature>();

            return this.Features.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public List<ProjectService> GetServices()
        {
            if (this.Services == null)
                this.Services = new List<ProjectService>();

            return this.Services.ToList();
        }

        public List<ProjectService> GetServicesWithReleaseStages()
        {
            if (this.Services == null)
                this.Services = new List<ProjectService>();

            return this.Services.Where(x => x.ReleaseStageId.HasValue).ToList();
        }

        public List<ProjectService> GetPreparingServices()
        {
            if (this.Services == null)
                this.Services = new List<ProjectService>();

            return this.Services.Where(x => x.Status == EntityStatus.Preparing).ToList();
        }

        public List<ProjectFeature> GetPreparingFeatures()
        {
            if (this.Features == null)
                this.Features = new List<ProjectFeature>();

            return this.Features.Where(x => x.Status == EntityStatus.Preparing).ToList();
        }

        public List<ProjectFeature> GetFeatures()
        {
            if (this.Features == null)
                this.Features = new List<ProjectFeature>();

            return this.Features.Where(x=> x.Status == EntityStatus.Active).ToList();
        }

        public ProjectFeature GetFeatureById(Guid featureId)
        {
            if (this.Features == null)
                this.Features = new List<ProjectFeature>();

            return this.Features.FirstOrDefault(x => x.ProjectFeatureId == featureId);
        }

        public ProjectEnvironment GetEnvironmentByName(string name)
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectEnvironment>();

            return this.Environments.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public ProjectEnvironment GetEnvironmentById(Guid environmentId)
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectEnvironment>();

            return this.Environments.FirstOrDefault(x => x.ProjectEnvironmentId == environmentId);
        }

        public ProjectEnvironment GetRootEnvironment()
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectEnvironment>();

            return this.Environments.FirstOrDefault(x => x.Type == EnvironmentType.Root);
        }

        public List<ProjectEnvironment> GetEnvironments()
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectEnvironment>();

            return this.Environments.ToList();
        }

        public List<ProjectActivity> GetActivities()
        {
            if (this.Activities == null)
                this.Activities = new List<ProjectActivity>();

            return this.Activities.ToList();
        }

        public void SetFakeVSTSProject(string projectName) {
            this.ProjectVSTSFakeName = projectName;
        }

        public ProjectUser GetProjectUserById(string userId)
        {
            if (this.Users == null)
                this.Users = new List<ProjectUser>();

            return this.Users.FirstOrDefault(x => x.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase));
        }

        public ProjectUser GetProjectUser(string userEmail)
        {
            if (this.Users == null)
                this.Users = new List<ProjectUser>();

            return this.Users.FirstOrDefault(x => x.User.Email.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase));
        }

        public List<ProjectUser> GetProjectUsers()
        {
            if (this.Users == null)
                this.Users = new List<ProjectUser>();

            return this.Users;
        }

        public ProjectUserInvitation GetProjectUserInvitation(string userEmail)
        {
            if (this.UserInvitations == null)
                this.UserInvitations = new List<ProjectUserInvitation>();

            return this.UserInvitations.FirstOrDefault(x => x.UserEmail.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase));
        }

        public void AddUserInvitation(ProjectUserInvitation projectUserInvitation)
        {
            if (this.UserInvitations == null)
                this.UserInvitations = new List<ProjectUserInvitation>();

            this.UserInvitations.Add(projectUserInvitation);
        }

        public ProjectUserInvitation GetUserInvitation(Guid invitationId)
        {
            if (this.UserInvitations == null)
                this.UserInvitations = new List<ProjectUserInvitation>();

            return this.UserInvitations.FirstOrDefault(x => x.ProjectUserInvitationId == invitationId);
        }

        public static class Factory
        {
            public static Project Create(string organizationExternalId, string name, string description, 
                ProjectType projectType, Guid organizationCMSId, Guid? organizationCPSId, Guid? projectTemplateId, string agentPoolId, 
                ProjectVisibility projectVisibility, CloudProviderService cloudProviderService, ConfigurationManagementService configurationManagementService,  string createdBy)
            {
                var entity = new Project()
                {
                    Name = name,
                    InternalName = name.Replace(".", "").Replace("_", ""),
                    OrganizationExternalId = organizationExternalId,
                    Description = description,
                    ProjectType = projectType,
                    OrganizationCMSId = organizationCMSId,
                    OrganizationCPSId = organizationCPSId,
                    ProjectTemplateId = projectTemplateId,
                    AgentPoolId = agentPoolId,
                    ProjectVisibility = projectVisibility,
                    OwnerId = createdBy,
                    CreatedBy = createdBy
                };

                var validationResult = new DataValidatorManager<Project>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                entity.GrantUserAccess(createdBy, PipelineRole.ProjectAdmin);

                //add activities
                entity.AddActivity(nameof(DomainConstants.Activities.PRCRBA), DomainConstants.Activities.PRCRBA);
                if (cloudProviderService == CloudProviderService.AWS)
                {
                    entity.AddActivity(nameof(DomainConstants.Activities.PREXBA), DomainConstants.Activities.PREXBA);
                }
                else if (cloudProviderService == CloudProviderService.Azure)
                {
                    entity.AddActivity(nameof(DomainConstants.Activities.PREXBO), DomainConstants.Activities.PREXBO);
                }

                if (configurationManagementService == ConfigurationManagementService.GitLab) {
                    entity.AddActivity(nameof(DomainConstants.Activities.PREXGL), DomainConstants.Activities.PREXGL);
                }

                entity.AddActivity(nameof(DomainConstants.Activities.PRCLEP), DomainConstants.Activities.PRCLEP);
                entity.AddActivity(nameof(DomainConstants.Activities.PRGTEP), DomainConstants.Activities.PRGTEP);                
                entity.AddActivity(nameof(DomainConstants.Activities.PRACBA), DomainConstants.Activities.PRACBA);
                if (projectTemplateId.HasValue)
                {
                    entity.AddActivity(nameof(DomainConstants.Activities.PRSTPT), DomainConstants.Activities.PRSTPT);
                }

                return entity;
            }
        }

        public void ValidateBasicConstraints()
        {
            var validationResult = new DataValidatorManager<Project>().Build().Validate(this);
            if (!validationResult.IsValid)
                throw new ApplicationException(validationResult.Errors);
        }
    }
}
