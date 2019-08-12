using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;

namespace PipelineSpace.Worker.Events
{
    public class ProjectServiceImportedEvent : ProjectServiceCreatedEvent
    {
        public string BuildDefinitionYML { get; set; }
        public string ServiceTemplatePath { get; set; }
        public string BranchName { get; set; }
        public string ProjectExternalName { get; set; }

        public ProjectServiceImportedEvent(string correlationId) : base(correlationId)
        {
        }
        
    }
    
}
