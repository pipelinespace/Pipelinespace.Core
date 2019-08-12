using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Events
{
    public class ProjectServiceTemplateCreatedEvent : BaseEvent
    {
        public ProjectServiceTemplateCreatedEvent(string correlationId) : base(correlationId)
        {

        }

        public Guid OrganizationId { get; set; }
        public Guid ProjectServiceTemplateId { get; set; }

        public string SourceTemplateUrl { get; set; }
        
        public TemplateAccess TemplateAccess { get; set; }
        public bool NeedCredentials { get; set; }
        public ConfigurationManagementService RepositoryCMSType { get; set; }
        public string RepositoryAccessId { get; set; }
        public string RepositoryAccessSecret { get; set; }
        public string RepositoryAccessToken { get; set; }
        public string RepositoryUrl { get; set; }
    }
}
