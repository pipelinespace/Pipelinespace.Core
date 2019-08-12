using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.GitHub
{
    public class CMSGitHubRepositoryModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("node_id")]
        public string NodeId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("private")]
        public bool IsPrivate { get; set; }
        [JsonProperty("teams_url")]
        public string TeamLink { get; set; }
        [JsonProperty("clone_url")]
        public string CloneUrl { get; set; }
        [JsonProperty("ssh_url")]
        public string SSHUrl { get; set; }
        [JsonProperty("default_branch")]
        public string DefaultBranch { get; set; }

    }
}
