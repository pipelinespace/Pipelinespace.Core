using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IProjectServiceRepository : IRepository<ProjectService>
    {
        Task<ProjectService> GetProjectServiceById(Guid organizationId, Guid projectId, Guid serviceId);
    }
}
