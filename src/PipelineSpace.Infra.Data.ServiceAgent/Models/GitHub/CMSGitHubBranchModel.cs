using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.GitHub
{
    public class CMSGitHubBranchModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("protection_url")]
        public string ProtectionUrl { get; set; }
       
    }
}
