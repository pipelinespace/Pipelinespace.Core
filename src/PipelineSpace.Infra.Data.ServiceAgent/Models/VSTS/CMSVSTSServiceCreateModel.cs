using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.VSTS
{
    public class CMSVSTSServiceCreateModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("project")]
        public CMSVSTSServiceProjectCreateModel Project { get; set; }
    }

    public class CMSVSTSServiceProjectCreateModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class CMSVSTSServiceCreateResultModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("remoteUrl")]
        public string RemoteUrl { get; set; }
    }
}
