using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public enum PipelineRole
    {
        None = 0,
        OrganizationAdmin = 1,
        OrganizationContributor = 2,
        ProjectAdmin = 3,
        ProjectContributor = 4
    }
}
