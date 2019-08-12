using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.GitHub
{
    public class CMSGitHubUserModel
    {
        [JsonProperty("login")]
        public string UserName { get; set; }
        [JsonProperty("name")]
        public string DisplayName { get; set; }
        [JsonProperty("id")]
        public string AccountId { get; set; }
        public string Url { get; set; }
        [JsonProperty("organizations_url")] 
        public string OrganizationsLink { get; set; }
        [JsonProperty("repos_url")]
        public string RepositoryLink { get; set; }
        
    }
}
