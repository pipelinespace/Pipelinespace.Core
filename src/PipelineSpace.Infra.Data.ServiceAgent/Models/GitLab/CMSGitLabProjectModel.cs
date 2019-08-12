using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.GitLab
{
    public class CMSGitLabProjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("ssh_url_to_repo")]
        public string SSHUrl { get; set; }
        [JsonProperty("http_url_to_repo")]
        public string CloneUrl { get; set; }

    }
}
