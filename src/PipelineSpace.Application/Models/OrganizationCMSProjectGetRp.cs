using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationCMSProjectListRp
    {
        public IReadOnlyList<OrganizationCMSProjectListItemRp> Items { get; set; }
    }
    
    public class OrganizationCMSProjectListItemRp
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
