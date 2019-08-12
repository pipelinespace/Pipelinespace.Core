using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.BitBucket
{
    public class CMSBitBucketUserModel
    {
        [JsonProperty("uuid")]
        public string Id { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("account_id")]
        public string AccountId { get; set; }
        public CMSBitBucketLinkModel Links { get; set; }
    }
}
