using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.GitHub
{
    public class CMSGitHubLinkModel
    {
        public CMSGitHubLinkHrefModel Repositories { get; set; }
        public CMSGitHubLinkHrefModel Hooks { get; set; }
    }

    public class CMSGitHubLinkHrefModel
    {
        public string Href { get; set; }
    }
}
