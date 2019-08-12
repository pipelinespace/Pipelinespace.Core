using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Events
{
    public class ProjectServiceBuildQueuedEvent : BaseEvent
    {
        public ProjectServiceBuildQueuedEvent(string correlationId) : base(correlationId)
        {

        }

        /// <summary>
        /// Organization Id
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Project Id
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Service Id
        /// </summary>
        public Guid ServiceId { get; set; }

    }
}
