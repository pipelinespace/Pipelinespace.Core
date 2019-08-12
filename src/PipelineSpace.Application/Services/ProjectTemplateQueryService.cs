using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class ProjectTemplateQueryService : IProjectTemplateQueryService
    {
        readonly IProjectTemplateRepository _projectTemplateRepository;

        public ProjectTemplateQueryService(IProjectTemplateRepository projectTemplateRepository)
        {
            _projectTemplateRepository = projectTemplateRepository;
        }

        public async Task<ProjectTemplateListRp> GetProjectTemplates(CloudProviderService cloudProviderType)
        {
            var projectTemplates = await _projectTemplateRepository.GetProjectTemplates(cloudProviderType);

            ProjectTemplateListRp list = new ProjectTemplateListRp
            {
                Items = projectTemplates.Select(x => new ProjectTemplateListItemRp()
                {
                    ProjectTemplateId = x.ProjectTemplateId,
                    Name = x.Name,
                    Description = x.Description,
                    Logo = x.Logo
                }).ToList()
            };

            return list;
        }
    }
}
