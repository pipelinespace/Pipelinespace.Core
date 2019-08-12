using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.BitBucket
{
    public class CMSBitBucketProjectListModel
    {
        [JsonProperty("values")]
        public List<CMSBitBucketProjectModel> Projects { get; set; }
    }

    public class CMSBitBucketProjectModel
    {
        [JsonProperty("uuid")]
        public string Id { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        public CMSBitBucketLinkModel Links { get; set; }
    }
}
