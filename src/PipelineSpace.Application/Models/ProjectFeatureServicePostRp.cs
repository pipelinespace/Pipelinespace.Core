using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureServicePostRp
    {
        [Required]
        public Guid[] Services { get; set; }
    }
}
