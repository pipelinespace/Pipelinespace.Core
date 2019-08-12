using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public enum PipelineBuildStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Build Succeeded")]
        BuildSucceeded = 1,
        [Description("Build Failed")]
        BuildFailed = 2,
        [Description("Build Canceled")]
        BuildCanceled = 3,
        [Description("Building")]
        Building = 4
    }
}
