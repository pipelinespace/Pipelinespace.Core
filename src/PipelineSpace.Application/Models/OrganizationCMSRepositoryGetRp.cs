using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationCMSRepositoryListRp
    {
        public IReadOnlyList<OrganizationCMSRepositoryListItemRp> Items { get; set; }
    }

    public class OrganizationCMSRepositoryListItemRp
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
    }
}
