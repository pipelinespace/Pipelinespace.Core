using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectFeatureEnvironmentVariable : BaseEntity
    {
        [Required]
        public Guid ProjectFeatureEnvironmentId { get; set; }

        public virtual ProjectFeatureEnvironment ProjectFeatureEnvironment { get; set; }

        [Required] 
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public static class Factory
        {
            public static ProjectFeatureEnvironmentVariable Create(Guid projectFeatureEnvironmentId, string name, string value, string createdBy)
            {
                var entity = new ProjectFeatureEnvironmentVariable()
                {
                    ProjectFeatureEnvironmentId = projectFeatureEnvironmentId,
                    Name = name,
                    Value = value,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectFeatureEnvironmentVariable>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
