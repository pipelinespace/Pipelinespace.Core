using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceSummaryGetRp
    {
        public string Name { get; set; }
        public string Description { get; set; }
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

        public ProjectServiceEventListRp Events { get; set; }
        public ProjectServiceActivityListRp Activities { get; set; }
    }
}
