using Microsoft.EntityFrameworkCore;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Data.SqlServer.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.SqlServer.Repositories
{
    public class OrganizationUserInvitationSqlServerRepository : Repository<OrganizationUserInvitation>, IOrganizationUserInvitationRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public OrganizationUserInvitationSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<OrganizationUserInvitation>> GetOrganizationUserInvitationByEmail(string userEmail)
        {
            return await _context.OrganizationUserInvitations.Where(x => x.UserEmail.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase)).ToListAsync();
        }

        public async Task<OrganizationUserInvitation> GetOrganizationUserInvitationById(Guid organizationUserInvitationId)
        {
            return await _context.OrganizationUserInvitations.FirstOrDefaultAsync(x => x.OrganizationUserInvitationId == organizationUserInvitationId);
        }
    }
}
