using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUser(string userId);
        Task<User> GetUserByEmail(string userEmail);
    }
}
