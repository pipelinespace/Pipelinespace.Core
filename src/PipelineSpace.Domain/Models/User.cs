
using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class User : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public virtual List<OrganizationUser> OrganizationUsers { get; set; }

        public virtual List<ProjectUser> ProjectUsers { get; set; }
        
        public virtual List<Organization> Organizations { get; set; }

        public virtual List<Project> Projects { get; set; }
        
        public string GetFullName() {
            return $"{this.FirstName} {this.LastName}";
        }
        
        public int GetTotalOrganizations()
        {
            return Organizations.Count();
        }

        public int GetTotalProjectsByOrganization(Guid organizationId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            return organization.Projects.Count();
        }

        public int GetTotalServicesByProject(Guid organizationId, Guid projectId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            return project.Services.Count();
        }

        public int GetTotalFeaturesByProject(Guid organizationId, Guid projectId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            return project.Features.Count();
        }

        public int GetTotalEnvironmentsByProject(Guid organizationId, Guid projectId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            return project.Environments.Count();
        }

        public Organization FindOrganizationByName(string organizationName)
        {
            if (Organizations == null)
                Organizations = new List<Organization>();

            return Organizations.FirstOrDefault(x => x.Name.Equals(organizationName, StringComparison.InvariantCultureIgnoreCase));
        }

        public Organization FindOrganizationById(Guid organizationId)
        {
            if (Organizations == null)
                Organizations = new List<Organization>();

            var organizations = this.FindOrganizations();

            Organization organization = organizations.FirstOrDefault(x => x.OrganizationId == organizationId);

            return organization;
        }

        public PipelineRole GetRoleInOrganization(Guid organizationId)
        {
            if (OrganizationUsers == null)
                OrganizationUsers = new List<OrganizationUser>();

            OrganizationUser organizationUser = OrganizationUsers.FirstOrDefault(x => x.OrganizationId == organizationId);

            if (organizationUser == null)
                return PipelineRole.None;

            return organizationUser.Role;
        }

        public List<Project> FindProjects(Guid organizationId)
        {
            if (ProjectUsers == null)
                ProjectUsers = new List<ProjectUser>();

            var projects = ProjectUsers.Where(x => x.Project.OrganizationId == organizationId).Select(x=> x.Project).ToList();

            return projects;
        }

        public Project FindProjectById(Guid projectId, bool validateCPS = true)
        {
            if (Projects == null)
                Projects = new List<Project>();

            Project project = Projects.FirstOrDefault(x => x.ProjectId == projectId);
            if (project == null)
            {
                project = ProjectUsers.FirstOrDefault(x => x.ProjectId == projectId)?.Project;
            }

            if (validateCPS) {
                if (project.OrganizationCPS == null)
                {
                    project.OrganizationCPS = new OrganizationCPS { Type = CloudProviderService.None };
                }
            }
            
            return project;
        }

        public PipelineRole GetRoleInProject(Guid projectId)
        {
            if (ProjectUsers == null)
                ProjectUsers = new List<ProjectUser>();

            ProjectUser projectUser = ProjectUsers.FirstOrDefault(x => x.ProjectId == projectId);

            if (projectUser == null)
                return PipelineRole.None;

            return projectUser.Role;
        }

        public List<Organization> FindOrganizations()
        {
            if (OrganizationUsers == null)
                OrganizationUsers = new List<OrganizationUser>();

            List<Organization> organizations = new List<Organization>();
            organizations = OrganizationUsers.Select(x => x.Organization).ToList();

            var organizationsProjects = ProjectUsers.Where(x=> x.Project.Status == EntityStatus.Active).Select(x => x.Project.Organization);
            organizations.AddRange(organizationsProjects);

            return organizations.Distinct().ToList();
        }

        public Organization CreateOrganization(string name, string description, string webSiteUrl, string ownerId)
        {
            if (Organizations == null)
                Organizations = new List<Organization>();

            Organization newOrganization = Organization.Factory.Create(name, description, webSiteUrl, ownerId, this.Id);

            Organizations.Add(newOrganization);

            return newOrganization;
        }

        public void UpdateOrganization(Guid organizationId, string name, string description, string webSiteUrl)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            organization.UpdateBasicInformation(name, description, webSiteUrl);
            organization.Audit(this.Id);
        }

        public void DeleteOrganization(Guid organizationId)
        {
            if (Organizations == null)
                Organizations = new List<Organization>();

            var organization = this.FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            organization.Delete(this.Id);
        }

        public Project CreateProject(Guid organizationId, string organizationExternalId, string name, string description, ProjectType projectType, 
            Guid organizationCMSId, Guid? organizationCPSId, Guid? projectTemplateId, string agentPoolId, 
            ProjectVisibility projectVisibility, CloudProviderService cloudProviderService, ConfigurationManagementService configurationManagementService)
        {
            Organization organization = FindOrganizationById(organizationId);
            if(organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project newProject = Project.Factory.Create(organizationExternalId, name, description, projectType, organizationCMSId, organizationCPSId, 
                projectTemplateId, agentPoolId, projectVisibility, cloudProviderService, configurationManagementService, this.Id);

            ProjectEnvironment developmentProjectEnvironment = ProjectEnvironment.Factory.Create(DomainConstants.Environments.Development, "Environment for development and some tests", EnvironmentType.Root, false, false, 1, this.Id);
            developmentProjectEnvironment.Activate();

            newProject.AddEnvironment(developmentProjectEnvironment);

            ProjectEnvironment productionProjectEnvironment = ProjectEnvironment.Factory.Create(DomainConstants.Environments.Production, "Environment for production", EnvironmentType.Fact, true, false, 2, this.Id);
            productionProjectEnvironment.Activate();

            newProject.AddEnvironment(productionProjectEnvironment);

            organization.AddProject(newProject);

            return newProject;
        }

        public Project ImportProject(Guid organizationId, string organizationExternalId, string name, string description, ProjectType projectType, 
            Guid organizationCMSId, Guid? organizationCPSId, Guid? projectTemplateId, string agentPoolId, ProjectVisibility projectVisibility, 
            CloudProviderService cloudProviderService, ConfigurationManagementService configurationManagementService)
        {
            var newProject = CreateProject(organizationId, organizationExternalId, name, description, projectType, organizationCMSId, organizationCPSId, projectTemplateId, agentPoolId, projectVisibility, cloudProviderService, configurationManagementService);

            newProject.IsImported = true;

            return newProject;
        }


        public void UpdateProject(Guid organizationId, Guid projectId, string name, string description)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            project.UpdateBasicInformation(name, description);
            project.Audit(this.Id);
        }

        public void DeleteProject(Guid organizationId, Guid projectId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            project.Delete(this.Id);
        }

        public ProjectService CreateProjectService(Guid organizationId, Guid projectId, Guid organizationCMSId, string agentPoolId, string name, string repositoryName, string description, Guid projectServiceTemplateId, PipeType pipeType)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            var newService = ProjectService.Factory.Create(name, repositoryName, description, projectServiceTemplateId, pipeType, projectId, organizationCMSId, agentPoolId, this.Id);

            project.AddService(newService);

            return newService;
        }

        public ProjectService ImportProjectService(Guid organizationId, Guid projectId, Guid organizationCMSId,  string agentId,
            string name, string repositoryName, string description, Guid projectServiceTemplateId, PipeType pipeType, string branchName, string branchUrl, string projectExternaId, string projectExternalName)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            var newService = ProjectService.Factory.Create(name, repositoryName, description, projectServiceTemplateId, pipeType, projectId, organizationCMSId, project.AgentPoolId, this.Id);

            newService.IsImported = true;
            newService.BranchName = branchName;
            newService.ProjectExternalName = projectExternalName;
            newService.ProjectExternalId = projectExternaId;
            newService.ProjectBranchServiceExternalUrl = branchUrl;

            project.AddService(newService);

            return newService;
        }

        public void UpdateProjectService(Guid organizationId, Guid projectId, Guid serviceId, string name, string description)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            ProjectService projectService = project.GetServiceById(serviceId);
            if (projectService == null)
            {
                throw new ApplicationException($"The pipe with id {serviceId} does not exists");
            }

            projectService.UpdateBasicInformation(name, description);
            projectService.Audit(this.Id);
        }

        public void DeleteProjectService(Guid organizationId, Guid projectId, Guid serviceId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            ProjectService projectService = project.GetServiceById(serviceId);
            if (projectService == null)
            {
                throw new ApplicationException($"The pipe with id {serviceId} does not exists");
            }

            projectService.Delete(this.Id);
        }

        public ProjectFeature CreateProjectFeature(Guid organizationId, Guid projectId, string name, string description)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            var newFeature = ProjectFeature.Factory.Create(name, description, this.Id);

            ProjectFeatureEnvironment developmentProjectEnvironment = ProjectFeatureEnvironment.Factory.Create(DomainConstants.Environments.Development, "Environment for development and some tests", EnvironmentType.Root, false, 1, this.Id);
            developmentProjectEnvironment.Activate();

            newFeature.AddEnvironment(developmentProjectEnvironment);

            project.AddFeature(newFeature);

            return newFeature;
        }
        

        public void DeleteProjectFeature(Guid organizationId, Guid projectId, Guid featureId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                throw new ApplicationException($"The feature with id {featureId} does not exists");
            }

            feature.Delete(this.Id);
        }

        public void CompleteProjectFeature(Guid organizationId, Guid projectId, Guid featureId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                throw new ApplicationException($"The feature with id {featureId} does not exists");
            }

            feature.Complete();
        }

        public ProjectEnvironment CreateProjectEnvironment(Guid organizationId, Guid projectId, string name, string description, bool requiresApproval, bool autoProvision)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            //Reorder
            var productionEnvironment = project.GetProductionEnvironment();

            var newEnvironment = ProjectEnvironment.Factory.Create(name, description, EnvironmentType.Plan, requiresApproval, autoProvision, productionEnvironment.Rank, this.Id);
            project.AddEnvironment(newEnvironment);

            productionEnvironment.Rank = productionEnvironment.Rank + 1;

            return newEnvironment;
        }

        public void DeleteProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            ProjectEnvironment projectEnvironment = project.GetEnvironmentById(environmentId);
            if (projectEnvironment == null)
            {
                throw new ApplicationException($"The project environment with id {environmentId} does not exists");
            }

            projectEnvironment.Delete(this.Id);
        }

        public void InactivateProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            ProjectEnvironment projectEnvironment = project.GetEnvironmentById(environmentId);
            if (projectEnvironment == null)
            {
                throw new ApplicationException($"The project environment with id {environmentId} does not exists");
            }

            projectEnvironment.Inactivate(this.Id);
        }

        public void ReactivateProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                throw new ApplicationException($"The project with id {projectId} does not exists");
            }

            ProjectEnvironment projectEnvironment = project.GetEnvironmentById(environmentId);
            if (projectEnvironment == null)
            {
                throw new ApplicationException($"The project environment with id {environmentId} does not exists");
            }

            projectEnvironment.Rectivate();
        }

        public void AddConfigurationManagementService(Guid organizationId, string name, ConfigurationManagementService type, CMSConnectionType connectionType, string accountId, string accountName, string accessId, string accessSecret, string accessToken)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            var newOrganizationCMS = OrganizationCMS.Factory.Create(name, type, connectionType, accountId, accountName, accessId, accessSecret, accessToken, this.Id);
            
            organization.AddConfigurationManagementService(newOrganizationCMS);
        }

        public void UpdateConfigurationManagementService(Guid organizationId, Guid organizationCMSId, string accessId, string accessSecret)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            OrganizationCMS organizationCMS = organization.GetConfigurationManagementServiceById(organizationCMSId);
            if (organizationCMS == null)
            {
                throw new ApplicationException($"The organization configuration management service with id {organizationCMSId} does not exists");
            }

            organizationCMS.UpdateCredentials(accessId, accessSecret);
            organizationCMS.Audit(this.Id);
        }

        public void DeleteConfigurationManagementService(Guid organizationId, Guid organizationCMSId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            OrganizationCMS organizationCMS = organization.GetConfigurationManagementServiceById(organizationCMSId);
            if (organizationCMS == null)
            {
                throw new ApplicationException($"The organization configuration management service with id {organizationCMSId} does not exists");
            }

            organizationCMS.Delete(this.Id);
        }

        public void AddCloudProviderService(Guid organizationId, string name, 
                                            CloudProviderService type, 
                                            string accessId, 
                                            string accessName, 
                                            string accessSecret,
                                            string accessAppId,
                                            string accessAppSecret,
                                            string accessDirectory,
                                            string accessRegion)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            var newOrganizationCPS = OrganizationCPS.Factory.Create(name, type, accessId, accessName, accessSecret, accessAppId, accessAppSecret, accessDirectory, accessRegion, this.Id);

            organization.AddCloudProviderService(newOrganizationCPS);
        }

        public void UpdateCloudProviderService(Guid organizationId, Guid organizationCPSId, string accessId,
                                            string accessName,
                                            string accessSecret,
                                            string accessAppId,
                                            string accessAppSecret,
                                            string accessDirectory,
                                            string accessRegion)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            OrganizationCPS organizationCPS = organization.GetCloudProviderServiceById(organizationCPSId);
            if (organizationCPS == null)
            {
                throw new ApplicationException($"The organization cloud providerservice with id {organizationCPSId} does not exists");
            }

            organizationCPS.UpdateCredentials(accessId, accessName, accessSecret, accessAppId, accessAppSecret, accessDirectory, accessRegion);
            organizationCPS.Audit(this.Id);
        }

        public void DeleteCloudProviderService(Guid organizationId, Guid organizationCPSId)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            OrganizationCPS organizationCPS = organization.GetCloudProviderServiceById(organizationCPSId);
            if (organizationCPS == null)
            {
                throw new ApplicationException($"The organization cloud providerservice with id {organizationCPSId} does not exists");
            }

            organizationCPS.Delete(this.Id);
        }

        public ProjectServiceTemplate AddProjectTemplateService(Guid organizationId, string name, ConfigurationManagementService serviceCMSType,
                                              CloudProviderService serviceCPSType, string description, string url, string logo,
                                              PipeType pipeType, TemplateType templateType, TemplateAccess templateAccess,
                                              bool needCredentials,
                                              Guid programmingLanguageId, string framework, ConfigurationManagementService cmsType, string accessId, string accessSecret, string accessToken,
                                              List<ProjectServiceTemplateParameter> parameters)
        {
            Organization organization = FindOrganizationById(organizationId);
            if (organization == null)
            {
                throw new ApplicationException($"The organization with id {organizationId} does not exists");
            }

            var newOrganizationProjectServiceTemplate = ProjectServiceTemplate.Factory.Create(name, serviceCMSType, serviceCPSType, description, url, logo, pipeType, templateType, templateAccess, needCredentials, programmingLanguageId, framework, this.Id);

            newOrganizationProjectServiceTemplate.AddCredential(cmsType, needCredentials, accessId, accessSecret, accessToken);

            newOrganizationProjectServiceTemplate.AddParameters(parameters);
            newOrganizationProjectServiceTemplate.AddOrganization(organizationId);

            return newOrganizationProjectServiceTemplate;
        }

        public static class Factory
        {
            public static User Create(string firstName, string lastName, string email)
            {
                var entity = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    CreatedBy = email
                };

                var validationResult = new DataValidatorManager<User>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
