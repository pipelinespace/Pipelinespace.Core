using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceEventListRp
    {
        public ProjectServiceEventListRp()
        {
            Items = new List<ProjectServiceEventListItemRp>();
        }

        public IReadOnlyList<ProjectServiceEventListItemRp> Items { get; set; }
    }

    public class ProjectServiceEventListItemRp
    {
        public string EventType { get; set; }
        public string EventDescription { get; set; }
        public string EventStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EventDate { get; set; }
    }

    public class ProjectServiceEventGetRp
    {
        public string EventType { get; set; }
        public string EventDescription { get; set; }
        public string EventStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EventDate { get; set; }
    }
}
