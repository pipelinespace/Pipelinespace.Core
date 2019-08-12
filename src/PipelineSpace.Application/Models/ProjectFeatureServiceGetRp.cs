using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureServiceListRp
    {
        public ProjectFeatureServiceListRp()
        {
            Items = new List<ProjectFeatureServiceListItemRp>();
        }

        public IReadOnlyList<ProjectFeatureServiceListItemRp> Items { get; set; }
    }

    public class ProjectFeatureServiceListItemRp
    {
        public Guid ProjectFeatureId { get; set; }
        public Guid ProjectServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineStatus PipelineStatus { get; set; }
    }

    public class ProjectFeatureServiceGetRp
    {
        public Guid ProjectFeatureId { get; set; }
        public Guid ProjectServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineStatus PipelineStatus { get; set; }
    }
}
