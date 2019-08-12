using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceFeatureListRp
    {
        public ProjectServiceFeatureListRp()
        {
            Items = new List<ProjectServiceFeatureListItemRp>();
        }

        public IReadOnlyList<ProjectServiceFeatureListItemRp> Items { get; set; }
    }

    public class ProjectServiceFeatureListItemRp
    {
        public Guid FeatureId { get; set; }
        public string FeatureName { get; set; }
    }

    public class ProjectServiceFeatureGetRp
    {
        public Guid FeatureId { get; set; }
        public string FeatureName { get; set; }
    }
}
