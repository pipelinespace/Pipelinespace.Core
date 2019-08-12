using PipelineSpace.Domain.Models;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public class OrganizationDeletedHandler : IEventHandler<OrganizationDeletedEvent>
    {
        readonly Func<ConfigurationManagementService, IProjectHandlerService> _projectHandlerService;
        public OrganizationDeletedHandler(Func<ConfigurationManagementService, IProjectHandlerService> projectHandlerService)
        {
            _projectHandlerService = projectHandlerService;
        }

        public async Task Handle(OrganizationDeletedEvent @event)
        {
            foreach (var item in @event.Projects)
            {
                await _projectHandlerService(item.CMSType).DeleteProject(item);
            }
        }
    }
}
