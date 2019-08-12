using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureListRp
    {
        public ProjectFeatureListRp()
        {
            Items = new List<ProjectFeatureListItemRp>();
        }

        public IReadOnlyList<ProjectFeatureListItemRp> Items { get; set; }
    }

    public class ProjectFeatureListItemRp
    {
        public Guid ProjectFeatureId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Status { get; set; }
    }

    public class ProjectFeatureGetRp
    {
        public Guid ProjectFeatureId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Status { get; set; }
    }
}
