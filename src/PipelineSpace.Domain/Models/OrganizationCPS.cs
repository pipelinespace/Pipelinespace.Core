using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class OrganizationCPS : BaseEntity
    {
        public Guid OrganizationCPSId { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        public CloudProviderService Type { get; set; }

        [Required]
        public string AccessId { get; set; }
        
        public string AccessName { get; set; }

        public string AccessSecret { get; set; }

        public string AccessAppId { get; set; }

        public string AccessAppSecret { get; set; }

        public string AccessDirectory { get; set; }

        [Required]
        public string AccessRegion { get; set; }
        
        public virtual List<Project> Projects { get; set; }

        public void UpdateCredentials(string accessId,
                                            string accessName,
                                            string accessSecret,
                                            string accessAppId,
                                            string accessAppSecret,
                                            string accessDirectory,
                                            string accessRegion)
        {
            AccessId = accessId;
            AccessName = accessName;
            AccessSecret = accessSecret;
            AccessAppId = accessAppId;
            AccessAppSecret = accessAppSecret;
            AccessDirectory = accessDirectory;
            AccessRegion = accessRegion;

            ValidateBasicConstraints();
        }

        public void ValidateBasicConstraints()
        {
            var validationResult = new DataValidatorManager<OrganizationCPS>().Build().Validate(this);
            if (!validationResult.IsValid)
                throw new ApplicationException(validationResult.Errors);

        }

        public static class Factory
        {
            public static OrganizationCPS Create(string name, CloudProviderService type, 
                                                string accessId, 
                                                string accessName, 
                                                string accessSecret,
                                                string accessAppId,
                                                string accessAppSecret,
                                                string accessDirectory,
                                                string accessRegion, 
                                                string createdBy)
            {
                var entity = new OrganizationCPS()
                {
                    Name = name,
                    Type = type,
                    AccessId = accessId,
                    AccessName = accessName,
                    AccessSecret = accessSecret,
                    AccessAppId = accessAppId,
                    AccessAppSecret = accessAppSecret,
                    AccessDirectory = accessDirectory,
                    AccessRegion = accessRegion,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<OrganizationCPS>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                if (type == CloudProviderService.AWS)
                {
                    if (string.IsNullOrEmpty(accessSecret))
                    {
                        throw new ApplicationException("Access Secret is required");
                    }
                      
                }

                if (type == CloudProviderService.Azure)
                {
                    if (string.IsNullOrEmpty(accessName))
                    {
                        throw new ApplicationException("Subscription Name is required");
                    }

                    if (string.IsNullOrEmpty(accessAppId))
                    {
                        throw new ApplicationException("Application Id is required");
                    }

                    if (string.IsNullOrEmpty(accessAppSecret))
                    {
                        throw new ApplicationException("Application Secret is required");
                    }

                    if (string.IsNullOrEmpty(accessDirectory))
                    {
                        throw new ApplicationException("Directory is required");
                    }
                }

                return entity;
            }
        }
    }
}
