using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class CMSRepositoryListRp
    {
        public List<CMSRepositoryListItemRp> Items { get; set; }
    }

    public class CMSRepositoryListItemRp
    {
        public string AccountId { get; set; }
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CMSRepositoryGetRp
    {
        public string AccountId { get; set; }
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
