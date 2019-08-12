using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSPipelineReleaseParamModel
    {
        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public string ProjectName { get; set; }
        public string ProjectExternalId { get; set; }

        public int ReleaseDefinitionId { get; set; }
        public string Alias { get; set; }
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        public string Description { get; set; }
    }
}
