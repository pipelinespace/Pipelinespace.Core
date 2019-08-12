using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectEnvironmentVariablePostRp
    {
        [Required]
        public List<ProjectEnvironmentVariableItemPostRp> Items { get; set; }
    }

    public class ProjectEnvironmentVariableItemPostRp
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
