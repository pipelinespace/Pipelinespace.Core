using PipelineSpace.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectUserInvitationQueryService
    {
        Task<ProjectUserInvitationListRp> GetUserInvitations();
        Task<ProjectUserInvitationListRp> GetInvitations(Guid organizationId, Guid ProjectId);
        Task<ProjectUserInvitationGetRp> GetInvitationById(Guid invitationId);
    }
}
