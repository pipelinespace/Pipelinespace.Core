using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class ProjectQueryService : IProjectQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IProjectRepository _projectRepository;
        readonly IUserRepository _userRepository;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;
        readonly IDataProtectorService _dataProtectorService;
        readonly Func<ConfigurationManagementService, ICMSQueryService> _cmsQueryService;
        readonly IOptions<FakeAccountServiceOptions> _vstsFakeOptions;
        readonly IProjectCloudCredentialService _cloudCredentialService;

        public ProjectQueryService(IDomainManagerService domainManagerService,
                                   IIdentityService identityService,
                                   IProjectRepository projectRepository,
                                   Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService,
                                   Func<ConfigurationManagementService, ICMSQueryService> cmsQueryService,
                                   IDataProtectorService dataProtectorService,
                                   IProjectCloudCredentialService cloudCredentialService,
                                   IOptions<FakeAccountServiceOptions> vstsFakeOptions,
                                   IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _cmsCredentialService = cmsCredentialService;
            this._dataProtectorService = dataProtectorService;
            this._cmsQueryService = cmsQueryService;
            this._vstsFakeOptions = vstsFakeOptions;
            this._cloudCredentialService = cloudCredentialService;
        }

        public async Task<ProjectListRp> GetProjects(Guid organizationId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            ProjectListRp list = new ProjectListRp();

            var projects = user.FindProjects(organizationId);

            if (projects != null)
            {
                list.Items = projects.Select(x => new ProjectListItemRp()
                {
                    ProjectId = x.ProjectId,
                    Name = x.Name,
                    Description = x.Description,
                    OrganizationId = x.OrganizationId,
                    Status = x.Status
                }).ToList();
            }

            return list;
        }

        public async Task<ProjectWithServiceListRp> GetProjectsWithServices(Guid organizationId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            ProjectWithServiceListRp list = new ProjectWithServiceListRp();

            var projects = user.FindProjects(organizationId);

            if (projects != null)
            {
                list.Items = projects.Select(x => new ProjectWithServiceListItemRp()
                {
                    ProjectId = x.ProjectId,
                    Name = x.Name,
                    Description = x.Description,
                    OrganizationId = x.OrganizationId,
                    Status = x.Status,
                    Services = new ProjectServiceListRp() {
                        Items = x.GetServices().Select(s=> new ProjectServiceListItemRp
                        {
                            ProjectServiceId = s.ProjectServiceId,
                            Name = s.Name,
                            Description = s.Description,
                            Status = s.Status,
                            PipelineStatus = s.PipelineStatus
                        }).ToList()
                    },
                    Users = new ProjectUserListRp()
                    {
                        Items = x.GetProjectUsers().Select(s => new ProjectUserListItemRp
                        {
                            UserName = s.User.GetFullName(),
                            UserEmail = s.User.Email,
                            Role = s.Role,
                            AddedDate = s.CreationDate
                        }).ToList()
                    }
                }).ToList();
            }

            return list;
        }

        public async Task<ProjectGetRp> GetProjectById(Guid organizationId, Guid projectId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            Project project = user.FindProjectById(projectId);
            if (project == null)
            {
                return null;
            }

            ProjectGetRp projectRp = new ProjectGetRp()
            {
                ProjectId = project.ProjectId,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                GitProviderType = project.OrganizationCMS.Type,
                CloudProviderType = project.OrganizationCPS.Type,
                OrganizationName = project.Organization.Name,
                AgentPoolId = project.AgentPoolId
            };

            return projectRp;
        }

        public async Task<ProjectExternalGetRp> GetProjectExternalById(Guid organizationId, Guid projectId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            Project project = user.FindProjectById(projectId);
            if (project == null)
            {
                return null;
            }

            OrganizationCMS organizationCMS = organization.GetConfigurationManagementServiceById(project.OrganizationCMSId);
            
            var projectCloudCredential = this._cloudCredentialService.ProjectCredentialResolver(organizationCMS, project);

            var accountId = projectCloudCredential.AccountName;
            var projectExternalId = projectCloudCredential.ProjectExternalId;

            try
            {
                var projectExternal = await this._cmsQueryService(projectCloudCredential.CMSType)
                   .GetProject(accountId, projectExternalId, projectCloudCredential.CMSAuthCredential);

                return new ProjectExternalGetRp
                {
                    Name = projectExternal.Name,
                    Description = projectExternal.Description,
                    Url = projectExternal.Link.Web.Href
                };
            }
            catch (Exception)
            {
                return new ProjectExternalGetRp {
                    Name = project.ProjectExternalName,
                    Description = project.ProjectExternalName,
                    Url = string.Empty
                };
            }

        }

        public async Task<ProjectEnvironmentSummaryGetRp> GetProjectEnvironmentSummayById(Guid organizationId, Guid projectId, Guid? environmentId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            Project project = user.FindProjectById(projectId);
            if (project == null)
            {
                return null;
            }

            int baseRank = 999;
            if (environmentId.HasValue && environmentId.Value != Guid.Empty)
            {
                var projectEnvironment = project.GetEnvironmentById(environmentId.Value);
                if(projectEnvironment == null)
                {
                    await _domainManagerService.AddNotFound($"The environment with id {environmentId.Value} does not exists.");
                    return null;
                }

                baseRank = projectEnvironment.Rank;
            }

            ProjectEnvironmentSummaryGetRp projectEnvironmentSummaryGetRp = new ProjectEnvironmentSummaryGetRp();
            var projectEnvironments = project.Environments.Where(x=> x.Rank < baseRank).OrderBy(x => x.Rank);
            foreach (var item in projectEnvironments)
            {
                projectEnvironmentSummaryGetRp.Environments.Add(new ProjectEnvironmentSummaryEnvironmentGetRp() {
                    Id = item.ProjectEnvironmentId,
                    Name = item.Name,
                    Rank = item.Rank
                });
            }

            var projectServices = project.Services;
            foreach (var item in projectServices)
            {
                var projectServiceEnvironments = item.Environments.Where(x => x.ProjectEnvironment.Rank < baseRank).OrderBy(x => x.ProjectEnvironment.Rank);
                projectEnvironmentSummaryGetRp.Services.Add(new ProjectEnvironmentSummaryServiceGetRp() {
                    Id = item.ProjectServiceId,
                    Name = item.Name,
                    Environments = projectServiceEnvironments.Select(x => new ProjectEnvironmentSummaryServiceEnvironmentGetRp()
                    {
                        Id = x.ProjectEnvironmentId,
                        LastSuccessVersionId = x.LastSuccessVersionId,
                        LastSuccessVersionName = x.LastSuccessVersionName
                    }).ToList()
                });
            }

            return projectEnvironmentSummaryGetRp;
        }
    }
}
