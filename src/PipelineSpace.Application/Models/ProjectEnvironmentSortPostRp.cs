using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectEnvironmentSortPostRp
    {
        [Required]
        public List<ProjectEnvironmentSortPostItemRp> Items { get; set; }
    }

    public class ProjectEnvironmentSortPostItemRp
    {
        public Guid EnvironmentId { get; set; }
        public int Rank { get; set; }
    }
}
