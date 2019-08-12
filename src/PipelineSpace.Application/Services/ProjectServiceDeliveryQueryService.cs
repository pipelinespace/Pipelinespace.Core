using Newtonsoft.Json;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModels = PipelineSpace.Domain.Models;
namespace PipelineSpace.Application.Services
{
    public class ProjectServiceDeliveryQueryService : IProjectServiceDeliveryQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;

        public ProjectServiceDeliveryQueryService(IDomainManagerService domainManagerService,
                                   IIdentityService identityService,
                                   IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
        }

        public async Task<ProjectServiceDeliveryListRp> GetProjectServiceDeliveries(Guid organizationId, Guid projectId, Guid serviceId)
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
                await _domainManagerService.AddNotFound($"The pipe with id {serviceId} does not exists.");
                return null;
            }

            ProjectServiceDeliveryListRp list = new ProjectServiceDeliveryListRp();

            if (service.Deliveries != null)
            {
                list.Items = service.Deliveries
                             .OrderByDescending(x=> x.DeliveryDate)
                             .Take(5)
                             .Select(x => new ProjectServiceDeliveryListItemRp
                             {
                    VersionId = x.VersionId,
                    VersionName = x.VersionName,
                    DeliveryDate = x.DeliveryDate,
                    Data = JsonConvert.DeserializeObject<ProjectServiceDeliveryDataRp>(x.Data)
                }).ToList();
            }

            return list;

        }
    }
}
