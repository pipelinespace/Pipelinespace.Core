using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectUserListRp
    {
        public ProjectUserListRp()
        {
            this.Items = new List<ProjectUserListItemRp>();
        }

        public IReadOnlyList<ProjectUserListItemRp> Items { get; set; }
    }

    public class ProjectUserListItemRp
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineRole Role { get; set; }
        public DateTime AddedDate { get; set; }
    }

    public class ProjectUserGetRp
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineRole Role { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
