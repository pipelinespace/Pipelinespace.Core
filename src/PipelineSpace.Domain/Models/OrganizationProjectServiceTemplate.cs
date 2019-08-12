using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class OrganizationProjectServiceTemplate : BaseEntity
    {
        [Required]
        public Guid OrganizationProjectServiceTemplateId { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        [Required]
        public Guid ProjectServiceTemplateId { get; set; }

        public virtual ProjectServiceTemplate ProjectServiceTemplate { get; set; }

        public static class Factory
        {
            public static OrganizationProjectServiceTemplate Create(Guid organizationId, Guid projectServiceTemplateId, string createdBy)
            {
                var entity = new OrganizationProjectServiceTemplate()
                {
                    OrganizationProjectServiceTemplateId = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    ProjectServiceTemplateId = projectServiceTemplateId,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<OrganizationProjectServiceTemplate>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);
                
                return entity;
            }
        }
    }
}
