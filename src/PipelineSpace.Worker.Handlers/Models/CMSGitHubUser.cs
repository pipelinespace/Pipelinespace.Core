using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSGitHubUser
    {
        [JsonProperty("id")]
        public int AccountId { get; set; }
    }
}
