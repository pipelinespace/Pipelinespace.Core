using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceListRp
    {
        public ProjectServiceListRp()
        {
            Items = new List<ProjectServiceListItemRp>();
        }

        public IReadOnlyList<ProjectServiceListItemRp> Items { get; set; }
    }

    public class ProjectServiceListItemRp
    {
        public Guid ProjectServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineStatus PipelineStatus { get; set; }
        public DateTime CreationDate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipeType PipeType { get; set; }
    }

    public class ProjectServiceGetRp
    {
        public Guid ProjectServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineStatus PipelineStatus { get; set; }
        public string ServiceExternalUrl { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ConfigurationManagementService GitProviderType { get; set; }
    }

    public class ProjectServiceExternalGetRp
    {
        public string GitUrl { get; set; }
        public string SSHUrl { get; set; }
        public string DefaultBranch { get; set; }
    }
}
