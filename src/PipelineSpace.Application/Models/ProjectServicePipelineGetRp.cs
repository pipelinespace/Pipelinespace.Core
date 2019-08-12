using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServicePipelineGetRp
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipeType PipeType { get; set; }
        
        public List<ProjectServicePipelinePhaseGetRp> Phases { get; set; }
    }

    public class ProjectServicePipelinePhaseGetRp
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public int Rank { get; set; }

        public string LastStatusCode { get; set; }

        public string LastStatusDescription { get; set; }
        
        public string LastVersionId { get; set; }

        public string LastVersionName { get; set; }

        public string LastSuccessVersionId { get; set; }

        public string LastSuccessVersionName { get; set; }

        public string LastApprovalId { get; set; }
    }
}
