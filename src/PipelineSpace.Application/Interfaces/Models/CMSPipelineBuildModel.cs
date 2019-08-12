using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSPipelineBuildParamModel
    {
        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public string ProjectName { get; set; }
        public string ProjectExternalId { get; set; }

        public int QueueId { get; set; }
        public int BuildDefinitionId { get; set; }

        public string SourceBranch { get; set; }
    }
}
