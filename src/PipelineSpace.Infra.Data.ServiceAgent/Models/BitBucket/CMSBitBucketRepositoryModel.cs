using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.BitBucket
{
    public class CMSBitBucketRepositoryListModel {
        [JsonProperty("values")]
        public List<CMSBitBucketRepositoryModel> Repositories { get; set; }
    }

    public class CMSBitBucketRepositoryModel
    {
        [JsonProperty("uuid")]
        public Guid Id { get; set; }
        [JsonProperty("scm")]
        public string SCM { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }
        [JsonProperty("is_private")]
        public bool IsPrivate { get; set; }
        public CMSBitBucketRepostoryLinkModel Links { get; set; }
        public CMSBitBucketRepostoryMainBrancModel MainBranch { get; set; }
    }

    public class CMSBitBucketRepostoryLinkModel
    {
        public CMSBitBucketLinkHrefModel Branches { get; set; }
        public List<CMSBitBucketLinkHrefModel> Clone { get; set; }
    }

    public class CMSBitBucketRepostoryMainBrancModel
    {
        public string Name { get; set; }
    }
}
