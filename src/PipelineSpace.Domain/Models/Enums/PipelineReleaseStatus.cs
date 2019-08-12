using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public enum PipelineReleaseStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Deploy Succeeded")]
        DeploySucceeded = 1,
        [Description("Deploy Failed")]
        DeployFailed = 2,
        [Description("Deploy Canceled")]
        DeployCanceled = 3,
        [Description("Deploying")]
        Deploying = 4,
        [Description("Deploy Pending Approval")]
        DeployPendingApproval = 5,
        [Description("DeployA ccepted Approval")]
        DeployAcceptedApproval = 6,
        [Description("Deploy Rejected Approval")]
        DeployRejectedApproval = 7,
        [Description("Deploy Rejected")]
        DeployRejected = 8
    }
}
