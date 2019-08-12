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
    public class ProjectServiceActivityQueryService : IProjectServiceActivityQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;

        public ProjectServiceActivityQueryService(IDomainManagerService domainManagerService,
                                   IIdentityService identityService,
                                   IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
        }

        public async Task<ProjectServiceActivityListRp> GetProjectServiceActivities(Guid organizationId, Guid projectId, Guid serviceId)
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

            ProjectServiceActivityListRp list = new ProjectServiceActivityListRp();

            if (service.Activities != null)
            {
                list.Items = service.Activities.Select(x => new ProjectServiceActivityListItemRp
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
