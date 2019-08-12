using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface IOrganizationUserService
    {
        Task RemoveUser(Guid organizationId, string userId);
    }
}
