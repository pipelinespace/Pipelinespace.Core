using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationCMSTeamListRp
    {
        public IReadOnlyList<OrganizationCMSTeamListItemRp> Items { get; set; }
    }
    
    public class OrganizationCMSTeamListItemRp
    {
        public string TeamId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
