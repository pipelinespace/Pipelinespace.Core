using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.InternalServices.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices
{
    public class InternalProjectService : IInternalProjectService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectRepository _projectRepository;
        readonly Func<ConfigurationManagementService, ICMSService> _cmsService;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;
        readonly IOptions<FakeAccountServiceOptions> _vstsFakeOptions;
        readonly ISlugService _slugService;

        public InternalProjectService(IDomainManagerService domainManagerService,
            Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService,
                                   Func<ConfigurationManagementService, ICMSService> cmsService,
                                      IProjectRepository projectRepository,
                                      ISlugService slugService,
                                      IOptions<FakeAccountServiceOptions> vstsFakeOptions)
        {
            _domainManagerService = domainManagerService;
            _projectRepository = projectRepository;
            _cmsService = cmsService;
            _cmsCredentialService = cmsCredentialService;
            _vstsFakeOptions = vstsFakeOptions;
            _slugService = slugService;
        }

        public async Task PatchProject(Guid organizationId, Guid projectId, ProjectPatchRp resource)
        {
            var project = await _projectRepository.GetProjectById(organizationId, projectId);

            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            if (!string.IsNullOrEmpty(resource.ProjectVSTSFakeId))
            {
                project.ProjectVSTSFakeId = resource.ProjectVSTSFakeId;
            }

            if (!string.IsNullOrEmpty(resource.ProjectVSTSFakeName))
            {
                project.ProjectVSTSFakeName = resource.ProjectVSTSFakeName;
            }

            if (!string.IsNullOrEmpty(resource.ProjectExternalId))
            {
                project.ProjectExternalId = resource.ProjectExternalId;
            }

            if (!string.IsNullOrEmpty(resource.ProjectExternalName))
            {
                project.ProjectExternalName = resource.ProjectExternalName;
            }

            if (!string.IsNullOrEmpty(resource.ProjectExternalEndpointId))
            {
                project.ProjectExternalEndpointId = resource.ProjectExternalEndpointId;
            }

            if (!string.IsNullOrEmpty(resource.ProjectExternalGitEndpoint))
            {
                project.ProjectExternalGitEndpoint = resource.ProjectExternalGitEndpoint;
            }

            _projectRepository.Update(project);

            await _projectRepository.SaveChanges();
        }

        public async Task ActivateProject(Guid organizationId, Guid projectId)
        {
            var project = await _projectRepository.GetProjectById(organizationId, projectId);

            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            if (project.Status != EntityStatus.Preparing)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status NEW to be activated.");
                return;
            }

            project.Activate();

            _projectRepository.Update(project);

            await _projectRepository.SaveChanges();
        }

        public async Task CreateVSTSFakeProject(Guid organizationId, Guid projectId)
        {
            var project = await _projectRepository.GetProjectWithOrgAndAccountOwnerByProjectId(organizationId, projectId);

            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            var cmsType = ConfigurationManagementService.VSTS;
            var projectName = this._slugService.GetSlug($"{project.Organization.Owner.Email} {project.Organization.Name} {project.Name}");
            var resource = new {
                Name = projectName,
                Description = project.Description
            };

            CMSAuthCredentialModel cmsAuthCredential = this._cmsCredentialService(cmsType).GetToken(_vstsFakeOptions.Value.AccountId, _vstsFakeOptions.Value.AccountId, _vstsFakeOptions.Value.AccessSecret);
            CMSProjectAvailabilityResultModel cmsProjectAvailability = await _cmsService(cmsType).ValidateProjectAvailability(cmsAuthCredential, string.Empty, resource.Name);

            if (!cmsProjectAvailability.Success)
            {
                await _domainManagerService.AddConflict($"The CMS data is not valid. {cmsProjectAvailability.GetReasonForNoSuccess()}");
                return;
            }

            CMSProjectCreateModel projectCreateModel = CMSProjectCreateModel.Factory.Create(project.Organization.Name, resource.Name, resource.Description, ProjectVisibility.Private);
            CMSProjectCreateResultModel cmsProjectCreate = await _cmsService(cmsType).CreateProject(cmsAuthCredential, projectCreateModel);

            if (!cmsProjectCreate.Success)
            {
                await _domainManagerService.AddConflict($"The CMS data is not valid. {cmsProjectCreate.GetReasonForNoSuccess()}");
                return;
            }

            project.SetFakeVSTSProject(projectName);

            this._projectRepository.Update(project);
            await this._projectRepository.SaveChanges();
        }
    }
}
