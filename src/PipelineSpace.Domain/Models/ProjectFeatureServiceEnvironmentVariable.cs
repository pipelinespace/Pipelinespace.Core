using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectFeatureServiceEnvironmentVariable : BaseEntity
    {
        [Required]
        public Guid ProjectFeatureServiceEnvironmentId { get; set; }

        public virtual ProjectFeatureServiceEnvironment ProjectFeatureServiceEnvironment { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public static class Factory
        {
            public static ProjectFeatureServiceEnvironmentVariable Create(Guid projectFeatureServiceEnvironmentId, string name, string value, string createdBy)
            {
                var entity = new ProjectFeatureServiceEnvironmentVariable()
                {
                    ProjectFeatureServiceEnvironmentId = projectFeatureServiceEnvironmentId,
                    Name = name,
                    Value = value,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectFeatureServiceEnvironmentVariable>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
