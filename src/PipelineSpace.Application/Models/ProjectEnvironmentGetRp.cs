using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectEnvironmentListRp
    {
        public ProjectEnvironmentListRp()
        {
            Items = new List<ProjectEnvironmentListItemRp>();
        }

        public IReadOnlyList<ProjectEnvironmentListItemRp> Items { get; set; }
    }

    public class ProjectEnvironmentListItemRp
    {
        public Guid ProjectEnvironmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EnvironmentType Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        public int Rank { get; set; }
        public ProjectServiceEnvironmentListRp Services { get; set; }
    }

    public class ProjectEnvironmentGetRp
    {
        public Guid ProjectEnvironmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EnvironmentType Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
    }
}
