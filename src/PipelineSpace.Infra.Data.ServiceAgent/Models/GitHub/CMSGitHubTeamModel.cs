using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.GitHub
{
    
    public class CMSGitHubTeamModel
    {
        [JsonProperty("id")]
        public string TeamId { get; set; }
        [JsonProperty("login")]
        public string DisplayName { get; set; }
        [JsonProperty("node_id")]
        public string NodeId { get; set; }
    }
}
