using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Events
{
    public class ProjectUserInvitedEvent : BaseEvent
    {
        public ProjectUserInvitedEvent(string correlationId) : base(correlationId)
        {

        }
        
        public Guid ProjectUserInvitationId { get; set; }

        public string RequestorFullName { get; set; }
        
        public string UserFullName { get; set; }
        
        public string UserEmail { get; set; }

        public PipelineRole Role { get; set; }

        public UserInvitationType InvitationType { get; set; }
    }
}
