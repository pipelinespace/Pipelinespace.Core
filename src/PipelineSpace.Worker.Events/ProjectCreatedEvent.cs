using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Events
{
    public class ProjectCreatedEvent : BaseEvent
    {
        public ProjectCreatedEvent(string correlationId) : base(correlationId)
        {

        }

        public Guid OrganizationId { get; set; }
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }
        public string InternalProjectName { get; set; }
        public string ProjectVSTSFake { get; set; }

        public string AgentPoolId { get; set; }

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
        public string CPSAccessAppId { get; set; }
        public string CPSAccessAppSecret { get; set; }
        public string CPSAccessDirectory { get; set; }

        public string UserId { get; set; }

        public ProjectTemplateCreatedEvent ProjectTemplate { get; set; }
        
    }

    public class ProjectTemplateCreatedEvent
    {
        public List<ProjectTemplateServiceCreatedEvent> Services { get; set; }
    }

    public class ProjectTemplateServiceCreatedEvent
    {
        public string Name { get; set; }
        public Guid ProjectServiceTemplateId { get; set; }
    }
}
