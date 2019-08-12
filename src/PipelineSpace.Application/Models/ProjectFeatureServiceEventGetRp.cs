using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureServiceEventListRp
    {
        public ProjectFeatureServiceEventListRp()
        {
            Items = new List<ProjectFeatureServiceEventListItemRp>();
        }

        public IReadOnlyList<ProjectFeatureServiceEventListItemRp> Items { get; set; }
    }

    public class ProjectFeatureServiceEventListItemRp
    {
        public string EventType { get; set; }
        public string EventDescription { get; set; }
        public string EventStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EventDate { get; set; }
    }

    public class ProjectFeatureServiceEventGetRp
    {
        public string EventType { get; set; }
        public string EventDescription { get; set; }
        public string EventStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EventDate { get; set; }
    }
}
