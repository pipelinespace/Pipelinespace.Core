using Force.DeepCloner;
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
    public class ProjectFeatureEnvironmentCreatedHandler : IEventHandler<ProjectFeatureEnvironmentCreatedEvent>
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly Func<ConfigurationManagementService, IProjectServiceHandlerService> _projectServiceHandlerService;
        readonly IPipelineSpaceManagerService _pipelineSpaceManagerService;

        public ProjectFeatureEnvironmentCreatedHandler(IOptions<VSTSServiceOptions> vstsOptions,
                                                       IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                                       IOptions<ApplicationOptions> applicationOptions,
                                                       Func<ConfigurationManagementService, IProjectServiceHandlerService> projectServiceHandlerService,
                                                       IPipelineSpaceManagerService pipelineSpaceManagerService)
        {
            _vstsOptions = vstsOptions;
            _applicationOptions = applicationOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _projectServiceHandlerService = projectServiceHandlerService;
            _pipelineSpaceManagerService = pipelineSpaceManagerService;
        }

        public async Task Handle(ProjectFeatureEnvironmentCreatedEvent @event)
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

            var developmentEnvironment = releaseDefinition.Environments.FirstOrDefault(x => x.Name.Equals(DomainConstants.Environments.Development));
            var lastAvailableEnvironment = releaseDefinition.Environments.Where(x=> !x.Name.Equals(DomainConstants.Environments.Production)).OrderByDescending(x => x.Id).First();
            CMSVSTSReleaseDefinitionEnvironmentCreateModel newEnvironment = null;

            var environments = @event.Environments.Where(x => !x.Name.Equals(DomainConstants.Environments.Production)).OrderBy(x => x.Rank);
            foreach (var environment in environments)
            {
                var existingEnvironment = releaseDefinition.Environments.FirstOrDefault(x => x.Name.Equals(environment.Name, StringComparison.InvariantCultureIgnoreCase));
                if (existingEnvironment == null)
                {
                    newEnvironment = lastAvailableEnvironment.DeepClone();
                    newEnvironment.Id = lastAvailableEnvironment.Id > 0 ? -1 : (lastAvailableEnvironment.Id - 1);
                    newEnvironment.Name = environment.Name;
                    newEnvironment.Rank = environment.Rank;
                    newEnvironment.Owner = lastAvailableEnvironment.Owner;
                    newEnvironment.Conditions = new List<CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel>()
                            {
                                new CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel()
                                {
                                    ConditionType = "2",
                                    EnvironmentId = lastAvailableEnvironment.Id,
                                    Name = lastAvailableEnvironment.Name,
                                    Value = "4"
                                }
                            };

                    //Change Variables Values
                    Dictionary<string, string> environmentVariableValues = new Dictionary<string, string>();
                    environmentVariableValues.Add("ASPNETCORE_ENVIRONMENT", environment.Name.ToLower());
                    environmentVariableValues.Add("PS_ENVIRONMENT_ENABLE", "True");
                    
                    foreach (var item in environment.Variables)
                    {
                        environmentVariableValues.Add(item.Name, item.Value);
                    }

                    var newEnvironmentVariables = ((JObject)newEnvironment.Variables).DeepClone();
                    foreach (var item in environmentVariableValues)
                    {
                        var property = ((JObject)newEnvironmentVariables).GetValue(item.Key);
                        if (property != null)
                        {
                            property["value"] = item.Value;
                        }
                        else
                        {
                            var newEnvironmentVariablesObj = ((JObject)newEnvironmentVariables);
                            dynamic jsonObject = new JObject();
                            jsonObject.value = item.Value;
                            newEnvironmentVariablesObj.Add(item.Key, jsonObject);
                        }
                    }
                    newEnvironment.Variables = newEnvironmentVariables;
                    newEnvironment.DeployPhases[0].DeploymentInput.Condition = $"and(not(contains(variables['Release.ReleaseDescription'], 'PS_SKIP_ENVIRONMENT_{environment.Name}')), eq(variables['PS_ENVIRONMENT_ENABLE'], 'True'))";

                    if (environment.RequiredApproval)
                    {
                        newEnvironment.PreDeployApprovals = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalCreateModel()
                        {
                            ApprovalOptions = new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalOptionCreateModel()
                            {
                                AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped = false,
                                EnforceIdentityRevalidation = false,
                                ExecutionOrder = "1",
                                releaseCreatorCanBeApprover = true,
                                TimeoutInMinutes = 0
                            },
                            Approvals = new List<CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel>()
                                {
                                    new CMSVSTSReleaseDefinitionEnvironmentDeployAprovalItemCreateModel()
                                    {
                                        IsAutomated = false,
                                        IsNotificationOn = false,
                                        Rank = 1,
                                        Approver = lastAvailableEnvironment.Owner
                                    }
                                }
                        };
                    }

                    releaseDefinition.Environments.Add(newEnvironment);
                    lastAvailableEnvironment = newEnvironment;
                }
                else
                {
                    //Change Variables Values
                    Dictionary<string, string> environmentVariableValues = new Dictionary<string, string>();
                    environmentVariableValues.Add("ASPNETCORE_ENVIRONMENT", environment.Name.ToLower());
                    environmentVariableValues.Add("PS_ENVIRONMENT_ENABLE", "True");
                    
                    foreach (var item in environment.Variables)
                    {
                        environmentVariableValues.Add(item.Name, item.Value);
                    }

                    var environmentVariables = ((JObject)existingEnvironment.Variables).DeepClone();
                    foreach (var item in environmentVariableValues)
                    {
                        var property = ((JObject)environmentVariables).GetValue(item.Key);
                        if (property != null)
                        {
                            property["value"] = item.Value;
                        }
                        else
                        {
                            var newEnvironmentVariablesObj = ((JObject)environmentVariables);
                            dynamic jsonObject = new JObject();
                            jsonObject.value = item.Value;
                            newEnvironmentVariablesObj.Add(item.Key, jsonObject);
                        }
                    }

                    if(!existingEnvironment.Name.Equals(developmentEnvironment.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        existingEnvironment.Conditions = new List<CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel>()
                            {
                                new CMSVSTSReleaseDefinitionEnvironmentConditionCreateModel()
                                {
                                    ConditionType = "2",
                                    EnvironmentId = lastAvailableEnvironment.Id,
                                    Name = lastAvailableEnvironment.Name,
                                    Value = "4"
                                }
                            };
                    }

                    existingEnvironment.Rank = environment.Rank;
                    existingEnvironment.Variables = environmentVariables;
                    existingEnvironment.DeployPhases[0].DeploymentInput.Condition = $"and(not(contains(variables['Release.ReleaseDescription'], 'PS_SKIP_ENVIRONMENT_{environment.Name}')), eq(variables['PS_ENVIRONMENT_ENABLE'], 'True'))";

                    lastAvailableEnvironment = existingEnvironment;
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
