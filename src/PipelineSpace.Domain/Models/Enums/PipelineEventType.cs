using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public enum PipelineEventType
    {
        [Description("Build Started")]
        BuildStarted = 0,
        [Description("Build Completed")]
        BuildCompleted = 1,
        [Description("Release Started")]
        ReleaseStarted = 2,
        [Description("Release Pending Approval")]
        ReleasePendingApproval = 3,
        [Description("Release Completed Approval")]
        ReleaseCompletedApproval = 4,
        [Description("Release Completed")]
        ReleaseCompleted = 5
    }
}
