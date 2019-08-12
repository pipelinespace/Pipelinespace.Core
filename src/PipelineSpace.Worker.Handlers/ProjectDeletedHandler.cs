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
    public class ProjectDeletedHandler : IEventHandler<ProjectDeletedEvent>
    {
        readonly Func<ConfigurationManagementService, IProjectHandlerService> _projectHandlerService;
        public ProjectDeletedHandler(Func<ConfigurationManagementService, IProjectHandlerService> projectHandlerService)
        {
            _projectHandlerService = projectHandlerService;
        }
        
        public async Task Handle(ProjectDeletedEvent @event)
        {
            if (@event.IsImported)
            {
                return;
            }

            await _projectHandlerService(@event.CMSType).DeleteProject(@event);
        }
    }
}
