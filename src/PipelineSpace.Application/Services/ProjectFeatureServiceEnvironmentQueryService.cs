using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using DomainModels = PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using PipelineSpace.Application.Interfaces.Models;

namespace PipelineSpace.Application.Services
{
    public class ProjectFeatureServiceEnvironmentQueryService : IProjectFeatureServiceEnvironmentQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly Func<DomainModels.CloudProviderService, ICPSQueryService> _cpsQueryService;
        readonly IDataProtectorService _dataProtectorService;

        public ProjectFeatureServiceEnvironmentQueryService(IDomainManagerService domainManagerService,
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

        public async Task<ProjectFeatureServiceEnvironmentListRp> GetFeatureProjectServiceEnvironments(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
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

            DomainModels.ProjectFeature projectFeature = project.GetFeatureById(featureId);
            if (projectFeature == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return null;
            }

            DomainModels.ProjectFeatureService projectFeatureService = projectFeature.GetFeatureServiceById(serviceId);
            if (projectFeatureService == null)
            {
                await _domainManagerService.AddNotFound($"The feature service with id {serviceId} does not exists.");
                return null;
            }

            ProjectFeatureServiceEnvironmentListRp list = new ProjectFeatureServiceEnvironmentListRp();

            CPSAuthCredentialModel authCredentials = new CPSAuthCredentialModel();
            authCredentials.AccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId);
            authCredentials.AccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName);
            authCredentials.AccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret);
            authCredentials.AccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion);
            authCredentials.AccessAppId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppId);
            authCredentials.AccessAppSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppSecret);
            authCredentials.AccessDirectory = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessDirectory);
            
            if (projectFeatureService.Environments != null)
            {
                List<ProjectFeatureServiceEnvironmentListItemRp> projectFeatureServiceEnvironmentList = new List<ProjectFeatureServiceEnvironmentListItemRp>();
                var environments = projectFeatureService.Environments.OrderBy(x => x.ProjectFeatureEnvironment.Rank);
                foreach (var item in environments)
                {
                    var projectServiceEnvironmentListItem = new ProjectFeatureServiceEnvironmentListItemRp();
                    projectServiceEnvironmentListItem.ProjectFeatureServiceEnvironmentId = item.ProjectFeatureServiceEnvironmentId;
                    projectServiceEnvironmentListItem.Name = item.ProjectFeatureEnvironment.Name;
                    projectServiceEnvironmentListItem.Status = item.Status;
                    projectServiceEnvironmentListItem.Summary = await _cpsQueryService(project.OrganizationCPS.Type).GetEnvironmentSummary(organization.Name, project.Name, projectFeatureService.ProjectService.Name, item.ProjectFeatureEnvironment.Name, projectFeature.Name, authCredentials);
                    projectServiceEnvironmentListItem.Variables = item.Variables.Select(p => new ProjectFeatureServiceEnvironmentVariableListItemRp()
                    {
                        Name = p.Name,
                        Value = p.Value
                    }).ToList();
                    projectFeatureServiceEnvironmentList.Add(projectServiceEnvironmentListItem);
                }
                list.Items = projectFeatureServiceEnvironmentList;
            }

            return list;
        }
    }
}
