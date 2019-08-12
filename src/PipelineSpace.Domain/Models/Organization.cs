using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class Organization : BaseEntity
    {
        public Organization()
        {

        }

        public Guid OrganizationId { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        [Url]
        public string WebSiteUrl { get; set; }

        public virtual User Owner { get; set; }

        [Required]
        [StringLength(450)]
        public string OwnerId { get; set; }

        public virtual List<OrganizationUser> Users { get; set; }

        public virtual List<Project> Projects { get; set; }
        
        public virtual List<OrganizationCMS> ConfigurationManagementServices { get; set; }

        public virtual List<OrganizationCPS> CloudProviderServices { get; set; }

        public virtual List<OrganizationUserInvitation> UserInvitations { get; set; }

        public virtual List<OrganizationProjectServiceTemplate> ProjectServiceTemplates { get; set; }
        
        public void GrantUserAccess(string userId, PipelineRole role)
        {
            if (this.Users == null)
                this.Users = new List<OrganizationUser>();

            var userOrganization = OrganizationUser.Factory.Create(this.OrganizationId, userId, role, userId);

            this.Users.Add(userOrganization);
        }

        public void UpdateBasicInformation(string name, string description, string webSiteUrl)
        {
            //this.Name = name;
            this.Description = description;
            this.WebSiteUrl = webSiteUrl;
        }

        public void AddProject(Project project)
        {
            if (this.Projects == null)
                this.Projects = new List<Project>();
            
            this.Projects.Add(project);
        }

        public Project GetProjectById(Guid projectId)
        {
            if (this.Projects == null)
                this.Projects = new List<Project>();

            return this.Projects.FirstOrDefault(x => x.ProjectId == projectId);
        }

        public OrganizationUser GetOrganizationUser(string userEmail)
        {
            if (this.Users == null)
                this.Users = new List<OrganizationUser>();

            return this.Users.FirstOrDefault(x => x.User.Email.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase));
        }

        public OrganizationUser GetOrganizationUserById(string userId)
        {
            if (this.Users == null)
                this.Users = new List<OrganizationUser>();

            return this.Users.FirstOrDefault(x => x.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase));
        }

        public OrganizationUserInvitation GetOrganizationUserInvitation(string userEmail)
        {
            if (this.UserInvitations == null)
                this.UserInvitations = new List<OrganizationUserInvitation>();

            return this.UserInvitations.FirstOrDefault(x => x.UserEmail.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase));
        }

        public void AddUserInvitation(OrganizationUserInvitation organizationUserInvitation)
        {
            if (this.UserInvitations == null)
                this.UserInvitations = new List<OrganizationUserInvitation>();

            this.UserInvitations.Add(organizationUserInvitation);
        }

        public OrganizationUserInvitation GetUserInvitation(Guid invitationId)
        {
            if (this.UserInvitations == null)
                this.UserInvitations = new List<OrganizationUserInvitation>();

            return this.UserInvitations.FirstOrDefault(x => x.OrganizationUserInvitationId == invitationId);
        }

        public List<Project> GetPreparingProjects()
        {
            if (this.Projects == null)
                this.Projects = new List<Project>();

            return this.Projects.Where(x => x.Status == EntityStatus.Preparing).ToList();
        }

        public Project GetProjectByName(string name)
        {
            if (this.Projects == null)
                this.Projects = new List<Project>();

            return this.Projects.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public List<Project> GetProjectsByCMSId(Guid organizationCMSId)
        {
            if (this.Projects == null)
                this.Projects = new List<Project>();

            return this.Projects.Where(x => x.OrganizationCMSId == organizationCMSId).ToList();
        }

        public List<Project> GetProjectsByCPSId(Guid organizationCPSId)
        {
            if (this.Projects == null)
                this.Projects = new List<Project>();

            return this.Projects.Where(x => x.OrganizationCPSId == organizationCPSId).ToList();
        }

        public void AddConfigurationManagementService(OrganizationCMS configurationManagementService)
        {
            if (this.ConfigurationManagementServices == null)
                this.ConfigurationManagementServices = new List<OrganizationCMS>();

            this.ConfigurationManagementServices.Add(configurationManagementService);
        }

        public OrganizationCMS GetConfigurationManagementServiceById(Guid organizationCMSId)
        {
            if (this.ConfigurationManagementServices == null)
                this.ConfigurationManagementServices = new List<OrganizationCMS>();

            return this.ConfigurationManagementServices.FirstOrDefault(x => x.OrganizationCMSId == organizationCMSId);
        }

        public OrganizationCMS GetConfigurationManagementServiceByName(string name)
        {
            if (this.ConfigurationManagementServices == null)
                this.ConfigurationManagementServices = new List<OrganizationCMS>();

            return this.ConfigurationManagementServices.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public OrganizationCMS GetConfigurationManagementServiceByType(ConfigurationManagementService type)
        {
            if (this.ConfigurationManagementServices == null)
                this.ConfigurationManagementServices = new List<OrganizationCMS>();

            return this.ConfigurationManagementServices.FirstOrDefault(x => x.Type == type);
        }

        public List<OrganizationCMS> GetConfigurationManagementServices(CMSConnectionType connectionType)
        {
            if (this.ConfigurationManagementServices == null)
                this.ConfigurationManagementServices = new List<OrganizationCMS>();

            return this.ConfigurationManagementServices.Where(x=> x.ConnectionType == connectionType).ToList();
        }

        public void AddCloudProviderService(OrganizationCPS cloudProviderservice)
        {
            if (this.CloudProviderServices == null)
                this.CloudProviderServices = new List<OrganizationCPS>();

            this.CloudProviderServices.Add(cloudProviderservice);
        }

        public OrganizationCPS GetCloudProviderServiceById(Guid oganizationCPSId)
        {
            if (this.CloudProviderServices == null)
                this.CloudProviderServices = new List<OrganizationCPS>();

            return this.CloudProviderServices.FirstOrDefault(x => x.OrganizationCPSId == oganizationCPSId);
        }

        public OrganizationCPS GetCloudProviderServiceByName(string name)
        {
            if (this.CloudProviderServices == null)
                this.CloudProviderServices = new List<OrganizationCPS>();

            return this.CloudProviderServices.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public OrganizationCPS GetCloudProviderServiceByType(CloudProviderService type)
        {
            if (this.CloudProviderServices == null)
                this.CloudProviderServices = new List<OrganizationCPS>();

            return this.CloudProviderServices.FirstOrDefault(x => x.Type == type);
        }

        public List<OrganizationCPS> GetCloudProviderServices()
        {
            if (this.CloudProviderServices == null)
                this.CloudProviderServices = new List<OrganizationCPS>();

            return this.CloudProviderServices.ToList();
        }

        public ProjectServiceTemplate GetProjectServiceTemplateById(Guid projectServiceTemplateId)
        {
            if (this.ProjectServiceTemplates == null)
                this.ProjectServiceTemplates = new List<OrganizationProjectServiceTemplate>();

            var organizationProjectServiceTemplate = this.ProjectServiceTemplates.FirstOrDefault(x => x.ProjectServiceTemplate.ProjectServiceTemplateId == projectServiceTemplateId);
            if (organizationProjectServiceTemplate == null)
                return null;

            return organizationProjectServiceTemplate.ProjectServiceTemplate;
        }

        public OrganizationProjectServiceTemplate GetProjectServiceTemplateByName(string name)
        {
            if (this.ProjectServiceTemplates == null)
                this.ProjectServiceTemplates = new List<OrganizationProjectServiceTemplate>();

            return this.ProjectServiceTemplates.FirstOrDefault(x => x.ProjectServiceTemplate.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public List<ProjectServiceTemplate> GetAllProjectServiceTemplates()
        {
            if (this.ProjectServiceTemplates == null)
                this.ProjectServiceTemplates = new List<OrganizationProjectServiceTemplate>();

            var projectServiceTemplates = this.ProjectServiceTemplates.Select(x => x.ProjectServiceTemplate);
            if (projectServiceTemplates == null)
                return null;

            return projectServiceTemplates.ToList();
        }

        public List<ProjectServiceTemplate> GetProjectServiceTemplates(CloudProviderService cloudProviderType, PipeType pipeType)
        {
            if (this.ProjectServiceTemplates == null)
                this.ProjectServiceTemplates = new List<OrganizationProjectServiceTemplate>();

            var projectServiceTemplates = this.ProjectServiceTemplates.Select(x => x.ProjectServiceTemplate);
            if (projectServiceTemplates == null)
                return null;

            return projectServiceTemplates.Where(x=> x.ServiceCPSType == cloudProviderType && x.PipeType == pipeType).ToList();
        }

        public bool IsTemplateBeingUsed(Guid projectServiceTemplateId)
        {
            if (this.Projects == null)
                this.Projects = new List<Project>();

            bool isBeingUsed = false;
            foreach (var project in this.Projects)
            {
                isBeingUsed = project.Services.Any(x => x.ProjectServiceTemplateId == projectServiceTemplateId);
                if(isBeingUsed)
                {
                    break;
                }
            }

            return isBeingUsed;
        }

        public static class Factory
        {
            public static Organization Create(string name, string description, string webSiteUrl, string ownerId, string createdBy)
            {
                var entity = new Organization()
                {
                    Name = name,
                    Description = description,
                    WebSiteUrl = webSiteUrl,
                    OwnerId = ownerId,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<Organization>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                entity.GrantUserAccess(createdBy, PipelineRole.OrganizationAdmin);

                return entity;
            }
        }
    }
}
