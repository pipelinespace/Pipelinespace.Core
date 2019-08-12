using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSVSTSObjectRepositoryModel : CMSBaseResultModel
    {
        [JsonProperty("objectId")]
        public string ObjectId { get; set; }
        [JsonProperty("gitObjectType")]
        public string ObjectType { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
