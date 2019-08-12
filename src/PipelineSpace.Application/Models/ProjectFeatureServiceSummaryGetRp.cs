using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureServiceSummaryGetRp
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipeType PipeType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineStatus PipelineStatus { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineBuildStatus LastPipelineBuildStatus { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineReleaseStatus LastPipelineReleaseStatus { get; set; }

        public ProjectFeatureServiceEventListRp Events { get; set; }
        public ProjectFeatureServiceActivityListRp Activities { get; set; }
    }
}
