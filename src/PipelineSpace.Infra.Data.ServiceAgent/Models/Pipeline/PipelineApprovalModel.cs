using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.Pipeline
{
    public class PipelineApprovalModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("comments")]
        public string Comments { get; set; }
        
        public static PipelineApprovalModel Create(string status, string comments)
        {
            var entity = new PipelineApprovalModel()
            {
                Status = status,
                Comments = comments
            };

            return entity;
        }
    }

}
