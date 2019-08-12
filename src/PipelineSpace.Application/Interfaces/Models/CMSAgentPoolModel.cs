using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSAgentPoolListModel : CMSBaseResultModel
    {
        [JsonProperty("value")]
        public IReadOnlyList<CMSAgentPoolListItemModel> Items { get; set; }
    }

    public class CMSAgentPoolListItemModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("isHosted")]
        public bool IsHosted { get; set; }
    }
}
