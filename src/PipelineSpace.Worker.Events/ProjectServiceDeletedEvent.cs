using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Events
{
    public class ProjectServiceDeletedEvent : BaseEvent
    {
        public ProjectServiceDeletedEvent(string correlationId) : base(correlationId)
        {

        }

        public string OrganizationExternalId { get; set; }
        public string OrganizationName { get; set; }
        public string ProjectName { get; set; }
        public string ServiceName { get; set; }
        public string ProjectVSTSFakeName { get; set; }

        public string ProjectExternalId { get; set; }
        public string ProjectServiceExternalId { get; set; }
        public int? CommitStageId { get; set; }
        public int? ReleaseStageId { get; set; }
        public Guid? CommitServiceHookId { get; set; }
        public Guid? ReleaseServiceHookId { get; set; }
        public Guid? CodeServiceHookId { get; set; }
        public Guid? ReleaseStartedServiceHookId { get; set; }
        public Guid? ReleasePendingApprovalServiceHookId { get; set; }
        public Guid? ReleaseCompletedApprovalServiceHookId { get; set; }
        
        public List<string> Environments { get; set; }

        public ConfigurationManagementService CMSType { get; set; }
        public string CMSAccountId { get; set; }
        public string CMSAccountName { get; set; }
        public string CMSAccessId { get; set; }
        public string CMSAccessSecret { get; set; }
        public string CMSAccessToken { get; set; }

        public CloudProviderService CPSType { get; set; }
        public string CPSAccessId { get; set; }
        public string CPSAccessName { get; set; }
        public string CPSAccessSecret { get; set; }
        public string CPSAccessRegion { get; set; }
        public string CPSAccessAppId { get; set; }
        public string CPSAccessAppSecret { get; set; }
        public string CPSAccessDirectory { get; set; }

        public SourceEvent SourceEvent { get; set; }
    }
}
