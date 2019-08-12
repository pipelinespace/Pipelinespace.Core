using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Events
{
    public class OrganizationUserInvitedEvent : BaseEvent
    {
        public OrganizationUserInvitedEvent(string correlationId) : base(correlationId)
        {

        }

        public Guid OrganizationUserInvitationId { get; set; }

        public string RequestorFullName { get; set; }
        
        public string UserFullName { get; set; }
        
        public string UserEmail { get; set; }

        public PipelineRole Role { get; set; }

        public UserInvitationType InvitationType { get; set; }
    }
}
