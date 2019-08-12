using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationListRp
    {
        public OrganizationListRp()
        {
            this.Items = new List<OrganizationListItemRp>();
        }

        public IReadOnlyList<OrganizationListItemRp> Items { get; set; }
    }

    public class OrganizationListItemRp
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
    }

    public class OrganizationGetRp
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string WebSiteUrl { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
    }
}
