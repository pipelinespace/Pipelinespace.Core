using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.BitBucket
{
    public class CMSBitBucketTeamListModel
    {
        [JsonProperty("values")]
        public List<CMSBitBucketTeamModel> Teams { get; set; }
    }

    public class CMSBitBucketTeamModel
    {
        [JsonProperty("uuid")]
        public string TeamId { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        public CMSBitBucketLinkModel Links { get; set; }
        public bool IsTeam { get; set; }
    }
    
}
