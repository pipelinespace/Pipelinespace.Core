using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.VSTS
{
    public class CMSVSTSAccountListModel
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<CMSVSTSAccountListItemModel> Items { get; set; }
    }

    public class CMSVSTSAccountListItemModel
    {
        [JsonProperty("AccountId")]
        public string AccountId { get; set; }

        [JsonProperty("AccountName")]
        public string AccountName { get; set; }
    }
}
