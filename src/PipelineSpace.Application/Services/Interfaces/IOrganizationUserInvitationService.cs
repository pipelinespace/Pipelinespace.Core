using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IOrganizationUserInvitationService
    {
        Task InviteUser(Guid organizationId, OrganizationUserInvitationPostRp resource);
        Task CancelInvitation(Guid organizationId, Guid invitationId);
        Task AcceptInvitation(Guid organizationId, Guid invitationId);
    }
}
