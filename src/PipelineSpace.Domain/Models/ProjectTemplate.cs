using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectTemplate : BaseEntity
    {
        public Guid ProjectTemplateId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public CloudProviderService CloudProviderType { get; set; }

        public virtual List<ProjectTemplateService> Services { get; set; }

        [Required]
        public string Logo { get; set; }
    }
}
