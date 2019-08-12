using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IOrganizationRepository : IRepository<Organization>
    {
        Task<Organization> GetOrganizationById(Guid organizationId);
        Task<List<Project>> GetProjects(Guid organizationId);
    }
}
