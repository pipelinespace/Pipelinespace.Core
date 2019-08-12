using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectTemplateService : BaseEntity
    {
        [Required]
        public Guid ProjectTemplateId { get; set; }

        public virtual ProjectTemplate ProjectTemplate { get; set; }
        
        [Required]
        public Guid ProjectServiceTemplateId { get; set; }

        public virtual ProjectServiceTemplate ProjectServiceTemplate { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
