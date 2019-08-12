using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IProjectUserService
    {
        Task RemoveUser(Guid organizationId, Guid projectId, string userId);
    }
}
