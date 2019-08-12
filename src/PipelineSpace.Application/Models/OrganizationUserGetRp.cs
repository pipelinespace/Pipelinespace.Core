using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationUserListRp
    {
        public OrganizationUserListRp()
        {
            this.Items = new List<OrganizationUserListItemRp>();
        }

        public IReadOnlyList<OrganizationUserListItemRp> Items { get; set; }
    }

    public class OrganizationUserListItemRp
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineRole Role { get; set; }
        public DateTime AddedDate { get; set; }
        public string UserId { get; set; }
    }

    public class OrganizationUserGetRp
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineRole Role { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
