using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureServiceEnvironmentListRp
    {
        public ProjectFeatureServiceEnvironmentListRp()
        {
            Items = new List<ProjectFeatureServiceEnvironmentListItemRp>();
        }

        public IReadOnlyList<ProjectFeatureServiceEnvironmentListItemRp> Items { get; set; }
    }

    public class ProjectFeatureServiceEnvironmentListItemRp
    {
        public Guid ProjectFeatureServiceEnvironmentId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        public CPSCloudResourceEnvironmentSummaryModel Summary { get; set; }
        public List<ProjectFeatureServiceEnvironmentVariableListItemRp> Variables { get; set; }
    }

    public class ProjectFeatureFeatureServiceEnvironmentGetRp
    {
        public Guid ProjectServiceEnvironmentId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        public CPSCloudResourceEnvironmentSummaryModel Summary { get; set; }
        public List<ProjectFeatureServiceEnvironmentVariableListItemRp> Variables { get; set; }
    }
}
