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
    public class ProjectServiceEnvironmentQueryService : IProjectServiceEnvironmentQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly Func<DomainModels.CloudProviderService, ICPSQueryService> _cpsQueryService;
        readonly IDataProtectorService _dataProtectorService;

        public ProjectServiceEnvironmentQueryService(IDomainManagerService domainManagerService,
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

        public async Task<ProjectServiceEnvironmentListRp> GetProjectServiceEnvironments(Guid organizationId, Guid projectId, Guid serviceId)
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

            DomainModels.ProjectService projectService = project.GetServiceById(serviceId);
            if (projectService == null)
            {
                await _domainManagerService.AddNotFound($"The project service with id {serviceId} does not exists.");
                return null;
            }

            ProjectServiceEnvironmentListRp list = new ProjectServiceEnvironmentListRp();

            if (project.OrganizationCPS.Type == DomainModels.CloudProviderService.None)
                return list;

            CPSAuthCredentialModel authCredentials = new CPSAuthCredentialModel();
            authCredentials.AccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId);
            authCredentials.AccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName);
            authCredentials.AccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret);
            authCredentials.AccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion);
            authCredentials.AccessAppId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppId);
            authCredentials.AccessAppSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppSecret);
            authCredentials.AccessDirectory = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessDirectory);
            
            if (projectService.Environments != null)
            {
                List<ProjectServiceEnvironmentListItemRp> projectServiceEnvironmentList = new List<ProjectServiceEnvironmentListItemRp>();
                var environments = projectService.Environments.OrderBy(x => x.ProjectEnvironment.Rank);
                foreach (var item in environments)
                {
                    var projectServiceEnvironmentListItem = new ProjectServiceEnvironmentListItemRp();
                    projectServiceEnvironmentListItem.ProjectServiceEnvironmentId = item.ProjectServiceEnvironmentId;
                    projectServiceEnvironmentListItem.Name = item.ProjectEnvironment.Name;
                    projectServiceEnvironmentListItem.Status = item.Status;
                    projectServiceEnvironmentListItem.Summary = await _cpsQueryService(project.OrganizationCPS.Type).GetEnvironmentSummary(organization.Name, project.Name, projectService.Name, item.ProjectEnvironment.Name, "Root", authCredentials);
                    projectServiceEnvironmentListItem.Variables = item.Variables.Select(p => new ProjectServiceEnvironmentVariableListItemRp()
                    {
                        Name = p.Name,
                        Value = p.Value
                    }).ToList();
                    projectServiceEnvironmentList.Add(projectServiceEnvironmentListItem);
                }
                list.Items = projectServiceEnvironmentList;
            }

            return list;
        }
    }
}
