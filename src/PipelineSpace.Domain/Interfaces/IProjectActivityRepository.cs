using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IProjectActivityRepository : IRepository<ProjectActivity>
    {
        Task<ProjectActivity> GetProjectActivityById(Guid organizationId, Guid projectId, string code);
    }
}
