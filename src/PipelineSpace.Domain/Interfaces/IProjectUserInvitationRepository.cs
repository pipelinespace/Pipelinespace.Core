using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IProjectUserInvitationRepository : IRepository<ProjectUserInvitation>
    {
        Task<List<ProjectUserInvitation>> GetProjectUserInvitationByEmail(string userEmail);
        Task<ProjectUserInvitation> GetProjectUserInvitationById(Guid projectUserInvitationId);
    }
}
