using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IOrganizationUserInvitationRepository : IRepository<OrganizationUserInvitation>
    {
        Task<OrganizationUserInvitation> GetOrganizationUserInvitationById(Guid organizationUserInvitationId);
        Task<List<OrganizationUserInvitation>> GetOrganizationUserInvitationByEmail(string userEmail);
    }
}
