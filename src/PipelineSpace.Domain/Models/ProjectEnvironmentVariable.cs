using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectEnvironmentVariable : BaseEntity
    {
        [Required]
        public Guid ProjectEnvironmentId { get; set; }

        public virtual ProjectEnvironment ProjectEnvironment { get; set; }

        [Required] 
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public static class Factory
        {
            public static ProjectEnvironmentVariable Create(Guid projectEnvironmentId, string name, string value, string createdBy)
            {
                var entity = new ProjectEnvironmentVariable()
                {
                    ProjectEnvironmentId = projectEnvironmentId,
                    Name = name,
                    Value = value,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectEnvironmentVariable>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
