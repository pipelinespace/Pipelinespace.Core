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
using PipelineSpace.Infra.CrossCutting.Extensions;
using PipelineSpace.Domain.Models;
using PipelineSpace.Application.Interfaces.Models;

namespace PipelineSpace.Application.Services
{
    public class ProjectFeatureServiceQueryService : IProjectFeatureServiceQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;
        readonly IDataProtectorService _dataProtectorService;
        readonly Func<ConfigurationManagementService, ICMSQueryService> _cmsQueryService;

        public ProjectFeatureServiceQueryService(IDomainManagerService domainManagerService,
                                                 IIdentityService identityService,
                                                 IDataProtectorService dataProtectorService,
                                                 Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService,
                                                 Func<ConfigurationManagementService, ICMSQueryService> cmsQueryService,
                                                 IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
            _cmsCredentialService = cmsCredentialService;
            _dataProtectorService = dataProtectorService;
            _cmsQueryService = cmsQueryService;
        }

        public async Task<ProjectFeatureServiceListRp> GetProjectFeatureServices(Guid organizationId, Guid projectId, Guid featureId)
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

            ProjectFeatureServiceListRp list = new ProjectFeatureServiceListRp();

            if (feature.Services != null)
            {
                list.Items = feature.Services.Select(x => new ProjectFeatureServiceListItemRp()
                {
                    ProjectFeatureId = x.ProjectFeatureId,
                    ProjectServiceId = x.ProjectServiceId,
                    Name = x.ProjectService.Name,
                    Description = x.ProjectService.Description,
                    Template = x.ProjectService.ProjectServiceTemplate.Name,
                    Status = x.Status,
                    PipelineStatus = x.PipelineStatus
                }).ToList();
            }

