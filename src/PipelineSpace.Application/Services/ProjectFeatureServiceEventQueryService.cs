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
    public class ProjectFeatureServiceEventQueryService : IProjectFeatureServiceEventQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;

        public ProjectFeatureServiceEventQueryService(IDomainManagerService domainManagerService,
                                                      IIdentityService identityService,
                                                      IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
        }

        public async Task<ProjectFeatureServiceEventListRp> GetProjectFeatureServiceEvents(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, BaseEventType baseEventType)
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

            DomainModels.ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return null;
            }

            DomainModels.ProjectFeatureService featureService = feature.GetFeatureServiceById(serviceId);
            if (featureService == null)
            {
                await _domainManagerService.AddNotFound($"The project feature pipe with id {serviceId} does not exists.");
                return null;
            }

            ProjectFeatureServiceEventListRp list = new ProjectFeatureServiceEventListRp();

            if (featureService.Events != null)
            {
                if(baseEventType == BaseEventType.None)
                {
                    list.Items = featureService.Events.Select(x => new ProjectFeatureServiceEventListItemRp
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
                    list.Items = featureService.Events.Where(x=> x.BaseEventType == baseEventType).Select(x => new ProjectFeatureServiceEventListItemRp
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
