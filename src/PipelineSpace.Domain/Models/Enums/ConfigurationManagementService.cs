using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public enum ConfigurationManagementService
    {
        VSTS = 0,
        Bitbucket = 1,
        GitHub = 2,
        GitLab = 3
    }
}
