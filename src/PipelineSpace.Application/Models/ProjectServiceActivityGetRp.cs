using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceActivityListRp
    {
        public ProjectServiceActivityListRp()
        {
            Items = new List<ProjectServiceActivityListItemRp>();
        }

        public IReadOnlyList<ProjectServiceActivityListItemRp> Items { get; set; }
    }

    public class ProjectServiceActivityListItemRp
    {
        public string Name { get; set; }
        public string Log { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ActivityStatus ActivityStatus { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
