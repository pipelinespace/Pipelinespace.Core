using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.VSTS
{
    public class CMSVSTSBranchListModel
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<CMSVSTSBranchListItemModel> Items { get; set; }
    }

    public class CMSVSTSBranchListItemModel
    {
        [JsonProperty("objectId")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }

    }
}
