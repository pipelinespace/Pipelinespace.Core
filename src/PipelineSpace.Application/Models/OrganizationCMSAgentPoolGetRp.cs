using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationCMSAgentPoolListRp
    {
        public IReadOnlyList<OrganizationCMSAgentPoolListItemRp> Items { get; set; }
    }

    public class OrganizationCMSAgentPoolListItemRp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHosted { get; set; }
    }
}
