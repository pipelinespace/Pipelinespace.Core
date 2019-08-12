using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.GitLab
{
    public class CMSGitLabRepositoryModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("name_with_namespace")]
        public string FullName { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("web_url")]
        public string TeamLink { get; set; }
        [JsonProperty("http_url_to_repo")]
        public string CloneUrl { get; set; }
        [JsonProperty("ssh_url_to_repo")]
        public string SSHUrl { get; set; }
        [JsonProperty("default_branch")]
        public string DefaultBranch { get; set; }

    }
}
