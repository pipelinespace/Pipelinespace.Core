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
    public class ProjectFeatureServiceEventService : IProjectFeatureServiceEventService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;

        public ProjectFeatureServiceEventService(IDomainManagerService domainManagerService,
                                                 IIdentityService identityService,
                                                 IOrganizationRepository organizationRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
        }

        public async Task CreateProjectFeatureServiceEvent(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, ProjectFeatureServiceEventPostRp resource)
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

            DomainModels.ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return;
            }

            DomainModels.ProjectFeatureService featureService = feature.GetFeatureServiceById(serviceId);
            if (featureService == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return;
            }

            featureService.AddEvent(BaseEventType.Build, resource.GetEventType().GetDescription(), resource.Message.Text, resource.Status, JsonConvert.SerializeObject(resource.DetailedMessage), JsonConvert.SerializeObject(resource.DetailedMessage), JsonConvert.SerializeObject(resource.Resource), resource.Date);

            _organizationRepository.Update(organization);

            await _organizationRepository.SaveChanges();
        }
    }
}
