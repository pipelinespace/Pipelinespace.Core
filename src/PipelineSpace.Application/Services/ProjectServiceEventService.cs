using Newtonsoft.Json;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.CrossCutting.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using DomainModels = PipelineSpace.Domain.Models;
namespace PipelineSpace.Application.Services
{
    public class ProjectServiceEventService : IProjectServiceEventService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;

        public ProjectServiceEventService(IDomainManagerService domainManagerService,
                                   IIdentityService identityService,
                                   IOrganizationRepository organizationRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
        }

        public async Task CreateProjectServiceEvent(Guid organizationId, Guid projectId, Guid serviceId, ProjectServiceEventPostRp resource)
        {
            DomainModels.Organization organization = await _organizationRepository.GetOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            DomainModels.Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            DomainModels.ProjectService service = project.GetServiceById(serviceId);
            if (service == null)
            {
                await _domainManagerService.AddNotFound($"The project pipe with id {serviceId} does not exists.");
                return;
            }
            
            service.AddEvent(BaseEventType.Build, resource.GetEventType().GetDescription(), resource.Message.Text, resource.Status, JsonConvert.SerializeObject(resource.DetailedMessage), JsonConvert.SerializeObject(resource.DetailedMessage), JsonConvert.SerializeObject(resource.Resource), resource.Date);

            _organizationRepository.Update(organization);

            await _organizationRepository.SaveChanges();

        }
    }
}
