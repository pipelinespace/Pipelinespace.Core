using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class CMSProjectListRp
    {
        public List<CMSProjectListItemRp> Items { get; set; }
    }

    public class CMSProjectListItemRp
    {
        public string AccountId { get; set; }
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CMSProjectGetRp
    {
        public string AccountId { get; set; }
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
