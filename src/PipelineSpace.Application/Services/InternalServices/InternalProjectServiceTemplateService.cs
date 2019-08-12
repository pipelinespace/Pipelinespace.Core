using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.InternalServices.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Worker.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices
{
    public class InternalProjectServiceTemplateService : IInternalProjectServiceTemplateService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectServiceTemplateRepository _projectServiceTemplateRepository;
        
        public InternalProjectServiceTemplateService(IDomainManagerService domainManagerService,
                                                     IProjectServiceTemplateRepository projectServiceTemplateRepository)
        {
            _domainManagerService = domainManagerService;
            _projectServiceTemplateRepository = projectServiceTemplateRepository;
        }

        public async Task ActivateProjectServiceTemplate(Guid organizationId, Guid projectServiceTemplateId)
        {
            var template = await _projectServiceTemplateRepository.GetPendingProjectServiceTemplateById(projectServiceTemplateId);

            if (template == null)
            {
                await _domainManagerService.AddNotFound($"The template with id {projectServiceTemplateId} does not exists.");
                return;
            }

            if (template.Status != EntityStatus.Preparing)
            {
                await _domainManagerService.AddConflict($"The template with id {projectServiceTemplateId} must be in status NEW to be activated.");
                return;
            }

            template.Activate();

            _projectServiceTemplateRepository.Update(template);

            await _projectServiceTemplateRepository.SaveChanges();
        }
    }
}
