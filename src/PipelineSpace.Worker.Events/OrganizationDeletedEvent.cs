using System.Collections.Generic;

namespace PipelineSpace.Worker.Events
{
    public class OrganizationDeletedEvent: BaseEvent
    {
        public OrganizationDeletedEvent(string correlationId) : base(correlationId)
        {

        }

        public List<ProjectDeletedEvent> Projects { get; set; }
    }
}
