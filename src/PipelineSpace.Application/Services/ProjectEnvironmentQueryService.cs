using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using System;
using DomainModels = PipelineSpace.Domain.Models;
using System.Collections.Generic;
using System.Text;
using PipelineSpace.Application.Models;
using System.Threading.Tasks;
using System.Linq;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Application.Extensions;
using System.Collections.Concurrent;

namespace PipelineSpace.Application.Services
{
    public class ProjectEnvironmentQueryService : IProjectEnvironmentQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly Func<DomainModels.CloudProviderService, ICPSQueryService> _cpsQueryService;
        readonly IDataProtectorService _dataProtectorService;

        public ProjectEnvironmentQueryService(IDomainManagerService domainManagerService,
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

        public async Task<ProjectEnvironmentListRp> GetProjectEnvironments(Guid organizationId, Guid projectId)
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

            ProjectEnvironmentListRp list = new ProjectEnvironmentListRp();

            if (project.Environments != null)
            {
                list.Items = project.Environments.Select(x => new ProjectEnvironmentListItemRp()
                {
                    ProjectEnvironmentId = x.ProjectEnvironmentId,
                    Name = x.Name,
                    Description = x.Description,
                    Type = x.Type,
                    Status = x.Status,
                    Rank = x.Rank
                }).OrderBy(x=> x.Rank).ToList();


                var services = project.Services.Select(x => new 
                {
                    ProjectServiceId = x.ProjectServiceId,
                    Name = x.Name
                }).ToList();

                CPSAuthCredentialModel authCredentials = new CPSAuthCredentialModel();
                authCredentials.AccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId);
                authCredentials.AccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName);
                authCredentials.AccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret);
                authCredentials.AccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion);
                authCredentials.AccessAppId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppId);
                authCredentials.AccessAppSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppSecret);
                authCredentials.AccessDirectory = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessDirectory);

                if (services != null && services.Any()) {

                    foreach (var env in list.Items)
                    {
                        var projectServiceEnvironments = new ProjectServiceEnvironmentListRp();
                        var projectServiceEnvironmentList = new ConcurrentBag<ProjectServiceEnvironmentListItemRp>();

                        await services.ForEachAsync(5, async service =>
                        {
                            var projectServiceEnvironmentListItem = new ProjectServiceEnvironmentListItemRp();
                            projectServiceEnvironmentListItem.Name = service.Name;
                            projectServiceEnvironmentListItem.Summary = await _cpsQueryService(project.OrganizationCPS.Type).GetEnvironmentSummary(organization.Name, project.Name, service.Name, env.Name, "Root", authCredentials);
                            projectServiceEnvironmentList.Add(projectServiceEnvironmentListItem);
                        });

                        projectServiceEnvironments.Items = projectServiceEnvironmentList.ToList();

                        env.Services = projectServiceEnvironments;
                        
                    }

                }
            }

            return list;
        }

        public async Task<ProjectEnvironmentGetRp> GetProjectEnvironmentById(Guid organizationId, Guid projectId, Guid environmentId)
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

            DomainModels.ProjectEnvironment environment = project.GetEnvironmentById(environmentId);
            if (environment == null)
            {
                return null;
            }

            ProjectEnvironmentGetRp environmentRp = new ProjectEnvironmentGetRp
            {
                ProjectEnvironmentId = environment.ProjectEnvironmentId,
                Name = environment.Name,
                Description = environment.Description,
                Status = environment.Status,
                Type = environment.Type
            };

            return environmentRp;
        }

        public async Task<ProjectEnvironmentVariableListRp> GetProjectEnvironmentVariables(Guid organizationId, Guid projectId, Guid environmentId)
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

            DomainModels.ProjectEnvironment environment = project.GetEnvironmentById(environmentId);
            if (environment == null)
            {
                return null;
            }

            DomainModels.ProjectEnvironment defaultEnvironment = project.GetRootEnvironment();
            ProjectEnvironmentVariableListRp list = new ProjectEnvironmentVariableListRp();

            if (environment.Variables != null)
            {
                var variables = from dv in defaultEnvironment.Variables
                                join ev in environment.Variables on dv.Name equals ev.Name into ev
                                from p in ev.DefaultIfEmpty()
                                select new ProjectEnvironmentVariableListItemRp
                                {
                                    Name = dv.Name,
                                    Value = p == null ? "" : p.Value
                                };

                list.Items = variables.ToList();
            }

            return list;
        }
    }
}
