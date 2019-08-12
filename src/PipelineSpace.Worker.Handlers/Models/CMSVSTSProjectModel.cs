using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSVSTSProjectListModel
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<CMSVSTSProjectListItemModel> Items { get; set; }
    }

    public class CMSVSTSProjectListItemModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
