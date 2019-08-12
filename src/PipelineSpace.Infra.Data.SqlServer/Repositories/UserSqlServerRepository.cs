using Microsoft.EntityFrameworkCore;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Data.SqlServer.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.SqlServer.Repositories
{
    public class UserSqlServerRepository : Repository<User>, IUserRepository 
    {
        private readonly PipelineSpaceDbContext _context;
        public UserSqlServerRepository(PipelineSpaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUser(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x=> x.Id.Equals(userId, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<User> GetUserByEmail(string userEmail)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
