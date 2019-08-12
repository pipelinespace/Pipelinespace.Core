using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using DomainModels = PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PipelineSpace.Infra.CrossCutting.Extensions;
using PipelineSpace.Domain.Models;
using PipelineSpace.Application.Interfaces.Models;

namespace PipelineSpace.Application.Services
{
    public class ProjectServiceQueryService : IProjectServiceQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IProjectServiceRepository _projectServiceRepository;
        readonly IUserRepository _userRepository;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;
        readonly IDataProtectorService _dataProtectorService;
        readonly Func<ConfigurationManagementService, ICMSQueryService> _cmsQueryService;

        public ProjectServiceQueryService(IDomainManagerService domainManagerService,
                                          IIdentityService identityService,
                                          IProjectServiceRepository projectServiceRepository,
                                          IUserRepository userRepository,
                                          Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService,
                                   Func<ConfigurationManagementService, ICMSQueryService> cmsQueryService,
                                   IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _projectServiceRepository = projectServiceRepository;
            _userRepository = userRepository;
            this._dataProtectorService = dataProtectorService;
            this._cmsQueryService = cmsQueryService;
            _cmsCredentialService = cmsCredentialService;
        }

        public async Task<ProjectServiceListRp> GetProjectServices(Guid organizationId, Guid projectId)
        {
            string loggedUserId = _identityService.GetUserId();

            DomainModels.User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            DomainModels.Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return null;
            }

            ProjectServiceListRp list = new ProjectServiceListRp();

            if(project.Services != null)
            {
                list.Items = project.Services.Select(x => new ProjectServiceListItemRp()
                {
                    ProjectServiceId = x.ProjectServiceId,
                    Name = x.Name,
                    Description = x.Description,
                    Template = x.ProjectServiceTemplate.Name,
                    Status = x.Status,
                    PipelineStatus = x.PipelineStatus,
                    CreationDate = x.CreationDate,
                    PipeType = x.ProjectServiceTemplate.PipeType
                }).ToList();
            }
            
