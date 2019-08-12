using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.GitLab
{
    public class CMSGitLabTeamModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("web_url")]
        public string Link { get; set; }
        public string Path { get; set; }
    }
}
