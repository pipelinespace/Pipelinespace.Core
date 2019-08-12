using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.VSTS
{
    public class CMSVSTSServiceContentPushModel
    {
        [JsonProperty("refUpdates")]
        public List<CMSVSTSServiceContentReferenceModel> RefUpdates { get; set; }
        
        [JsonProperty("commits")]
        public List<CMSVSTSServiceContentCommitModel> Commits { get; set; }
    }

    public class CMSVSTSServiceContentReferenceModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class CMSVSTSServiceContentCommitModel
    {
        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("changes")]
        public List<CMSVSTSServiceContentChangeModel> Changes { get; set; }
    }

    public class CMSVSTSServiceContentChangeModel
    {
        [JsonProperty("changeType")]
        public string ChangeType { get; set; }

        [JsonProperty("item")]
        public CMSVSTSServiceContentItemModel Item { get; set; }
    }

    public class CMSVSTSServiceContentItemModel
    {
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}

