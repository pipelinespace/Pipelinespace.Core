using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Interfaces
{
    public interface IProjectTemplateRepository
    {
        Task<ProjectTemplate> GetProjectTemplateById(Guid projectTemplateId);
        Task<List<ProjectTemplate>> GetProjectTemplates(CloudProviderService cloudProviderType);
    }
}
