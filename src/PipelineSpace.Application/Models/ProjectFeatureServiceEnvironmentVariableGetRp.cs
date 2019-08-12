using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureServiceEnvironmentVariableListRp
    {
        public List<ProjectFeatureServiceEnvironmentVariableListItemRp> Items { get; set; }
    }

    public class ProjectFeatureServiceEnvironmentVariableListItemRp
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
