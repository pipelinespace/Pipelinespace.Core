using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public enum PipelineStatus
    {
        [Description("Preparing")]
        Preparing = 0,
        [Description("Building")]
        Building = 1,
        [Description("Build Succeeded")]
        BuildSucceeded = 2,
        [Description("Build Failed")]
        BuildFailed = 3,
        [Description("Build Canceled")]
        BuildCanceled = 4,
        [Description("Deploying")]
        Deploying = 5,
        [Description("Deploy Succeeded")]
        DeploySucceeded = 6,
        [Description("Deploy Failed")]
        DeployFailed = 7,
        [Description("Deploy Canceled")]
        DeployCanceled = 8,
        [Description("Deploy Pending Approval")]
        DeployPendingApproval = 9,
        [Description("Deploy Accepted Approval")]
        DeployAcceptedApproval = 10,
        [Description("Deploy Rejected Approval")]
        DeployRejectedApproval = 11,
        [Description("Deploy Rejected")]
        DeployRejected = 12,
    }
}
