using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PipelineSpace.Domain;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public class ProjectEnvironmentActivatedHandler : IEventHandler<ProjectEnvironmentActivatedEvent>
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly Func<ConfigurationManagementService, IProjectServiceHandlerService> _projectServiceHandlerService;
        readonly Func<CloudProviderService, ICPSService> _cpsService;
        readonly IPipelineSpaceManagerService _pipelineSpaceManagerService;

        public ProjectEnvironmentActivatedHandler(IOptions<VSTSServiceOptions> vstsOptions,
                                                  IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                                  IOptions<ApplicationOptions> applicationOptions,
                                                  Func<ConfigurationManagementService, IProjectServiceHandlerService> projectServiceHandlerService,
                                                  Func<CloudProviderService, ICPSService> cpsService,
                                                  IPipelineSpaceManagerService pipelineSpaceManagerService)
        {
            _vstsOptions = vstsOptions;
            _applicationOptions = applicationOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _projectServiceHandlerService = projectServiceHandlerService;
            _cpsService = cpsService;
            _pipelineSpaceManagerService = pipelineSpaceManagerService;
        }

        public async Task Handle(ProjectEnvironmentActivatedEvent @event)
        {
            ReadReleaseDefinitionOptions readReleaseDefinitionOptions = new ReadReleaseDefinitionOptions();
            readReleaseDefinitionOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            readReleaseDefinitionOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
            readReleaseDefinitionOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
            readReleaseDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

            readReleaseDefinitionOptions.OrganizationName = @event.OrganizationName;
            readReleaseDefinitionOptions.ProjectName = @event.ProjectName;
            readReleaseDefinitionOptions.ReleaseStageId = @event.ReleseStageId;

            var releaseDefinition = await _pipelineSpaceManagerService.GetReleaseDefinition(readReleaseDefinitionOptions);

            var productionEnvironment = releaseDefinition.Environments.FirstOrDefault(x => x.Name.Equals(DomainConstants.Environments.Production));
            var lastAvailableEnvironment = releaseDefinition.Environments.Where(x => !x.Name.Equals(DomainConstants.Environments.Production)).OrderByDescending(x => x.Id).First();

            var existingEnvironment = releaseDefinition.Environments.FirstOrDefault(x => x.Name.Equals(@event.EnvironmentName, StringComparison.InvariantCultureIgnoreCase));
            if (existingEnvironment != null)
            {
                /*Environment Variables*/
                var baseEnvironmentVariables = ((JObject)existingEnvironment.Variables);
                var propertyEnvironmentEnable = ((JObject)baseEnvironmentVariables).GetValue("PS_ENVIRONMENT_ENABLE");
                if (propertyEnvironmentEnable != null)
                {
                    propertyEnvironmentEnable["value"] = "True";
                }
            }

            releaseDefinition.PipelineProcess = new CMSVSTSReleaseDefinitionPipelineProcessCreateModel() { Type = "designer" };
            releaseDefinition.Properties = new CMSVSTSReleaseDefinitionPropertiesCreateModel()
            {
                DefinitionCreationSource = new CMSVSTSReleaseDefinitionPropertiesItemCreateModel()
                {
                    Type = "System.String",
                    Value = "ReleaseNew"
                }
            };

            UpdateReleaseDefinitionOptions updateReleaseDefinitionOptions = new UpdateReleaseDefinitionOptions();
            updateReleaseDefinitionOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            updateReleaseDefinitionOptions.VSTSAccountName = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccountName : _fakeAccountOptions.Value.AccountId;
            updateReleaseDefinitionOptions.VSTSAccessSecret = @event.CMSType == ConfigurationManagementService.VSTS ? @event.CMSAccessSecret : _fakeAccountOptions.Value.AccessSecret;
            updateReleaseDefinitionOptions.VSTSAccountProjectId = @event.CMSType == ConfigurationManagementService.VSTS ? @event.ProjectName : @event.ProjectVSTSFakeName;

            updateReleaseDefinitionOptions.OrganizationName = @event.OrganizationName;
            updateReleaseDefinitionOptions.ProjectName = @event.ProjectName;
            updateReleaseDefinitionOptions.Model = releaseDefinition;

            await _pipelineSpaceManagerService.UpdateReleaseDefinition(updateReleaseDefinitionOptions);
        }
    }
}
