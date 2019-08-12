using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSPipelineApprovalParamModel
    {
        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public string ProjectName { get; set; }
        public string ProjectExternalId { get; set; }

        public int ApprovalId { get; set; }

        public string Status { get; set; }
        public string Comments { get; set; }
    }
}
