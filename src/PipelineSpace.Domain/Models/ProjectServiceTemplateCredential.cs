using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectServiceTemplateCredential : BaseEntity
    {
        [Key]
        public Guid ProjectServiceTemplateId { get; set; }
        
        [Required]
        public ConfigurationManagementService CMSType { get; set; }
        
        //vsts or bitbucket
        public string AccessId { get; set; }

        public string AccessSecret { get; set; }

        //github
        public string AccessToken { get; set; }

        public static class Factory
        {
            public static ProjectServiceTemplateCredential Create(ConfigurationManagementService CMSType,
                                                                  string accessId,
                                                                  string accessSecret,
                                                                  string accessToken,
                                                                  string createdBy)
            {
                var entity = new ProjectServiceTemplateCredential()
                {
                    CMSType = CMSType,
                    AccessId = accessId,
                    AccessSecret = accessSecret,
                    AccessToken = accessToken,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectServiceTemplateCredential>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                if (CMSType == ConfigurationManagementService.VSTS)
                {
                    if (string.IsNullOrEmpty(accessId))
                    {
                        throw new ApplicationException("Access Id is required");
                    }

                    if (string.IsNullOrEmpty(accessSecret))
                    {
                        throw new ApplicationException("Access Secret is required");
                    }
                }

                if (CMSType == ConfigurationManagementService.GitHub)
                {
                    if (string.IsNullOrEmpty(accessId))
                    {
                        throw new ApplicationException("Access Id is required");
                    }

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        throw new ApplicationException("Access Token is required");
                    }
                }

                return entity;
            }

            public static ProjectServiceTemplateCredential Create(ConfigurationManagementService CMSType, string createdBy)
            {
                var entity = new ProjectServiceTemplateCredential()
                {
                    CMSType = CMSType,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectServiceTemplateCredential>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }

        }
    }
}
