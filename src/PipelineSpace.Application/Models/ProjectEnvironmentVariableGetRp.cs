using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectEnvironmentVariableListRp
    {
        public List<ProjectEnvironmentVariableListItemRp> Items { get; set; }
    }

    public class ProjectEnvironmentVariableListItemRp
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
