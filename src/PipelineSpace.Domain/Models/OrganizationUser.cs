using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class OrganizationUser : BaseEntity
    {
        [Required]
        public Guid OrganizationUserId { get; set; }
        
        [Required]
        [StringLength(450)]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

        [Required]
        public PipelineRole Role { get; set; }
        
        public static class Factory
        {
            public static OrganizationUser Create(Guid organizationId, string userId, PipelineRole role, string createdBy)
            {
                var entity = new OrganizationUser()
                {
                    OrganizationUserId = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    UserId = userId,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active,
                    Role = role
                };

                var validationResult = new DataValidatorManager<OrganizationUser>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
