using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using DomainModels = PipelineSpace.Domain.Models;
namespace PipelineSpace.Application.Services
{
    public class ProjectServiceEventQueryService : IProjectServiceEventQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;

        public ProjectServiceEventQueryService(IDomainManagerService domainManagerService,
                                   IIdentityService identityService,
                                   IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
        }

        public async Task<ProjectServiceEventListRp> GetProjectServiceEvents(Guid organizationId, Guid projectId, Guid serviceId, BaseEventType baseEventType)
        {
            string loggedUserId = _identityService.GetUserId();

            DomainModels.User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            DomainModels.Project project = user.FindProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return null;
            }

            DomainModels.ProjectService service = project.GetServiceById(serviceId);
            if (service == null)
            {
                await _domainManagerService.AddNotFound($"The project pipe with id {serviceId} does not exists.");
                return null;
            }

            ProjectServiceEventListRp list = new ProjectServiceEventListRp();

            if (service.Events != null)
            {
                if(baseEventType == BaseEventType.None)
                {
                    list.Items = service.Events.Select(x => new ProjectServiceEventListItemRp
                    {
                        EventType = x.EventType,
                        EventDescription = x.EventDescription,
                        EventStatus = x.EventStatus,
                        CreationDate = x.CreationDate,
                        EventDate = x.EventDate
                    }).OrderByDescending(x => x.EventDate).Take(10).ToList();
                }
                else
                {
                    list.Items = service.Events.Where(x=> x.BaseEventType == baseEventType).Select(x => new ProjectServiceEventListItemRp
                    {
                        EventType = x.EventType,
                        EventDescription = x.EventDescription,
                        EventStatus = x.EventStatus,
                        CreationDate = x.CreationDate,
                        EventDate = x.EventDate
                    }).OrderByDescending(x => x.EventDate).Take(10).ToList();
                }

            }

            return list;

        }
    }
}
