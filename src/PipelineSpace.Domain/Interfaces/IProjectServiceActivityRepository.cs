using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IProjectServiceActivityRepository : IRepository<ProjectServiceActivity>
    {
        Task<ProjectServiceActivity> GetProjectServiceActivityById(Guid organizationId, Guid projectId, Guid serviceId, string code);
    }
}
