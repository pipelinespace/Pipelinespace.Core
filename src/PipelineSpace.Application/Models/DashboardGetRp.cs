using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class DashboardGetRp
    {
        public decimal CurrentOrganization { get; set; }
        public decimal CurrentProjects { get; set; }
        public decimal CurrentPipes { get; set; }
        public decimal CurrentFeatures { get; set; }
        public List<OrganizationListItemRp> OrganizationItems { get; set; }
    }
    
}
