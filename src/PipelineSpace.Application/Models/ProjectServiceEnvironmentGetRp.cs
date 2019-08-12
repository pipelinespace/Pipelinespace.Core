using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceEnvironmentListRp
    {
        public ProjectServiceEnvironmentListRp()
        {
            Items = new List<ProjectServiceEnvironmentListItemRp>();
        }

        public IReadOnlyList<ProjectServiceEnvironmentListItemRp> Items { get; set; }
    }

    public class ProjectServiceEnvironmentListItemRp
    {
        public Guid ProjectServiceEnvironmentId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        public CPSCloudResourceEnvironmentSummaryModel Summary { get; set; }
        public List<ProjectServiceEnvironmentVariableListItemRp> Variables { get; set; }
    }

    public class ProjectServiceEnvironmentGetRp
    {
        public Guid ProjectServiceEnvironmentId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        public CPSCloudResourceEnvironmentSummaryModel Summary { get; set; }
        public List<ProjectServiceEnvironmentVariableListItemRp> Variables { get; set; }
    }
}
