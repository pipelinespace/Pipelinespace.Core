using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.VSTS
{
    public class CMSVSTSServiceListModel
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<CMSVSTSServiceListItemModel> Items { get; set; }
    }

    public class CMSVSTSServiceListItemModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
