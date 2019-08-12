using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectServiceTemplate : BaseEntity
    {
        public Guid ProjectServiceTemplateId { get; set; }

        [Required]
        public ConfigurationManagementService ServiceCMSType { get; set; }

        [Required]
        public CloudProviderService ServiceCPSType { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Url { get; set; }

        public string Path { get; set; }

        [Required]
        public string Logo { get; set; }

        [Required]
        public PipeType PipeType { get; set; }

        [Required]
        public TemplateType TemplateType { get; set; }

        [Required]
        public TemplateAccess TemplateAccess { get; set; }

        [Required]
        public bool NeedCredentials { get; set; }

        [Required]
        public Guid ProgrammingLanguageId { get; set; }

        public virtual ProgrammingLanguage ProgrammingLanguage { get; set; }

        [Required]
        public string Framework { get; set; }
        
        public virtual List<ProjectService> ProjectServices { get; set; }

        public virtual List<ProjectTemplateService> ProjectTemplateServices { get; set; }

        public virtual List<ProjectServiceTemplateParameter> Parameters { get; set; }

        public virtual List<OrganizationProjectServiceTemplate> Organizations { get; set; }

        public virtual ProjectServiceTemplateCredential Credential { get; set; }

        public void AddParameters(List<ProjectServiceTemplateParameter> parameters)
        {
            if (this.Parameters == null)
                this.Parameters = new List<ProjectServiceTemplateParameter>();

            foreach (var item in parameters)
            {
                this.Parameters.Add(new ProjectServiceTemplateParameter() {
                    ProjectServiceTemplateParameterId = Guid.NewGuid(),
                    ProjectServiceTemplateId = this.ProjectServiceTemplateId,
                    Name = item.Name,
                    Value = item.Value,
                    Scope = item.Scope,
                    VariableName = item.VariableName,
                    Status = EntityStatus.Active,
                    CreatedBy = this.CreatedBy
                });
            }
        }

        public void AddOrganization(Guid organizationId)
        {
            if (this.Organizations == null)
                this.Organizations = new List<OrganizationProjectServiceTemplate>();

            var newOrganizationProjectServiceTemplate = OrganizationProjectServiceTemplate.Factory.Create(organizationId, this.ProjectServiceTemplateId, this.CreatedBy);

            this.Organizations.Add(newOrganizationProjectServiceTemplate);
        }

        public void AddCredential(ConfigurationManagementService cmsType, bool needCredentials, string accessId, string accessSecret, string accessToken)
        {
            if (needCredentials)
            {
                this.Credential = ProjectServiceTemplateCredential.Factory.Create(cmsType, accessId, accessSecret, accessToken, this.CreatedBy);
            }
            else
            {
                this.Credential = ProjectServiceTemplateCredential.Factory.Create(cmsType, this.CreatedBy);
            }
        }

        public static class Factory
        {
            public static ProjectServiceTemplate Create(string name,
                                                        ConfigurationManagementService serviceCMSType,
                                                        CloudProviderService serviceCPSType,
                                                        string description,
                                                        string url,
                                                        string logo,
                                                        PipeType pipeType,
                                                        TemplateType templateType,
                                                        TemplateAccess templateAccess,
                                                        bool needCredentials,
                                                        Guid programmingLanguageId,
                                                        string framework,
                                                        string createdBy)
            {
                var entity = new ProjectServiceTemplate()
                {
                    Name = name,
                    ServiceCMSType = serviceCMSType,
                    ServiceCPSType = serviceCPSType,
                    Description = description,
                    Url = url,
                    Logo = logo,
                    PipeType = pipeType,
                    TemplateType = templateType,
                    TemplateAccess = templateAccess,
                    NeedCredentials = needCredentials,
                    ProgrammingLanguageId = programmingLanguageId,
                    Framework = framework,
                    CreatedBy = createdBy
                };

                var validationResult = new DataValidatorManager<ProjectServiceTemplate>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
