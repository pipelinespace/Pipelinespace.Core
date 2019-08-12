using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Events
{
    public class ProjectDeletedEvent : BaseEvent
    {
        public ProjectDeletedEvent(string correlationId, bool isImported) : base(correlationId)
        {
            this.IsImported = isImported;
        }

        public ConfigurationManagementService CMSType { get; set; }
        public string CMSAccountId { get; set; }
        public string CMSAccountName { get; set; }
        public string CMSAccessId { get; set; }
        public string CMSAccessSecret { get; set; }
        public string CMSAccessToken { get; set; }
        public string ProjectExternalId { get; set; }
        public string ProjectVSTSFakeId { get; set; }
        public string OrganizationExternalId { get; set; }
        public bool IsImported { get; set; }
    }
}
