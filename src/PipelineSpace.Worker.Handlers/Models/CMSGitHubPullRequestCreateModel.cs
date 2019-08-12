using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSGitHubPullRequestCreateModel
    {
        [JsonProperty("head")]
        public string SourceRefName { get; set; }

        [JsonProperty("base")]
        public string TargetRefName { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Description { get; set; }
    }
}
