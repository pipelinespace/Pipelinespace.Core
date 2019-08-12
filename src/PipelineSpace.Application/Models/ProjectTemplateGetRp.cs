using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectTemplateListRp
    {
        public IReadOnlyList<ProjectTemplateListItemRp> Items { get; set; }
    }

    public class ProjectTemplateListItemRp
    {
        public Guid ProjectTemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
    }

    public class ProjectTemplateGetRp
    {
        public Guid ProjectTemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
    }
}
