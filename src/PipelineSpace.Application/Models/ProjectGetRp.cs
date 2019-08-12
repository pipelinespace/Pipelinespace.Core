using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectListRp
    {
        public ProjectListRp()
        {
            Items = new List<ProjectListItemRp>();
        }

        public IReadOnlyList<ProjectListItemRp> Items { get; set; }
    }

    public class ProjectWithServiceListRp 
    {
        public ProjectWithServiceListRp()
        {
            Items = new List<ProjectWithServiceListItemRp>();
        }

        public IReadOnlyList<ProjectWithServiceListItemRp> Items { get; set; }
    }

    public class ProjectWithServiceListItemRp
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OrganizationName { get; set; }
        public Guid OrganizationId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        public ProjectServiceListRp Services { get; set; }
        public ProjectUserListRp Users { get; set; }
    }

    public class ProjectListItemRp
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OrganizationName { get; set; }
        public Guid OrganizationId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
    }

    public class ProjectGetRp
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OrganizationName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ConfigurationManagementService GitProviderType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CloudProviderService CloudProviderType { get; set; }
        public string AgentPoolId { get; set; }
    }

    public class ProjectExternalGetRp
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
