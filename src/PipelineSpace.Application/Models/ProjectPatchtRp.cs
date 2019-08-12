using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectPatchRp
    {
        public string ProjectVSTSFakeId { get; set; }
        public string ProjectVSTSFakeName { get; set; }

        public string ProjectExternalId { get; set; }
        public string ProjectExternalName { get; set; }

        public string ProjectExternalEndpointId { get; set; }

        public string ProjectExternalGitEndpoint { get; set; }
    }
}
