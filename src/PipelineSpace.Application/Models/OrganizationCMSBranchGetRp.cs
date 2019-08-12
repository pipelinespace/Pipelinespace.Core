using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationCMSBranchListRp
    {
        public IReadOnlyList<OrganizationCMSBranchListItemRp> Items { get; set; }
    }

    public class OrganizationCMSBranchListItemRp
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
