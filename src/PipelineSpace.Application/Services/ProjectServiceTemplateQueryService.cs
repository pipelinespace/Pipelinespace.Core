using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class ProjectServiceTemplateQueryService : IProjectServiceTemplateQueryService
    {
        readonly IProjectServiceTemplateRepository _projectServiceTemplateRepository;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly ITemplateService _templateService;

        public ProjectServiceTemplateQueryService(IProjectServiceTemplateRepository projectServiceTemplateRepository,
            Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService,
            IOptions<VSTSServiceOptions> vstsOptions,
            ITemplateService templateService)
        {
            _projectServiceTemplateRepository = projectServiceTemplateRepository;
            this._vstsOptions = vstsOptions;
            this._cmsCredentialService = cmsCredentialService;
            this._templateService = templateService;
        }

        public async Task<ProjectServiceTemplateListRp> GetProjectServiceTemplates(ConfigurationManagementService gitProviderType, CloudProviderService cloudProviderType, PipeType pipeType)
        {
            var serviceTemplates = await _projectServiceTemplateRepository.GetProjectServiceTemplates(gitProviderType, cloudProviderType, pipeType);

            ProjectServiceTemplateListRp list = new ProjectServiceTemplateListRp
            {
                Items = serviceTemplates.Select(x => new ProjectServiceTemplateListItemRp()
                {
                    ProjectServiceTemplateId = x.ProjectServiceTemplateId,
                    Name = x.Name,
                    Description = x.Description
                }).ToList()
            };

            return list;
        }

        public async Task<ProjectServiceTemplateListRp> GetProjectServiceTemplateInternals(Guid programmingLanguageId, CloudProviderService cloudProviderType)
        {
            var serviceTemplates = await _projectServiceTemplateRepository.GetProjectServiceTemplateInternals(programmingLanguageId, cloudProviderType);

            ProjectServiceTemplateListRp list = new ProjectServiceTemplateListRp
            {
                Items = serviceTemplates.Select(x => new ProjectServiceTemplateListItemRp()
                {
                    ProjectServiceTemplateId = x.ProjectServiceTemplateId,
                    Name = x.Name,
                    Description = x.Description,
                    ProgrammingLanguageName = x.ProgrammingLanguage.Name,
                    Framework = x.Framework
                }).ToList()
            };

            return list;
        }

        public async Task<ProjectServiceTemplateDefinitionGetRp> GetProjectServiceTemplateDefinition(Guid projectServiceTemplateId)
        {
            var serviceTemplate = await this._projectServiceTemplateRepository.FindFirst(c => c.ProjectServiceTemplateId.Equals(projectServiceTemplateId));

            //Auth
            CMSAuthCredentialModel cmsAuthCredential = this._cmsCredentialService(ConfigurationManagementService.VSTS).GetToken(
                                                                this._vstsOptions.Value.AccountId,
                                                                this._vstsOptions.Value.AccessId,
                                                                this._vstsOptions.Value.AccessSecret,
                                                                this._vstsOptions.Value.AccessSecret);

            var templateSplit = serviceTemplate.Url.Split('/');
            var repository = templateSplit[templateSplit.Length - 1];
            var templateName = $"{serviceTemplate.Path}";

            var templateBuildDefinition = await this._templateService.GetTemplateBuildDefinition(repository, templateName, "build.definition.yml", cmsAuthCredential);
            var templateInfraDefinition = await this._templateService.GetTemplateInfraDefinition(repository, templateName, serviceTemplate.ServiceCPSType == CloudProviderService.AWS ? "infra.definition.yml" : "infra.definition.json", cmsAuthCredential);

            return new ProjectServiceTemplateDefinitionGetRp {
                ProjectServiceTemplateId = serviceTemplate.ProjectServiceTemplateId,
                Name = serviceTemplate.Name,
                Description = serviceTemplate.Description,
                Framework = serviceTemplate.Framework,
                ProgrammingLanguageName = serviceTemplate.Framework,
                BuildDefinition = templateBuildDefinition.Content,
                InfraDefinition = templateInfraDefinition.Content
            };
        }
    }
}
