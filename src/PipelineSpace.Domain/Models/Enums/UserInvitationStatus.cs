using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public enum UserInvitationStatus
    {
        Pending = 0,
        Accepted = 1,
        Canceled = 2,
        AutoCanceled = 3
    }
}
