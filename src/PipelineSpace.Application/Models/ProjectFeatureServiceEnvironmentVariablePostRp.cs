using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureServiceEnvironmentVariablePostRp
    {
        [Required]
        public List<ProjectFeatureServiceEnvironmentVariableItemPostRp> Items { get; set; }
    }

    public class ProjectFeatureServiceEnvironmentVariableItemPostRp
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
