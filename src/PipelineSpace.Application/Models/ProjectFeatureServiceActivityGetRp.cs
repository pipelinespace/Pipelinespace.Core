using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureServiceActivityListRp
    {
        public ProjectFeatureServiceActivityListRp()
        {
            Items = new List<ProjectFeatureServiceActivityListItemRp>();
        }

        public IReadOnlyList<ProjectFeatureServiceActivityListItemRp> Items { get; set; }
    }

    public class ProjectFeatureServiceActivityListItemRp
    {
        public string Name { get; set; }
        public string Log { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ActivityStatus ActivityStatus { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class ProjectFeatureServiceActivity
    {
        public string Name { get; set; }
        public string Log { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ActivityStatus ActivityStatus { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
