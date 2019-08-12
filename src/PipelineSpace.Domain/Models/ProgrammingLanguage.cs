using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProgrammingLanguage : BaseEntity
    {
        [Required]
        public Guid ProgrammingLanguageId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
