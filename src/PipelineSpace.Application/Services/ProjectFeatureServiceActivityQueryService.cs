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
    public class ProjectFeatureServiceActivityQueryService : IProjectFeatureServiceActivityQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;

        public ProjectFeatureServiceActivityQueryService(IDomainManagerService domainManagerService,
                                                         IIdentityService identityService,
                                                         IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
        }

        public async Task<ProjectFeatureServiceActivityListRp> GetProjectFeatureServiceActivities(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
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

            ProjectFeatureServiceActivityListRp list = new ProjectFeatureServiceActivityListRp();

            if (featureService.Activities != null)
            {
                list.Items = featureService.Activities.Select(x => new ProjectFeatureServiceActivityListItemRp
                {
                    Name = x.Name,
                    Log = x.Log,
                    ActivityStatus = x.ActivityStatus,
                    CreationDate = x.CreationDate
                }).OrderBy(x=> x.CreationDate).ToList();
            }

            return list;

        }
    }
}
