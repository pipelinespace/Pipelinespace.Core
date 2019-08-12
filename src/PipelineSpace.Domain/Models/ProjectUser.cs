using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectUser : BaseEntity
    {
        [Required]
        public Guid ProjectUserId { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }

        [Required]
        public PipelineRole Role { get; set; }

        public static class Factory
        {
            public static ProjectUser Create(Guid projectId, string userId, PipelineRole role, string createdBy)
            {
                var entity = new ProjectUser()
                {
                    ProjectUserId = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = userId,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active,
                    Role = role
                };

                var validationResult = new DataValidatorManager<ProjectUser>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
