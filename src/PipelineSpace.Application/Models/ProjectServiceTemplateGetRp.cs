using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceTemplateListRp
    {
        public IReadOnlyList<ProjectServiceTemplateListItemRp> Items { get; set; }
    }

    public class ProjectServiceTemplateListItemRp
    {
        public Guid ProjectServiceTemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProgrammingLanguageName { get; set; }
        public string Framework { get; set; }
    }

    public class ProjectServiceTemplateGetRp
    {
        public Guid ProjectServiceTemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProgrammingLanguageName { get; set; }
        public string Framework { get; set; }
    }

    public class ProjectServiceTemplateDefinitionGetRp
    {
        public Guid ProjectServiceTemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProgrammingLanguageName { get; set; }
        public string Framework { get; set; }
        public string BuildDefinition { get; set; }
        public string InfraDefinition { get; set; }

    }
}
