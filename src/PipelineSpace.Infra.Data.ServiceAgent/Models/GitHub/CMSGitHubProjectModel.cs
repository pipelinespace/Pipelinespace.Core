using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.GitHub
{
   
    public class CMSGitHubProjectModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("nodeId")]
        public string NodeId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
