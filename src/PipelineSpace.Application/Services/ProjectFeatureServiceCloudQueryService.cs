using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Application.Services.Interfaces;
using DomainModels = PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using System.Linq;
using PipelineSpace.Domain;

namespace PipelineSpace.Application.Services
{
    public class ProjectFeatureServiceCloudQueryService : IProjectFeatureServiceCloudQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly Func<DomainModels.CloudProviderService, ICPSQueryService> _cpsQueryService;
        readonly IDataProtectorService _dataProtectorService;

        public ProjectFeatureServiceCloudQueryService(IDomainManagerService domainManagerService, 
                                                      IIdentityService identityService, 
                                                      IUserRepository userRepository, 
                                                      Func<DomainModels.CloudProviderService, ICPSQueryService> cpsQueryService,
                                                      IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
            _cpsQueryService = cpsQueryService;
            _dataProtectorService = dataProtectorService;
        }

        public async Task<CPSCloudResourceSummaryModel> GetProjectFeatureServiceCloudSummary(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            string loggedUserId = _identityService.GetUserId();

            DomainModels.User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id ${organizationId} does not exists.");
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

            var environments = new List<string>() { DomainConstants.Environments.Development };

            CPSAuthCredentialModel authCredentials = new CPSAuthCredentialModel();
            authCredentials.AccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId);
            authCredentials.AccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName);
            authCredentials.AccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret);
            authCredentials.AccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion);
            authCredentials.AccessAppId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppId);
            authCredentials.AccessAppSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppSecret);
            authCredentials.AccessDirectory = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessDirectory);

            var summary = await _cpsQueryService(project.OrganizationCPS.Type).GetSummary(organization.Name, project.Name, featureService.ProjectService.Name, environments, feature.Name, authCredentials);

            return summary;
        }

    }
}
