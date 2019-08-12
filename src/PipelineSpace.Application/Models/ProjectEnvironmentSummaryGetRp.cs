using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectEnvironmentSummaryGetRp
    {
        public ProjectEnvironmentSummaryGetRp()
        {
            Environments = new List<ProjectEnvironmentSummaryEnvironmentGetRp>();
            Services = new List<ProjectEnvironmentSummaryServiceGetRp>();
        }
        public List<ProjectEnvironmentSummaryEnvironmentGetRp> Environments { get; set; }
        public List<ProjectEnvironmentSummaryServiceGetRp> Services { get; set; }
    }

    public class ProjectEnvironmentSummaryEnvironmentGetRp
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
    }

    public class ProjectEnvironmentSummaryServiceGetRp
    {
        public ProjectEnvironmentSummaryServiceGetRp()
        {
            Environments = new List<ProjectEnvironmentSummaryServiceEnvironmentGetRp>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ProjectEnvironmentSummaryServiceEnvironmentGetRp> Environments { get; set; }
    }

    public class ProjectEnvironmentSummaryServiceEnvironmentGetRp
    {
        public Guid Id { get; set; }
        public string LastSuccessVersionId { get; set; }
        public string LastSuccessVersionName { get; set; }
    }
}