            return list;
        }

        public async Task<ProjectServiceGetRp> GetProjectServiceById(Guid organizationId, Guid projectId, Guid serviceId)
        {
            string loggedUserId = _identityService.GetUserId();

            DomainModels.User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            DomainModels.Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return null;
            }

            DomainModels.ProjectService service = project.GetServiceById(serviceId);
            if (service == null)
            {
                return null;
            }

            ProjectServiceGetRp serviceRp = new ProjectServiceGetRp
            {
                ProjectServiceId = service.ProjectServiceId,
                Name = service.Name,
                Description = service.Description,
                Template = service.ProjectServiceTemplate.Name,
                Status = service.Status,
                PipelineStatus = service.PipelineStatus,
                ServiceExternalUrl = service.ProjectServiceExternalUrl,
                GitProviderType = service.OrganizationCMS.Type,
            };

            return serviceRp;
        }

        public async Task<ProjectServiceSummaryGetRp> GetProjectServiceSummaryById(Guid organizationId, Guid projectId, Guid serviceId)
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
                return null;
            }

            ProjectServiceSummaryGetRp serviceSummaryRp = new ProjectServiceSummaryGetRp
            {
                Name = service.Name,
                Description = service.Description,
                Status = service.Status,
                PipeType = service.PipeType,
                PipelineStatus = service.PipelineStatus,
                LastPipelineBuildStatus = service.LastPipelineBuildStatus,
                LastPipelineReleaseStatus = service.LastPipelineReleaseStatus,
                Activities = new ProjectServiceActivityListRp()
                {
                    Items = service.Activities.OrderBy(x => x.CreationDate).Select(x => new ProjectServiceActivityListItemRp() {
                        Name = x.Name,
                        Log = x.Log,
                        CreationDate = x.CreationDate,
                        ActivityStatus = x.ActivityStatus
                    }).ToList()
                },
                Events = new ProjectServiceEventListRp()
                {
                    Items = service.Events.OrderByDescending(x => x.EventDate).Take(6).Select(x => new ProjectServiceEventListItemRp()
                    {
                        EventDescription = x.EventDescription,
                        EventType = x.EventType,
                        EventStatus = x.EventStatus,
                        CreationDate = x.CreationDate,
                        EventDate = x.EventDate
                    }).ToList()
                }
            };

            return serviceSummaryRp;
        }

        public async Task<ProjectServicePipelineGetRp> GetProjectServicePipelineById(Guid organizationId, Guid projectId, Guid serviceId)
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
                return null;
            }

            ProjectServicePipelineGetRp servicePipelineRp = new ProjectServicePipelineGetRp
            {
                Name = service.Name,
                Description = service.Description,
                Status = service.Status,
                PipeType = service.PipeType,
                Phases = new List<ProjectServicePipelinePhaseGetRp>()
            };

            servicePipelineRp.Phases.Add(new ProjectServicePipelinePhaseGetRp() {
                Type = "Build",
                Name = "Build",
                Rank = 1,
                LastStatusCode = service.LastPipelineBuildStatus.ToString(),
                LastStatusDescription = service.LastPipelineBuildStatus.GetDescription(),
                LastVersionId = service.LastBuildVersionId,
                LastVersionName = service.LastBuildVersionName,
                LastSuccessVersionId = service.LastBuildSuccessVersionId,
                LastSuccessVersionName = service.LastBuildSuccessVersionName,
                LastApprovalId = string.Empty
            });

            foreach (var environment in service.Environments)
            {
                servicePipelineRp.Phases.Add(new ProjectServicePipelinePhaseGetRp()
                {
                    Type = "Release",
                    Name = environment.ProjectEnvironment.Name,
                    Rank = environment.ProjectEnvironment.Rank + 1,
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
            servicePipelineRp.Phases = servicePipelineRp.Phases.OrderBy(x => x.Rank).ToList();

            return servicePipelineRp;
        }

        public async Task<ProjectServiceFeatureListRp> GetProjectServiceFeaturesById(Guid organizationId, Guid projectId, Guid serviceId)
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
                return null;
            }

            ProjectServiceFeatureListRp featureListRp = new ProjectServiceFeatureListRp
            {
                Items = service.Features.Select(x=> new ProjectServiceFeatureListItemRp() {
                    FeatureId = x.ProjectFeatureId,
                    FeatureName = x.ProjectFeature.Name
                }).ToList()
            };

            return featureListRp;
        }

        public async Task<ProjectServiceExternalGetRp> GetProjectServiceExternalById(Guid organizationId, Guid projectId, Guid serviceId)
        {
            string loggedUserId = _identityService.GetUserId();

            DomainModels.User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            DomainModels.Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return null;
            }

            DomainModels.ProjectService service = project.GetServiceById(serviceId);
            if (service == null)
            {
                return null;
            }

            CMSAuthCredentialModel cmsAuthCredential = null;
            var serviceExternalName = string.Empty;
            var projectExternalId = string.Empty;
            //Auth
            cmsAuthCredential = this._cmsCredentialService(service.OrganizationCMS.Type).GetToken(
                                                                _dataProtectorService.Unprotect(service.OrganizationCMS.AccountId),
                                                                _dataProtectorService.Unprotect(service.OrganizationCMS.AccountName),
                                                                _dataProtectorService.Unprotect(service.OrganizationCMS.AccessSecret),
                                                                _dataProtectorService.Unprotect(service.OrganizationCMS.AccessToken));

            
            switch (service.OrganizationCMS.Type)
            {
                case ConfigurationManagementService.VSTS:
                    serviceExternalName = service.ProjectServiceExternalId;
                    projectExternalId = service.ProjectExternalId;
                    break;
                case ConfigurationManagementService.Bitbucket:
                    serviceExternalName = service.ProjectServiceExternalId;
                    projectExternalId = project.OrganizationExternalId;
                    break;
                case ConfigurationManagementService.GitHub:
                    serviceExternalName = service.ProjectServiceExternalName;
                    projectExternalId = service.ProjectExternalId;
                    break;
                case ConfigurationManagementService.GitLab:
                    serviceExternalName = service.ProjectServiceExternalId;
                    projectExternalId = service.ProjectExternalId;
                    break;
                default:
                    break;
            }

            var serviceExternal = await this._cmsQueryService(service.OrganizationCMS.Type).GetRepository(projectExternalId, serviceExternalName, cmsAuthCredential);
            
            return new ProjectServiceExternalGetRp {
                DefaultBranch = serviceExternal.DefaultBranch,
                GitUrl = serviceExternal.Link,
                SSHUrl = serviceExternal.SSHUrl
            };
        }
    }
}
