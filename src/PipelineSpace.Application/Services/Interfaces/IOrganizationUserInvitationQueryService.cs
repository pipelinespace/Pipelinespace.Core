using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IOrganizationUserInvitationQueryService
    {
        Task<OrganizationUserInvitationListRp> GetUserInvitations();
        Task<OrganizationUserInvitationListRp> GetInvitations(Guid organizationId);
        Task<OrganizationUserInvitationGetRp> GetInvitationById(Guid invitationId);
    }
}
