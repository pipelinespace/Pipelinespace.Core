using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceEnvironmentVariablePostRp
    {
        [Required]
        public List<ProjectServiceEnvironmentVariableItemPostRp> Items { get; set; }
    }

    public class ProjectServiceEnvironmentVariableItemPostRp
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
