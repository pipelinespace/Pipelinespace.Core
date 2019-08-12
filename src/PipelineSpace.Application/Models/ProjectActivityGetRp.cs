using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectActivitytListRp
    {
        public ProjectActivitytListRp()
        {
            Items = new List<ProjectActivityListItemRp>();
        }

        public IReadOnlyList<ProjectActivityListItemRp> Items { get; set; }
    }

    public class ProjectActivityListItemRp
    {
        public string Name { get; set; }
        public string Log { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ActivityStatus ActivityStatus { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class ProjectActivityGetRp
    {
        public string Name { get; set; }
        public string Log { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ActivityStatus ActivityStatus { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
