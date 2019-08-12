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
    public class ProjectFeatureServiceDeliveryQueryService : IProjectFeatureServiceDeliveryQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;

        public ProjectFeatureServiceDeliveryQueryService(IDomainManagerService domainManagerService,
                                                         IIdentityService identityService,
                                                         IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
        }

        public async Task<ProjectFeatureServiceDeliveryListRp> GetProjectFeatureServiceDeliveries(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
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
                await _domainManagerService.AddNotFound($"The feature with id {featureId} does not exists.");
                return null;
            }

            DomainModels.ProjectFeatureService featureService = feature.GetFeatureServiceById(serviceId);
            if (featureService == null)
            {
                await _domainManagerService.AddNotFound($"The feature pipe with id {featureId} does not exists.");
                return null;
            }

            ProjectFeatureServiceDeliveryListRp list = new ProjectFeatureServiceDeliveryListRp();

            if (featureService.Deliveries != null)
            {
                list.Items = featureService.Deliveries
                             .OrderByDescending(x=> x.DeliveryDate)
                             .Take(5)
                             .Select(x => new ProjectFeatureServiceDeliveryListItemRp
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