            return list;
        }

        public async Task<ProjectFeatureServiceSummaryGetRp> GetProjectFeatureServiceSummaryById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
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
                return null;
            }

            ProjectFeatureServiceSummaryGetRp featureServiceSummaryRp = new ProjectFeatureServiceSummaryGetRp
            {
                Name = featureService.ProjectService.Name,
                Description = featureService.ProjectService.Description,
                Status = featureService.Status,
                PipeType = featureService.ProjectService.PipeType,
                PipelineStatus = featureService.PipelineStatus,
                LastPipelineBuildStatus = featureService.LastPipelineBuildStatus,
                LastPipelineReleaseStatus = featureService.LastPipelineReleaseStatus,
                Activities = new ProjectFeatureServiceActivityListRp()
                {
                    Items = featureService.Activities.OrderBy(x => x.CreationDate).Select(x => new ProjectFeatureServiceActivityListItemRp()
                    {
                        Name = x.Name,
                        Log = x.Log,
                        CreationDate = x.CreationDate,
                        ActivityStatus = x.ActivityStatus,
                        
                    }).ToList()
                },
                Events = new ProjectFeatureServiceEventListRp()
                {
                    Items = featureService.Events.OrderByDescending(x => x.EventDate).Take(6).Select(x => new ProjectFeatureServiceEventListItemRp()
                    {
                        EventDescription = x.EventDescription,
                        EventType = x.EventType,
                        EventStatus = x.EventStatus,
                        CreationDate = x.CreationDate,
                        EventDate = x.EventDate
                    }).ToList()
                }
            };

            return featureServiceSummaryRp;
        }

        public async Task<ProjectFeatureServicePipelineGetRp> GetProjectFeatureServicePipelineById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
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
                return null;
            }

            ProjectFeatureServicePipelineGetRp featureServicePipelineRp = new ProjectFeatureServicePipelineGetRp
            {
                Name = featureService.ProjectService.Name,
                Description = featureService.ProjectService.Description,
                Status = featureService.Status,
                PipeType = featureService.ProjectService.PipeType,
                Phases = new List<ProjectFeatureServicePipelinePhaseGetRp>()
            };

            featureServicePipelineRp.Phases.Add(new ProjectFeatureServicePipelinePhaseGetRp()
            {
                Type = "Build",
                Name = "Build",
                Rank = 1,
                LastStatusCode = featureService.LastPipelineBuildStatus.ToString(),
                LastStatusDescription = featureService.LastPipelineBuildStatus.GetDescription(),
                LastVersionId = featureService.LastBuildVersionId,
                LastVersionName = featureService.LastBuildVersionName,
                LastSuccessVersionId = featureService.LastBuildSuccessVersionId,
                LastSuccessVersionName = featureService.LastBuildSuccessVersionName,
                LastApprovalId = string.Empty
            });

            foreach (var environment in featureService.Environments)
            {
                featureServicePipelineRp.Phases.Add(new ProjectFeatureServicePipelinePhaseGetRp()
                {
                    Type = "Release",
                    Name = environment.ProjectFeatureEnvironment.Name,
                    Rank = environment.ProjectFeatureEnvironment.Rank + 1,
                    LastStatusCode = string.IsNullOrEmpty(environment.LastStatusCode) ? PipelineReleaseStatus.Pending.ToString() : environment.LastStatusCode,
                    LastStatusDescription = string.IsNullOrEmpty(environment.LastStatus) ? PipelineReleaseStatus.Pending.ToString() : environment.LastStatus,
                    LastVersionId = environment.LastVersionId,
                    LastVersionName = environment.LastVersionName,
                    LastSuccessVersionId = environment.LastSuccessVersionId,
                    LastSuccessVersionName = environment.LastSuccessVersionName,
                    LastApprovalId = environment.LastApprovalId
                });
            }

            //Order
            featureServicePipelineRp.Phases = featureServicePipelineRp.Phases.OrderBy(x => x.Rank).ToList();

            return featureServicePipelineRp;
        }

        public async Task<ProjectFeatureAllServiceListRp> GetProjectFeatureAllServices(Guid organizationId, Guid projectId, Guid featureId)
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

            ProjectFeatureAllServiceListRp list = new ProjectFeatureAllServiceListRp();

            var projectServices = project.GetServices();
            foreach (var projectService in projectServices)
            {
                var featureService = feature.GetFeatureServiceById(projectService.ProjectServiceId);

                var item = new ProjectFeatureAllServiceListItemRp()
                {
                    ProjectServiceId = projectService.ProjectServiceId,
                    Name = projectService.Name
                };
                
                if (featureService != null)
                {
                    item.IsFeatureService = true;
                }

                list.Items.Add(item);
            }

            return list;
        }

        public async Task<ProjectFeatureServiceExternalGetRp> GetProjectFeatureServiceExternalById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
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

            CMSAuthCredentialModel cmsAuthCredential = null;
            var serviceExternalName = string.Empty;
            var projectExternalId = project.ProjectExternalId;
            var service = feature.Services.FirstOrDefault(c => c.ProjectServiceId.Equals(serviceId)).ProjectService;
            //Auth
            cmsAuthCredential = this._cmsCredentialService(service.OrganizationCMS.Type).GetToken(
                                                                _dataProtectorService.Unprotect(service.OrganizationCMS.AccountId),
                                                                _dataProtectorService.Unprotect(service.OrganizationCMS.AccountName),
                                                                _dataProtectorService.Unprotect(service.OrganizationCMS.AccessSecret),
                                                                _dataProtectorService.Unprotect(service.OrganizationCMS.AccessToken));
            
            switch (service.OrganizationCMS.Type)
            {
                case ConfigurationManagementService.VSTS:
                    serviceExternalName = service.ProjectServiceExternalName;
                    projectExternalId = project.ProjectExternalId;
                    break;
                case ConfigurationManagementService.Bitbucket:
                    serviceExternalName = service.ProjectServiceExternalId;
                    projectExternalId = project.OrganizationExternalId;
                    break;
                case ConfigurationManagementService.GitHub:
                    serviceExternalName = service.ProjectServiceExternalName;
                    projectExternalId = project.ProjectExternalId;
                    break;
                case ConfigurationManagementService.GitLab:
                    serviceExternalName = service.ProjectServiceExternalId;
                    projectExternalId = project.ProjectExternalId;
                    break;
                default:
                    break;
            }

            if (service.IsImported)
            {
                projectExternalId = service.ProjectExternalId;
            }

            var serviceExternal = await this._cmsQueryService(service.OrganizationCMS.Type).GetRepository(projectExternalId, serviceExternalName, cmsAuthCredential);

            return new ProjectFeatureServiceExternalGetRp
            {
                DefaultBranch = feature.Name,
                GitUrl = serviceExternal.Link,
                SSHUrl = serviceExternal.SSHUrl
            };
        }
    }
}
