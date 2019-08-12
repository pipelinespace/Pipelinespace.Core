using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureAllServiceListRp
    {
        public ProjectFeatureAllServiceListRp()
        {
            Items = new List<ProjectFeatureAllServiceListItemRp>();
        }

        public List<ProjectFeatureAllServiceListItemRp> Items { get; set; }
    }

    public class ProjectFeatureAllServiceListItemRp
    {
        public Guid ProjectServiceId { get; set; }
        public string Name { get; set; }
        public bool IsFeatureService { get; set; }
    }

    public class ProjectFeatureAllServiceGetRp
    {
        public Guid ProjectServiceId { get; set; }
        public string Name { get; set; }
        public bool IsFeatureService { get; set; }
    }
}
