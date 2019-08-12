using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using System;
using DomainModels = PipelineSpace.Domain.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace PipelineSpace.Application.Services
{
    public class ProjectFeatureQueryService : IProjectFeatureQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;

        public ProjectFeatureQueryService(IDomainManagerService domainManagerService,
                                          IIdentityService identityService,
                                          IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
        }

        public async Task<ProjectFeatureListRp> GetProjectFeatures(Guid organizationId, Guid projectId)
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

            ProjectFeatureListRp list = new ProjectFeatureListRp();

            if (project.Features != null)
            {
                list.Items = project.Features.Select(x => new ProjectFeatureListItemRp()
                {
                    ProjectFeatureId = x.ProjectFeatureId,
                    Name = x.Name,
                    Description = x.Description,
                    StartDate = x.CreationDate,
                    CompletionDate = x.CompletionDate,
                    Status = x.GetStatusName()
                }).ToList();
            }

            return list;
        }

        public async Task<ProjectFeatureGetRp> GetProjectFeatureById(Guid organizationId, Guid projectId, Guid featureId)
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
                return null;
            }

            ProjectFeatureGetRp featureRp = new ProjectFeatureGetRp
            {
                ProjectFeatureId = feature.ProjectFeatureId,
                Name = feature.Name,
                Description = feature.Description,
                StartDate = feature.CreationDate,
                CompletionDate = feature.CompletionDate,
                Status = feature.GetStatusName()
            };

            return featureRp;
        }
    }
}
