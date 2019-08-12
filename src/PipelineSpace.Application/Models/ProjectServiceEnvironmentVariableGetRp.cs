using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceEnvironmentVariableListRp
    {
        public List<ProjectServiceEnvironmentVariableListItemRp> Items { get; set; }
    }

    public class ProjectServiceEnvironmentVariableListItemRp
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
