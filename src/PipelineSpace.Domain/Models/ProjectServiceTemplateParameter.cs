using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectServiceTemplateParameter : BaseEntity
    {
        public Guid ProjectServiceTemplateParameterId { get; set; }

        [Required]
        public Guid ProjectServiceTemplateId { get; set; }

        public virtual ProjectServiceTemplate ProjectServiceTemplate { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string VariableName { get; set; }

        [Required]
        public string Scope { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
