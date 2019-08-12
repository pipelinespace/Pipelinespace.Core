using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureServicePatchtRp
    {
        public int? CommitStageId { get; set; }

        public int? ReleaseStageId { get; set; }

        public Guid? CommitServiceHookId { get; set; }

        public Guid? ReleaseServiceHookId { get; set; }

        public Guid? CodeServiceHookId { get; set; }

        public Guid? ReleaseStartedServiceHookId { get; set; }

        public Guid? ReleasePendingApprovalServiceHookId { get; set; }

        public Guid? ReleaseCompletedApprovalServiceHookId { get; set; }

        public PipelineStatus? PipelineStatus { get; set; }
    }
}
