using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectUserInvitationService
    {
        Task InviteUser(Guid organizationId, Guid projectId, ProjectUserInvitationPostRp resource);
        Task CancelInvitation(Guid organizationId, Guid projectId, Guid invitationId);
        Task AcceptInvitation(Guid organizationId, Guid projectId, Guid invitationId);
    }
}
