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
    public class ProjectUserInvitationSqlServerRepository : Repository<ProjectUserInvitation>, IProjectUserInvitationRepository
    {
        private readonly PipelineSpaceDbContext _context;
        public ProjectUserInvitationSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<List<ProjectUserInvitation>> GetProjectUserInvitationByEmail(string userEmail)
        {
            return await _context.ProjectUserInvitations.Where(x => x.UserEmail.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase)).ToListAsync();
        }

        public async Task<ProjectUserInvitation> GetProjectUserInvitationById(Guid projectUserInvitationId)
        {
            return await _context.ProjectUserInvitations.FirstOrDefaultAsync(x => x.ProjectUserInvitationId == projectUserInvitationId);
        }
        
    }
}
