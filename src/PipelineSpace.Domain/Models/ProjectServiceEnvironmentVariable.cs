using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectServiceEnvironmentVariable : BaseEntity
    {
        [Required]
        public Guid ProjectServiceEnvironmentId { get; set; }

        public virtual ProjectServiceEnvironment ProjectServiceEnvironment { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public static class Factory
        {
            public static ProjectServiceEnvironmentVariable Create(Guid projectServiceEnvironmentId, string name, string value, string createdBy)
            {
                var entity = new ProjectServiceEnvironmentVariable()
                {
                    ProjectServiceEnvironmentId = projectServiceEnvironmentId,
                    Name = name,
                    Value = value,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectServiceEnvironmentVariable>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
