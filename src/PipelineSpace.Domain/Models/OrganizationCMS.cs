using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class OrganizationCMS : BaseEntity
    {
        public Guid OrganizationCMSId { get; set; }
        
        [Required]
        public Guid OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public string AccountName { get; set; }
        
        [Required]
        public ConfigurationManagementService Type { get; set; }

        [Required]
        public CMSConnectionType ConnectionType { get; set; }
        
        [Required]
        public string AccessId { get; set; }

        public string AccessSecret { get; set; }

        [Required]
        public string AccessToken { get; set; }

        public virtual List<Project> Projects { get; set; }
        public virtual List<ProjectService> Services { get; set; }
        
        public void UpdateCredentials(string accessId, string accessSecret)
        {
            //AccessId = accessId;
            AccessSecret = accessSecret;

            ValidateBasicConstraints();
        }

        public void ValidateBasicConstraints()
        {
            var validationResult = new DataValidatorManager<OrganizationCMS>().Build().Validate(this);
            if (!validationResult.IsValid)
                throw new ApplicationException(validationResult.Errors);

        }

        public static class Factory
        {
            public static OrganizationCMS Create(string name, 
                                                 ConfigurationManagementService type,
                                                 CMSConnectionType connectionType,
                                                 string accountId,
                                                 string accountName,
                                                 string accessId, 
                                                 string accessSecret,
                                                 string accessToken,
                                                 string createdBy)
            {
                var entity = new OrganizationCMS()
                {
                    Name = name,
                    Type = type,
                    ConnectionType = connectionType,
                    AccountId = accountId,
                    AccountName = accountName,
                    AccessId = accessId,
                    AccessSecret = accessSecret,
                    AccessToken = accessToken,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<OrganizationCMS>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                if((type == ConfigurationManagementService.VSTS || 
                    type == ConfigurationManagementService.Bitbucket) && string.IsNullOrEmpty(accessSecret))
                {
                    throw new ApplicationException("Access Secret is required");
                }

                return entity;
            }
        }
    }
}
